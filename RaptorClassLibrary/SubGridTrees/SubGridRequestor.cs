﻿using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using VSS.VisionLink.Raptor.Common;
using VSS.VisionLink.Raptor.Filters;
using VSS.VisionLink.Raptor.Geometry;
using VSS.VisionLink.Raptor.Interfaces;
using VSS.VisionLink.Raptor.Services.Surfaces;
using VSS.VisionLink.Raptor.SiteModels;
using VSS.VisionLink.Raptor.SubGridTrees.Client;
using VSS.VisionLink.Raptor.SubGridTrees.Interfaces;
using VSS.VisionLink.Raptor.Surfaces;
using VSS.VisionLink.Raptor.Surfaces.GridFabric.Arguments;
using VSS.VisionLink.Raptor.Surfaces.GridFabric.Requests;
using VSS.VisionLink.Raptor.Types;

namespace VSS.VisionLink.Raptor.SubGridTrees
{
    public class SubGridRequestor
    {
        [NonSerialized]
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        [NonSerialized]
        private SubGridRetriever retriever = null;

        [NonSerialized]
        SiteModel SiteModel = null;

        [NonSerialized]
        CombinedFilter Filter = null;

        [NonSerialized]
        SurfaceElevationPatchRequest surfaceElevationPatchRequest = null;

        [NonSerialized]
        byte TreeLevel = SubGridTree.SubGridTreeLevels;

        [NonSerialized]
        bool HasOverrideSpatialCellRestriction = false;

        [NonSerialized]
        BoundingIntegerExtent2D OverrideSpatialCellRestriction;

        [NonSerialized]
        int MaxNumberOfPassesToReturn = int.MaxValue;

        [NonSerialized]
        private SubGridCellAddress SubGridAddress;

        [NonSerialized]
        private bool ProdDataRequested;

        [NonSerialized]
        private bool SurveyedSurfaceDataRequested;

        [NonSerialized]
        private  IClientLeafSubGrid ClientGrid;

        [NonSerialized]
        private SurfaceElevationPatchArgument SurfaceElevationPatchArg = null;

        [NonSerialized]
        private uint CellX;

        [NonSerialized]
        private uint CellY;

        [NonSerialized]
        public SubGridTreeBitmapSubGridBits CellOverrideMask;

        [NonSerialized]
        public AreaControlSet AreaControlSet = AreaControlSet.Null();

        // For height requests, the ProcessingMap is ultimately used to indicate which elevations were provided from a surveyed surface (if any)
        [NonSerialized]
        private SubGridTreeBitmapSubGridBits ProcessingMap; // = new SubGridTreeBitmapSubGridBits(SubGridTreeBitmapSubGridBits.SubGridBitsCreationOptions.Unfilled);

        [NonSerialized]
        private SurveyedSurfaces FilteredSurveyedSurfaces = null;


        /// <summary>
        /// Default no-arg constructor
        /// </summary>
        public SubGridRequestor(SiteModel sitemodel,
                                CombinedFilter filter,
                                bool hasOverrideSpatialCellRestriction,
                                BoundingIntegerExtent2D overrideSpatialCellRestriction,
                                byte treeLevel,
                                int maxNumberOfPassesToReturn,
                                AreaControlSet areaControlSet)
        {
            SiteModel = sitemodel;
            Filter = filter;
            TreeLevel = treeLevel;
            HasOverrideSpatialCellRestriction = hasOverrideSpatialCellRestriction;
            OverrideSpatialCellRestriction = overrideSpatialCellRestriction;
            MaxNumberOfPassesToReturn = maxNumberOfPassesToReturn;

            retriever = new SubGridRetriever(sitemodel,
                                             filter,
                                             hasOverrideSpatialCellRestriction,
                                             overrideSpatialCellRestriction,
                                             false, // Assigned(SubgridCache), //ClientGrid.SupportsAssignationFromCachedPreProcessedClientSubgrid
                                             treeLevel,
                                             maxNumberOfPassesToReturn,
                                             areaControlSet
                                             );

            surfaceElevationPatchRequest = new SurfaceElevationPatchRequest();

            // Instantiate a single instance of the argument object for the surface elevation patch requests and populate it with 
            // the common elements for this set of subgrids being requested
            SurfaceElevationPatchArg = new SurfaceElevationPatchArgument()
            {
                SiteModelID = SiteModel.ID,
                EarliestSurface = Filter.AttributeFilter.ReturnEarliestFilteredCellPass
            };

            AreaControlSet = areaControlSet;

            FilteredSurveyedSurfaces = new SurveyedSurfaces();

            ProcessingMap = new SubGridTreeBitmapSubGridBits(SubGridTreeBitmapSubGridBits.SubGridBitsCreationOptions.Unfilled);
       }

    // InitialiseFilterContext performs any required filter initialisation and configuration
    // that is external to the filter prior to engaging in cell by cell processing of
    // this subgrid
    private bool InitialiseFilterContext(CombinedFilter Filter)
        {
            if (Filter == null)
            {
                return true;
            }

            /* TODO Not yet supported
            ErrorCode: TDesignProfilerRequestResult;

            if (Filter.AttributeFilter.HasElevationRangeFilter)
            {
                ClonedFilter = TICGridDataPassFilter.Create(null);
                ClonedFilter.Assign(PassFilter);

                PassFilter = ClonedFilter;
                PassFilter.ClearElevationRangeFilterInitialisation;

                // If the elevation range filter uses a design then the design elevations
                // for the subgrid need to be calculated and supplied to the filter

                if (!Filter.AttributeFilter.ElevationRangeDesign.IsNull)
                {
                    // Query the DesignProfiler service to get the patch of elevations calculated
                    ErrorCode = PSNodeImplInstance.DesignProfilerService.RequestDesignElevationPatch
                                 (Construct_CalculateDesignElevationPatch_Args(SiteModel.Grid.DataModelID,
                                                                               SubgridX, SubgridY,
                                                                               SiteModel.Grid.CellSize,
                                                                               PassFilter.ElevationRangeDesign,
                                                                               TSubGridTreeLeafBitmapSubGridBits.FullMask),
                                 DesignElevations);

                    if (ErrorCode != dppiOK || DesignElevations == null)
                    {
                        return false;
                    }
                }

                PassFilter.InitialiseElevationRangeFilter(DesignElevations);
            }
            */

            /*

    if CellFilter.HasDesignFilter then
      begin
//        SIGLogMessage.PublishNoODS(Nil, Format('#D# InitialiseFilterContext RequestDesignElevationPatch for Design %s',[CellFilter.DesignFilter.FileName]), slmcDebug);
        // Query the DesignProfiler service to get the patch of elevations calculated
        ErrorCode := PSNodeImplInstance.DesignProfilerService.RequestDesignElevationPatch
                     (Construct_CalculateDesignElevationPatch_Args(SiteModel.Grid.DataModelID,
                                                                   SubgridX, SubgridY,
                                                                   SiteModel.Grid.CellSize,
                                                                   CellFilter.DesignFilter,
                                                                   TSubGridTreeLeafBitmapSubGridBits.FullMask),
                     DesignFilterElevations);

        if (ErrorCode <> dppiOK) or not Assigned(DesignFilterElevations) then
          begin
            SIGLogMessage.PublishNoODS(Nil, Format('#D# InitialiseFilterContext RequestDesignElevationPatch for Design %s Failed',[CellFilter.DesignFilter.FileName]), slmcError);
            Result := False;
           Exit;
          end;
      end;
*/
            return true;
        }

        private ServerRequestResult PerformDataExtraction()
        {
            // note there is an assumption you have already checked on a existenance map that there is a subgrid for this address

            // TICClientSubGridTreeLeaf_CellProfile ClientGridAsCellProfile = null
            // bool ClientGrid_is_TICClientSubGridTreeLeaf_HeightAndTime;
            // bool ClientGrid_is_TICClientSubGridTreeLeaf_CellProfile;
            // ISubGrid CachedSubgrid = null;
            // ISubGrid ClientGrid2;
            // bool AddedSubgridToSubgridCache;
            // ILeafSubGrid TempClientGrid;
            // CombinedFilter ClonedFilter = null;
            // bool SubgridAlreadyPresentInCache = false;

            ClientHeightLeafSubGrid DesignElevations = null;
            ServerRequestResult Result = ServerRequestResult.UnknownError;

            // Log.Info("Entering RequestSubGridInternal");

            /* TODO - subgrid general result cache not supported yet
            // Determine if there is a suitable pre-calculated result present
            // in the general subgrid result cache. If there is, then apply the
            // filter mask to the cached data and copy it to the client grid
            if (SubgridCache != null)
            {
                CachedSubgrid = SubgridCache.Lookup(ClientGrid.OriginAsCellAddress);
            }
            else
            {
                CachedSubgrid = null;
            }

            // If there was a cached subgrid located, assign
            // it's contents according the client grid mask into the client grid and return it
            if (CachedSubgrid != null)
            {
                try
                {
                    // Compute the matching filter mask that the full processing would have computed
                    if ConstructSubgridCellFilterMask(ClientGrid, SiteModel, CellFilter,
                                                      CellOverrideMask, AHasOverrideSpatialCellRestriction, AOverrideSpatialCellRestriction,
                                                      ClientGrid.ProdDataMap, ClientGrid.FilterMap)
                          {
                        // If we have DesignElevations at this point, then a Lift filter is in operation and
                        // we need to use it to constrain the returned client grid to the extents of the design elevations
                        if (DesignElevations != null)
                        {
                            TempClientGrid = ClientGrid;
                            TempClientGrid.FilterMap.IterateOverSetBits(procedure(const X, Y: Integer) begin TempClientGrid.FilterMap.SetBitValue(X, Y, DesignElevations.CellHasValue(X, Y)); end);
                        }

                        // Use that mask to copy the relevant cells from the cache to the client subgrid
                        ClientGrid.AssignFromCachedPreProcessedClientSubgrid(CachedSubgrid, ClientGrid.FilterMap);

                        Result = ServerRequestResult.NoError;
                    }
                    else
                    {
                        Result = ServerRequestResult.FailedToComputeDesignFilterPatch;
                    }
                }
                finally
                {
                    // The lookup of the cached subgrid performs a reference of the subgrid.
                    // The reference needs to be offset by a DeReference to indicate this
                    // interest in the subgrid is no longer required.
                    CachedSubgrid.DeReference;
                }
            }
            */

            if (false)
            {
                // TODO placeholder for cache implementation above
            }
            else
            {
                Result = retriever.RetrieveSubGrid(// DataStoreInstance.GridDataCache,
                                                   CellX, CellY,
                                                   // LiftBuildSettings,
                                                   ClientGrid,
                                                   CellOverrideMask,
                                                   // ASubgridLockToken,
                                                   DesignElevations);

                /* TODO: General subgrid result caching not yet supported
                // If a subgrid was retrieved and this is a supported data type in the cache
                // then add it to the cache
                if (Result = ServerRequestResult.NoError && (SubgridCache != null))
                {
                    // Don't add subgrids computed using a non-trivial WMS seive to the general subgrid cache
                    if (AAreaControlSet.PixelXWorldSize == 0 && AAreaControlSet.PixelYWorldSize == 0)
                    {
                        // Add the newly computed client subgrid to the cache
                        AddedSubgridToSubgridCache = SubgridCache.Add(ClientGrid, SubgridAlreadyPresentInCache);

                        try
                        {
                            if (!AddedSubgridToSubgridCache && !SubgridAlreadyPresentInCache)
                            {
                                // TODO Add when logging available
                                // SIGLogMessage.PublishNoODS(Nil, Format('Failed to add subgrid %s, data model %d to subgrid result cache', [ClientGrid.Moniker, SiteModel.ID]), slmcWarning);
                            }

                            // Create a clone of the client grid that has the filter mask applied to
                            // returned the requested set of cell values back to the caller
                            ClientGrid2 = PSNodeImplInstance.RequestProcessor.RequestClientGrid(ClientGrid.GridDataType,
                                                                                                ClientGrid.CellSize,
                                                                                                ClientGrid.IndexOriginOffset);

                            // If we have DesignElevations at this point, then a Lift filter is in operation and
                            // we need to use it to constrain the returned client grid to the extents of the design elevations
                            if (DesignElevations != null)
                            {
                                TempClientGrid = ClientGrid;
                                TempClientGrid.FilterMap.IterateOverSetBits((x, y) => { TempClientGrid.FilterMap.SetBitValue(X, Y, DesignElevations.CellHasValue(X, Y)); });
                            }

                            ClientGrid2.Assign(ClientGrid);
                            ClientGrid2.AssignFromCachedPreProcessedClientSubgrid(ClientGrid, ClientGrid.FilterMap);
                        }
                        finally
                        {
                            // Remove interest in the cached client grid if it was previously added to the cache
                            if (AddedSubgridToSubgridCache)
                            {
                                ClientGrid.DeReference;
                            }
                            else // If not added to the cache, release it back to the pool
                            {
                                PSNodeImplInstance.RequestProcessor.RepatriateClientGrid(ClientGrid);
                            }
                        }

                        ClientGrid = ClientGrid2;
                    }
                }
                */
            }

            //if VLPDSvcLocations.Debug_ExtremeLogSwitchB then
            //  SIGLogMessage.PublishNoODS(Nil, 'Completed call to RetrieveSubGrid()', slmcDebug);

            return Result;
        }

        /// <summary>
        /// Annotates height information with elevations from surveyed surfaces?
        /// </summary>
        private ServerRequestResult PerformHeightAnnotation()
        {
            ClientHeightAndTimeLeafSubGrid ClientGridAsHeightAndTime = null;
            ClientHeightAndTimeLeafSubGrid SurfaceElevations = null;
            bool SurveyedSurfaceElevationWanted;

            ServerRequestResult Result = ServerRequestResult.NoError;

            // if VLPDSvcLocations.Debug_SwitchOffCompositeSurfaceGenerationFromSurveyedSurfaces then
            // Exit;

            bool ClientGrid_is_TICClientSubGridTreeLeaf_HeightAndTime = ClientGrid is ClientHeightAndTimeLeafSubGrid;

            ///* TODO - cell profiles not yet supported
            // ClientGrid_is_TICClientSubGridTreeLeaf_CellProfile = ClientGrid is ClientCellProfileLeafSubGrid; // TICClientSubGridTreeLeaf_CellProfile;

            if (!(ClientGrid_is_TICClientSubGridTreeLeaf_HeightAndTime /* || ClientGrid_is_TICClientSubGridTreeLeaf_CellProfile */))
            {
                return ServerRequestResult.NoError;
            }

            SurveyedSurfaces SurveyedSurfaceList = SiteModel.SurveyedSurfaces;
            if (SurveyedSurfaceList.Count() == 0)
            {
                return ServerRequestResult.NoError;
            }

            // Obtain local reference to surveyed surface list. If it is replaced while processing the
            // list then the local reference will still be valid allowing lock free read access to the list.
            FilteredSurveyedSurfaces.Clear(); // = new SurveyedSurfaces();

            // Filter out any surveyed surfaces which don't match current filter (if any) - realistically, this is time filters we're thinking of here
            SurveyedSurfaceList.FilterSurveyedSurfaceDetails(Filter.AttributeFilter.HasTimeFilter,
                 Filter.AttributeFilter.StartTime, Filter.AttributeFilter.EndTime,
                 Filter.AttributeFilter.ExcludeSurveyedSurfaces(), FilteredSurveyedSurfaces,
                 Filter.AttributeFilter.SurveyedSurfaceExclusionList);

            if (FilteredSurveyedSurfaces.Count == 0)
            {
                return ServerRequestResult.NoError;
            }

            bool ReturnEarliestFilteredCellPass = Filter.AttributeFilter.ReturnEarliestFilteredCellPass;

            // Ensure that the filtered ground surfaces are in a known ordered state
            FilteredSurveyedSurfaces.SortChronologically();

            if (ClientGrid_is_TICClientSubGridTreeLeaf_HeightAndTime)
            {
                ClientGridAsHeightAndTime = ClientGrid as ClientHeightAndTimeLeafSubGrid;

                // Temporarily just fill the processing map as not doing so results in no surveyed surfaced information being requested
                ProcessingMap.Fill();

                // ProcessingMap.Assign(ClientGridAsHeightAndTime.FilterMap);

                // If we're interested in a particular cell, but we don't have any
                // surveyed surfaces later (or earlier) than the cell production data
                // pass time (depending on PassFilter.ReturnEarliestFilteredCellPass)
                // then there's no point in asking the Design Profiler service for an elevation
                Int64[,] Times = ClientGridAsHeightAndTime.Times;

                ProcessingMap.ForEachSetBit((x, y) =>
                {
                    if (ClientGridAsHeightAndTime.Cells[x, y] != Consts.NullHeight &&
                        (ReturnEarliestFilteredCellPass ? !FilteredSurveyedSurfaces.HasSurfaceEarlierThan(Times[x, y]) : !FilteredSurveyedSurfaces.HasSurfaceLaterThan(Times[x, y])))
                    {
                        ProcessingMap.ClearBit(x, y);
                    }
                });
            }
            /*
            else
            if (ClientGrid_is_TICClientSubGridTreeLeaf_CellProfile)
            {
                ClientGridAsCellProfile = TICClientSubGridTreeLeaf_CellProfile(ClientGrid);
                ProcessingMap.Assign(ClientGridAsCellProfile .FilterMap);

                // If we're interested in a particular cell, but we don't have any
                // surveyed surfaces later (or earlier) than the cell production data
                // pass time (depending on PassFilter.ReturnEarliestFilteredCellPass)
                // then there's no point in asking the Design Profiler service for an elevation
                if (Result == ServerRequestResult.NoError)
                {
                    ProcessingMap.ForEachSetBit((x, y) =>
                    {
                        if (ClientGridAsCellProfile.Cells[x, y].Height == Consts.NullHeight)
                        {
                            return;
                        }

                        if (Filter.AttributeFilter.ReturnEarliestFilteredCellPass)
                        {
                            if (!FilteredSurveyedSurfaces.HasSurfaceEarlierThan(ClientGridAsCellProfile.Cells[x, y].Time))
                            {
                                ProcessingMap.ClearBit(x, y);
                            }
                        }
                        else
                        {
                            if (!FilteredSurveyedSurfaces.HasSurfaceLaterThan(ClientGridAsCellProfile.Cells[x, y].Time))
                            {
                                ProcessingMap.ClearBit(x, y);
                            }
                        }
                    });
                }
            }
            */

            // If we still have any cells to request surveyed surface elevations for...
            if (ProcessingMap.IsEmpty())
            {
                return Result;
            }

            try
            {
                // Hand client grid details, a mask of cells we need surveyed surface elevations for, and a temp grid to the Design Profiler
                SurfaceElevationPatchArg.CellSize = ClientGrid.CellSize;
                SurfaceElevationPatchArg.OTGCellBottomLeftX = ClientGrid.OriginX;
                SurfaceElevationPatchArg.OTGCellBottomLeftY = ClientGrid.OriginY;
                SurfaceElevationPatchArg.ProcessingMap = ProcessingMap;
                SurfaceElevationPatchArg.IncludedSurveyedSurfaces = FilteredSurveyedSurfaces;

                SurfaceElevations = surfaceElevationPatchRequest.Execute(SurfaceElevationPatchArg);
                if (SurfaceElevations == null)
                {
                    return Result;
                }

                //ClientHeightLeafSubGrid temp = new ClientHeightLeafSubGrid(null, null, 6, 0.34, SubGridTree.DefaultIndexOriginOffset);
                //temp.Assign(SurfaceElevations);

                // For all cells we wanted to request a surveyed surface elevation for,
                // update the cell elevation if a non null surveyed surface of appropriate
                // time was computed
                float ProdHeight;
                Int64 ProdTime;
                float SurveyedSurfaceCellHeight;
                Int64 SurveyedSurfaceCellTime;

                ProcessingMap.ForEachSetBit((x, y) =>
                {
                    SurveyedSurfaceCellHeight = SurfaceElevations.Cells[x, y];
                    SurveyedSurfaceCellTime = SurfaceElevations.Times[x, y];

                    // If we got back a surveyed surface elevation...ComputeFuncs

                    if (ClientGrid_is_TICClientSubGridTreeLeaf_HeightAndTime)
                    {
                        ProdHeight = ClientGridAsHeightAndTime.Cells[x, y];
                        ProdTime = ClientGridAsHeightAndTime.Times[x, y];
                    }
                    else
                    /*
                    if (ClientGrid_is_TICClientSubGridTreeLeaf_CellProfile)
                    {
                        ProdHeight = ClientGridAsCellProfile.Cells[x, y].Height;
                        ProdTime = ClientGridAsCellProfile.Cells[x, y].LastPassTime;
                    }
                    else
                    */
                    {
                        ProdHeight = Consts.NullHeight; // should not get here
                        ProdTime = DateTime.MinValue.ToBinary();
                    }

                    if (ReturnEarliestFilteredCellPass)
                    {
                        SurveyedSurfaceElevationWanted = (SurveyedSurfaceCellHeight != Consts.NullHeight) &&
                                                         ((ProdHeight == Consts.NullHeight) || (SurveyedSurfaceCellTime < ProdTime));
                    }
                    else
                    {
                        SurveyedSurfaceElevationWanted = (SurveyedSurfaceCellHeight != Consts.NullHeight) &&
                                                         ((ProdHeight == Consts.NullHeight) || (SurveyedSurfaceCellTime > ProdTime));
                    }

                    if (SurveyedSurfaceElevationWanted)
                    {
                        // Check if there is an elevation range filter in effect and whether the
                        // surveyed surface elevation data matches it

                        bool ContinueProcessing = true;

                        if (Filter.AttributeFilter.HasElevationRangeFilter)
                        {
                            Filter.AttributeFilter.InitaliaseFilteringForCell((byte)x, (byte)y);

                            if (!Filter.AttributeFilter.FiltersElevation(SurveyedSurfaceCellHeight))
                            {
                                // We didn't get a surveyed surface elevation, so clear the bit so that ASNode won't render it as a surveyed surface
                                ProcessingMap.ClearBit(x, y);
                                ContinueProcessing = false;
                            }
                        }

                        if (ContinueProcessing)
                        {
                            if (ClientGrid_is_TICClientSubGridTreeLeaf_HeightAndTime)
                            {
                                ClientGridAsHeightAndTime.Cells[x, y] = SurveyedSurfaceCellHeight;
                                ClientGridAsHeightAndTime.Times[x, y] = SurveyedSurfaceCellTime;
                            }
                            /*
                            else
                            {
                                if (ClientGrid_is_TICClientSubGridTreeLeaf_CellProfile)
                                {
                                    ClientGridAsCellProfile.Cells[I, J] = SurveyedSurfaceCellHeight;
                                }
                            }
                            */
                        }
                    }
                    else
                    {
                        // We didn't get a surveyed surface elevation, so clear the bit so that the renderer won't render it as a surveyed surface
                        ProcessingMap.ClearBit(x, y);
                    }
                });

                if (ClientGrid_is_TICClientSubGridTreeLeaf_HeightAndTime)
                {
                    ClientGridAsHeightAndTime.SurveyedSurfaceMap.Assign(ProcessingMap);
                }

                Result = ServerRequestResult.NoError;
            }
            finally
            {
                // TODO: Use client subgrid pool...
                //    PSNodeImplInstance.RequestProcessor.RepatriateClientGrid(TICSubGridTreeLeafSubGridBase(SurfaceElevations));
            }

            return Result;
        }

        /// <summary>
        /// Responsible for coordinating the retrieval of production data for a subgrid from a site model and also annotating it with
        /// surveyd surface informationk for requests involving height data.
        /// </summary>
        /// <param name="subGridAddress"></param>
        /// <param name="prodDataRequested"></param>
        /// <param name="surveyedSurfaceDataRequested"></param>
        /// <param name="clientGrid"></param>
        /// <returns></returns>
        public ServerRequestResult RequestSubGridInternal(// SubgridCache : TDataModelContextSubgridResultCache;
                                                          SubGridCellAddress subGridAddress,
                                                          // LiftBuildSettings: TICLiftBuildSettings;
                                                          // ASubgridLockToken : Integer;
                                                          bool prodDataRequested,
                                                          bool surveyedSurfaceDataRequested,
                                                          IClientLeafSubGrid clientGrid
                                                          )
        {
            SubGridAddress = subGridAddress;
            ProdDataRequested = prodDataRequested;
            SurveyedSurfaceDataRequested = surveyedSurfaceDataRequested;
            ClientGrid = clientGrid;

            ServerRequestResult Result = ServerRequestResult.UnknownError;

            if (!(ProdDataRequested || SurveyedSurfaceDataRequested))
            {
                return ServerRequestResult.MissingInputParameters;
            }

            if (!InitialiseFilterContext(Filter))
            {
                return ServerRequestResult.FilterInitialisationFailure;
            }

            // For now, it is safe to assume all subgrids containing on-the-ground cells have kSubGridTreeLevels levels
            CellX = subGridAddress.X << ((SubGridTree.SubGridTreeLevels - TreeLevel) * SubGridTree.SubGridIndexBitsPerLevel);
            CellY = subGridAddress.Y << ((SubGridTree.SubGridTreeLevels - TreeLevel) * SubGridTree.SubGridIndexBitsPerLevel);

            // if VLPDSvcLocations.Debug_ExtremeLogSwitchB then
            //    SIGLogMessage.PublishNoODS(Nil, 'About to call RetrieveSubGrid()', slmcDebug);

            ClientGrid.SetAbsoluteOriginPosition((uint)(subGridAddress.X & ~SubGridTree.SubGridLocalKeyMask),
                                                 (uint)(subGridAddress.Y & ~SubGridTree.SubGridLocalKeyMask));
            ClientGrid.SetAbsoluteLevel(TreeLevel);

            if (ProdDataRequested)
            {
                Result = PerformDataExtraction();

                if (Result != ServerRequestResult.NoError)
                {
                    return Result;
                }
            }

            if (SurveyedSurfaceDataRequested)
            {
                // Construct the filter mask (e.g. spatial filtering) to be applied to the results of surveyd surface analysis
                if (SubGridFilterMasks.ConstructSubgridCellFilterMask(ClientGrid, SiteModel, Filter,
                                                                      CellOverrideMask,
                                                                      HasOverrideSpatialCellRestriction,
                                                                      OverrideSpatialCellRestriction,
                                                                      ref (ClientGrid as ClientLeafSubGrid).ProdDataMap,
                                                                      ref (ClientGrid as ClientLeafSubGrid).FilterMap))
                {
                    Result = ServerRequestResult.NoError;
                }
                else
                {
                    Result = ServerRequestResult.FailedToComputeDesignFilterPatch;
                }

                Result = PerformHeightAnnotation();
            }

            return Result;
        }
    }
}
