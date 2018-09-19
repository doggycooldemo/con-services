﻿using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.Extensions.Logging;
using VSS.TRex.Common;
using VSS.TRex.CoordinateSystems;
using VSS.TRex.Designs.Interfaces;
using VSS.TRex.DI;
using VSS.TRex.Events;
using VSS.TRex.Events.Interfaces;
using VSS.TRex.Exceptions;
using VSS.TRex.Geometry;
using VSS.TRex.Machines;
using VSS.TRex.Machines.Interfaces;
using VSS.TRex.SiteModels.Interfaces;
using VSS.TRex.SiteModels.Interfaces.Events;
using VSS.TRex.Storage.Interfaces;
using VSS.TRex.SubGridTrees;
using VSS.TRex.SubGridTrees.Core.Utilities;
using VSS.TRex.SubGridTrees.Interfaces;
using VSS.TRex.SubGridTrees.Server;
using VSS.TRex.SubGridTrees.Server.Interfaces;
using VSS.TRex.SurveyedSurfaces.Interfaces;
using VSS.TRex.Types;
using VSS.TRex.Utilities.ExtensionMethods;

namespace VSS.TRex.SiteModels
{
    public class SiteModel : ISiteModel
    {
        [NonSerialized]
        private static readonly ILogger Log = Logging.Logger.CreateLogger(MethodBase.GetCurrentMethod().DeclaringType?.Name);

        public const string kSiteModelXMLFileName = "ProductionDataModel.XML";
        public const string kSubGridExistanceMapFileName = "SubGridExistanceMap";

        private const int kMajorVersion = 1;
        private const int kMinorVersion = 0;
        private const int kMinorVersionLatest = 0;

        public Guid ID { get; set; } = Guid.Empty;

        public DateTime LastModifiedDate { get; set; } 

        /// <summary>
        /// Gets/sets transient state for this sitemodel. Transient site models are not persisted.
        /// </summary>
        public bool IsTransient { get; private set; } = true;

        /// <summary>
        /// The grid data for this site model
        /// </summary>
        [NonSerialized]
        private IServerSubGridTree grid;

        /// <summary>
        /// The grid data for this site model
        /// </summary>
        public IServerSubGridTree Grid { get { return grid; } }

        [NonSerialized]
        private ISubGridTreeBitMask existanceMap;

        /// <summary>
        /// Returns a reference to the existance map for the site model. If the existance map is not yet present
        /// load it from storage/cache
        /// </summary>
        public ISubGridTreeBitMask ExistanceMap
        {
          get { return existanceMap ?? GetProductionDataExistanceMap(); }
        }

        /// <summary>
        /// Gets the loaded state of the existence map. This permits testing if an existance map is loaded without forcing
        /// the existence map to be loaded via the ExistenceMap property
        /// </summary>
        public bool ExistenceMapLoaded
        {
          get => existanceMap != null;
        }

        /// <summary>
        /// SiteModelExtent records the 3D extents of the data stored in the site model
        /// </summary>
        public BoundingWorldExtent3D SiteModelExtent { get; } = BoundingWorldExtent3D.Inverted();

        /// <summary>
        /// Local cached copy of the coordiante system CSIB
        /// </summary>
        private string csib = null;

        /// <summary>
        /// The string serialized CSIB gained from adding a coordinate system from a DC or similar file
        /// to the project. This getter is reponsible for accessing the information from the persistent
        /// store and caching it in the site model
        /// </summary>
        public string CSIB()
        {
          if (csib != null)
            return csib;

          FileSystemErrorStatus readResult = 
            DIContext.Obtain<ISiteModels>().StorageProxy.
              ReadStreamFromPersistentStore(ID, 
                                            CoordinateSystemConsts.kCoordinateSystemCSIBStorageKeyName, 
                                            FileSystemStreamType.CoordinateSystemCSIB, 
                                            out MemoryStream csibStream);

          if (readResult != FileSystemErrorStatus.OK || csibStream == null || csibStream.Length == 0)
            return null;

          using (csibStream)
          {
            return Encoding.ASCII.GetString(csibStream.ToArray());
          }
        }


        /// <summary>
        /// Gets the loaded state of the CSIB. This permits testing if a CSIB is loaded without forcing
        /// the CSIB to be loaded via the CSIB property
        /// </summary>
        public bool CSIBLoaded
        {
          get => csib != null;
        }
  
        // ProofingRuns is the set of proofing runs that have been collected in this site model
        // public SiteProofingRuns ProofingRuns;

        // MachinesTargetValues stores a list of target values, one list per machine,
        // that record how the cofigured target CCV and pass count settings on each
        // machine has changed over time.
        [NonSerialized]
        private IMachinesProductionEventLists machinesTargetValues;
        public IMachinesProductionEventLists MachinesTargetValues
        {
          // Allow lazy loading of the machine event lists to occur organically.
          // Any requests holding references to events lists will continue to do so as the lists themselves
          // wont be garbage collected until all request references to them are relinquished
          get => machinesTargetValues ?? (machinesTargetValues = new MachinesProductionEventLists(this, Machines));
          private set => machinesTargetValues = value;
        }

        public bool MachineTargetValuesLoaded
        {
          get => machinesTargetValues != null;
        }

        private SiteModelDesignList siteModelDesigns = new SiteModelDesignList();

        /// <summary>
        /// SiteModelDesigns records all the designs that have been seen in this sitemodel.
        /// Each site model designs records the name of the site model and the extents
        /// of the cell information that have been record for it.
        /// </summary>
        public ISiteModelDesignList SiteModelDesigns { get { return siteModelDesigns; } }

        private ISurveyedSurfaces surveyedSurfaces = null;

        // This is a list of TTM descriptors which indicate designs
        // that can be used as a snapshot of an actual ground surface at a specific point in time
        public ISurveyedSurfaces SurveyedSurfaces
        {
            get => surveyedSurfaces ?? (surveyedSurfaces = DIContext.Obtain<ISurveyedSurfaceManager>().List(ID));
        }

        public bool SurveyedSurfacesLoaded
        {
            get => surveyedSurfaces != null;
        }

        private IDesigns designs = null;
    
        /// <summary>
        /// Designs records all the design surfaces that have been imported into the sitemodel
        /// </summary>
        public IDesigns Designs 
        {
            get => designs ?? (designs = DIContext.Obtain<IDesignManager>().List(ID));
        }

        public bool DesignsLoaded
        {
            get => designs != null;
        }

        // FSiteModelDesignNames is an integrated list of all the design names that have appeared
        // in design change events. It shadows the FSiteModelDesigns to an alarming degree
        // and FSiteModelDesigns could either be refactored to use it, or the two could be
        // merged in intent.
        // public SiteModelDesignNames : TICClientDesignNames;
      
        // Machines contains a list of compactor machines that this site model knows
        // about. Each machine contains a link to the machine hardware ID for the
        // appropriate machine

        private IMachinesList machines { get; set; }
     
        public IMachinesList Machines
        {
          get
          {
            if (machines == null)
            {
              machines = new MachinesList();
              machines.LoadFromPersistentStore();
            }

            return machines;
          }
        }

        public bool MachinesLoaded
        {
          get => machines != null;
        }

        public bool IgnoreInvalidPositions { get; set; } = true;

        public SiteModel()
        {
          LastModifiedDate = DateTime.MinValue;
        }

        /// <summary>
        /// Constructs a sitemodel from an 'origin' sitemodel that provides select information to seed the new site model
        /// </summary>
        /// <param name="originModel"></param>
        /// <param name="originFlags"></param>
        public SiteModel(ISiteModel originModel, SiteModelOriginConstructionFlags originFlags) : this()
        {
          if (originModel.IsTransient)
            throw new TRexException("Cannot use a transient sitemodel as an origin for constructing a new site model");

          ID = originModel.ID;

          // FCreationDate:= Now;
          // FName:= Format('SiteModel-%d', [AID]);
          // FDescription:= '';
          // FActive:= True;

          IsTransient = false;

          LastModifiedDate = originModel.LastModifiedDate;

         // SiteModelDesignNames = LastModifiedDate.SiteModelDesignNames;

          grid = (originFlags & SiteModelOriginConstructionFlags.PreserveGrid) != 0 
            ? originModel.Grid : new ServerSubGridTree(originModel.ID);

          existanceMap = originModel.ExistenceMapLoaded && (originFlags & SiteModelOriginConstructionFlags.PreserveExistenceMap) != 0
            ? originModel.ExistanceMap : null;

          designs = originModel.DesignsLoaded && (originFlags & SiteModelOriginConstructionFlags.PreserveDesigns) != 0 
            ? originModel.Designs : null;

          surveyedSurfaces = originModel.SurveyedSurfacesLoaded && (originFlags & SiteModelOriginConstructionFlags.PreserveSurveyedSurfaces) != 0
            ? originModel.SurveyedSurfaces : null;

          machines = originModel.MachinesLoaded && (originFlags & SiteModelOriginConstructionFlags.PreserveMachines) != 0
            ? originModel.Machines: null;

          // Machine target values are an extension vector from machines. If the machine have not changed
          machinesTargetValues = originModel.MachineTargetValuesLoaded && (originFlags & SiteModelOriginConstructionFlags.PreserveMachineTargetValues) != 0
            ? originModel.MachinesTargetValues
            : null;

          // FProofingRuns:= TICSiteProofingRuns.Create;

          // Reload the bits that need to be reloaded
          LoadFromPersistentStore();
        }

        public SiteModel(Guid id, bool isTransient = true) : this()
        {
            ID = id;

            // FCreationDate:= Now;
            // FName:= Format('SiteModel-%d', [AID]);
            // FDescription:= '';
            // FActive:= True;

            IsTransient = isTransient;

            Machines.DataModelID = ID;

            // FSiteModelDesignNames:= TICClientDesignNames.Create(FID);

            grid = new ServerSubGridTree(ID);

            // Allow existence map loading to be deferred/lazy on reference
            existanceMap = null;

            // FProofingRuns:= TICSiteProofingRuns.Create;
        }

        public SiteModel(//string name,
                         //string description,
                         Guid id,
                         double cellSize) : this(id)
        {
            //  FName := AName;
            //  FDescription := ADescription;
            Grid.CellSize = cellSize;
        }

        public void Include(ISiteModel Source)
        {
            // SiteModel extents
            SiteModelExtent.Include(Source.SiteModelExtent);
       
            // Proofing runs
            /* TODO: Proofing runs
            for (int I = 0; I < Source.ProofingRuns.ProofingRuns.Count; I++)
              with Source.ProofingRuns.ProofingRuns[I] do
                begin
                  Index := FProofingRuns.IndexOf(Name, MachineID, StartTime, EndTime);
        
                  if Index = -1 then
                    FProofingRuns.CreateNew(Name, MachineID, StartTime, EndTime, Extents)
                  else
                    begin
                      FProofingRuns.ProofingRuns[Index].Extents.Include(Extents);
                      if FProofingRuns.ProofingRuns[Index].StartTime > StartTime then
                        FProofingRuns.ProofingRuns[Index].StartTime := StartTime;
                      if FProofingRuns.ProofingRuns[Index].EndTime<EndTime then
                        FProofingRuns.ProofingRuns[Index].EndTime := EndTime;
                    end;
                end;
             */
            // Designs
            // Note: Design names are handled as a part of integration of machine events
        
            LastModifiedDate = Source.LastModifiedDate;
        }

        public void Write(BinaryWriter writer)
        {
            // Write the SiteModel attributes
            writer.Write(kMajorVersion);
            writer.Write(kMinorVersionLatest);
            // writer.Write(Name);
            // writer.Write(Description);
            writer.Write(ID.ToByteArray());

            // WriteBooleanToStream(Stream, FActive);

            //WriteBooleanToStream(Stream, FIgnoreInvalidPositions);

            writer.Write(Grid.CellSize);

            SiteModelExtent.Write(writer);

            //FProofingRuns.WriteToStream(Stream);
            //FSiteModelDesigns.WriteToStream(Stream);

            // Write the design names list
            //FSiteModelDesignNames.SaveToStream(Stream);

            writer.Write(LastModifiedDate.ToBinary());
        }

        public bool Read(BinaryReader reader)
        {
            // Read the SiteModel attributes
            int MajorVersion = reader.ReadInt32();
            int MinorVersion = reader.ReadInt32();

            if (!(MajorVersion == kMajorVersion && (MinorVersion == kMinorVersion)))
            {
                Log.LogError($"Unknown version number {MajorVersion}:{MinorVersion} in Read()");
                return false;
            }

            // Name = reader.ReadString();
            // Description = reader.ReadString();

            // Read the ID of the data model from the stream.
            // If the site model already has an assigned ID then
            // use this ID in favour of the ID read from the data model.
            Guid LocalID = reader.ReadGuid();

            if (ID == Guid.Empty)
            {
                ID = LocalID;
            }

            /* 
            Active = reader.ReadBool();
            if (!Active)
            {
                SIGLogMessage.PublishNoODS(Self, Format('Site model %d is not marked as active in the internal data model file, resetting to active', [FID]), slmcError);
                Active = true;
            }
            */

            // FIgnoreInvalidPositions:= ReadBooleanFromStream(Stream);

            double SiteModelGridCellSize = reader.ReadDouble();
            if (SiteModelGridCellSize < 0.001)
            {
                Log.LogError($"'SiteModelGridCellSize is suspicious: {SiteModelGridCellSize} for datamodel {ID}, setting to default");
                SiteModelGridCellSize = SubGridTreeConsts.DefaultCellSize; 
            }
            Grid.CellSize = SiteModelGridCellSize;

            SiteModelExtent.Read(reader);

            // FProofingRuns.ReadFromStream(Stream);
            // FSiteModelDesigns.ReadFromStream(Stream);

            // Read the design names list
            //FSiteModelDesignNames.LoadFromStream(Stream);

            LastModifiedDate = DateTime.FromBinary(reader.ReadInt64());

            return true;
        }

        public bool SaveToPersistentStore(IStorageProxy StorageProxy)
        {
            bool Result;

            using (MemoryStream MS = new MemoryStream())
            {
                using (BinaryWriter writer = new BinaryWriter(MS))
                {
                    lock (this)
                    {
                        Write(writer);

                        Result = StorageProxy.WriteStreamToPersistentStore(ID, kSiteModelXMLFileName, FileSystemStreamType.ProductionDataXML, MS) == FileSystemErrorStatus.OK
                                 && SaveProductionDataExistanceMapToStorage(StorageProxy) == FileSystemErrorStatus.OK;
                    }
                }
            }

            if (Result)
            {
                if (!IsTransient && TRexConfig.AdviseOtherServicesOfDataModelChanges)
                {
                  // Notify the site model in all contents in the grid that it's attributes have changed
                  Log.LogInformation($"Notifying site model attributes changed for {ID}");

                  // Notify the immutable grid listeners that attributes of this sitemodel have changed
                  DIContext.Obtain<ISiteModelAttributesChangedEventSender>()?.ModelAttributesChanged(ID);
                }
            }
            else
            {
                Log.LogError($"Failed to save site model for project {ID} to persistent store");
            }

            return Result;
        }

        public FileSystemErrorStatus LoadFromPersistentStore()
        {
            Guid SavedID = ID;
            FileSystemErrorStatus Result = DIContext.Obtain<ISiteModels>().StorageProxy.ReadStreamFromPersistentStoreDirect(ID, kSiteModelXMLFileName, FileSystemStreamType.ProductionDataXML, out MemoryStream MS);

            if (Result == FileSystemErrorStatus.OK && MS != null)
            {
                using (MS)
                {
                    if (SavedID != ID)
                    {
                        // The SiteModelID read from the FS file does not match the ID expected.

                        // RPW 31/1/11: This used to be an error with it's own error code. This is now
                        // changed to a warning, but loading of the sitemodel is allowed. This
                        // is particularly useful for testing purposes where copying around projects
                        // is much quicker than reprocessing large sets of TAG files

                        Log.LogWarning($"Site model ID read from FS file ({ID}) does not match expected ID ({SavedID}), setting to expected");
                        ID = SavedID;
                    }

                    MS.Position = 0;
                    using (BinaryReader reader = new BinaryReader(MS, Encoding.UTF8, true))
                    {
                        lock (this)
                        {
                            Read(reader);
                        }
                    }

                    if (Result == FileSystemErrorStatus.OK)
                    {
                        Log.LogDebug($"Site model read from FS file (ID:{ID}) succeeded");
                        Log.LogDebug($"Data model extents: {SiteModelExtent}, CellSize: {Grid.CellSize}");
                    }
                    else
                    {
                        Log.LogWarning($"Site model ID read from FS file ({ID}) failed with error {Result}");
                    }
                }
            }

            return Result;
        }

        /// <summary>
        /// Returns a reference to the existance map for the site model. If the existance map is not yet present
        /// load it from storage/cache
        /// </summary>
        /// <returns></returns>
        public ISubGridTreeBitMask GetProductionDataExistanceMap()
        {
            if (existanceMap == null)
            {
                return LoadProductionDataExistanceMapFromStorage() == FileSystemErrorStatus.OK ? existanceMap : null;
            }

            return existanceMap;
        }

        /// <summary>
        /// Saves the content of the existence map to storage
        /// </summary>
        /// <returns></returns>
        protected FileSystemErrorStatus SaveProductionDataExistanceMapToStorage(IStorageProxy StorageProxy)
        {
            try
            {
                // Create the new existance map instance
              if (existanceMap != null)
              {
                ISubGridTreeBitMask localExistanceMap = existanceMap;

                // Save its content to storage
                using (MemoryStream MS = new MemoryStream())
                {
                  using (BinaryWriter writer = new BinaryWriter(MS))
                  {
                    SubGridTreePersistor.Write(localExistanceMap, "ExistanceMap", 1, writer, null);
                    StorageProxy.WriteStreamToPersistentStoreDirect(ID, kSubGridExistanceMapFileName, FileSystemStreamType.SubgridExistenceMap, MS);
                  }
                }
              }
            }
            catch (Exception e)
            {
                Log.LogDebug($"Exception occurred: {e}");
                return FileSystemErrorStatus.UnknownErrorWritingToFS;
            }

            return FileSystemErrorStatus.OK;
        }

        /// <summary>
        /// Retrieves the content of the existance map from storage
        /// </summary>
        /// <returns></returns>
        protected FileSystemErrorStatus LoadProductionDataExistanceMapFromStorage()
        {
            try
            {
                // Create the new existance map instance
                ISubGridTreeBitMask localExistanceMap = new SubGridTreeSubGridExistenceBitMask();

                // Read its content from storage 
                DIContext.Obtain<ISiteModels>().StorageProxy.ReadStreamFromPersistentStoreDirect(ID, kSubGridExistanceMapFileName, FileSystemStreamType.ProductionDataXML, out MemoryStream MS);

                if (MS == null)
                {
                    Log.LogInformation($"Attempt to read existence map for site model {ID} failed as the map does not exist, creating new existence map");
                    existanceMap = new SubGridTreeSubGridExistenceBitMask();
                    return FileSystemErrorStatus.OK;
                }

                using (MS)
                {
                    using (BinaryReader reader = new BinaryReader(MS))
                    {
                        SubGridTreePersistor.Read(localExistanceMap, "ExistanceMap", 1, reader, null);
                    }
                }

                // Replace existance map with the newly read map
                existanceMap = localExistanceMap;
            }
            catch
            {
                return FileSystemErrorStatus.UnknownErrorReadingFromFS;
            }

            return FileSystemErrorStatus.OK;
        }

        /// <summary>
        /// GetAdjustedDataModelSpatialExtents returns the bounding extent of the production data held in the 
        /// data model expanded to include the bounding extents of the surveyed surfaces associated with the 
        /// datamodel, excepting those identitied in the SurveyedSurfaceExclusionList
        /// </summary>
        /// <returns></returns>
        public BoundingWorldExtent3D GetAdjustedDataModelSpatialExtents(Guid[] SurveyedSurfaceExclusionList)
        {
            if (SurveyedSurfaces == null || SurveyedSurfaces.Count == 0)
                return SiteModelExtent;

            // Start with the data model extents
            BoundingWorldExtent3D SpatialExtents = new BoundingWorldExtent3D(SiteModelExtent);

            // Iterate over all non-exluded surveyed surfaces and expand the SpatialExtents as necessary
            if (SurveyedSurfaceExclusionList == null || SurveyedSurfaceExclusionList.Length == 0)
            {
                foreach (ISurveyedSurface surveyedSurface in SurveyedSurfaces)
                    SpatialExtents.Include(surveyedSurface.Extents);
            }
            else
            {
                foreach (ISurveyedSurface surveyedSurface in SurveyedSurfaces)
                {
                    if (SurveyedSurfaceExclusionList.All(x => x != surveyedSurface.ID))
                        SpatialExtents.Include(surveyedSurface.Extents);
                }
            }

            return SpatialExtents;
        }
    }
}
