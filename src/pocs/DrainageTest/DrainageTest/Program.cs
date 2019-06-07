﻿using Core.Contracts.SurfaceImportExport;
using Core.Services.SurfaceImportExport;
using Microsoft.Practices.ServiceLocation;
using Morph.Contracts.Interfaces;
using Morph.Core.Utility;
using Morph.Module.Services.QAInputOutput;
using Morph.Module.Services.Utility.Fmx;
using Morph.Module.Services.Utility.MultiPlane;
using Morph.Services.Core.DataModel;
using Morph.Services.Core.Interfaces;
using Morph.Services.Core.Tools;
using SkuTester.DataModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Media;
//using System.Windows.Media.Imaging;
//using System.Windows.Media.Media3D;
using Microsoft.Practices.Prism.Logging;
using Newtonsoft.Json;
using Trimble.Vce.Data;
//using Trimble.Vce.Data.Skp;
//using Trimble.Vce.Data.Skp.SkpLib;
using Trimble.Vce.Geometry;


namespace DrainageTest
{
  public class Program
  {
    static void Main(string[] args)
    {
      var logger = (ILogger)null;
      try
      {
        var thread = new Thread(Program.CreateWPFApp);
        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
        Thread.Sleep(100);
        new BootStrapper().Run();
        logger = ServiceLocator.Current.GetInstance<ILogger>();

        //  try arg     ..\..\TestData\Sample\TestCase.xml
        StringBuilder stringBuilder = new StringBuilder();
        foreach (string str in args)
          stringBuilder.AppendFormat("{0} ", (object)str);
        logger.LogInfo(nameof(Main), "start: {0}", (object)stringBuilder.ToString());
        Program.Execute(args, logger);
      }
      catch (Exception ex)
      {
        Console.WriteLine($"{nameof(Main)}: exception: {ex}");
        logger.LogError($"{nameof(Main)} Exception: {ex}", Category.Exception.ToString());
      }
      finally
      {
        Application.Current.Dispatcher.Invoke((Action)(() => Application.Current.Shutdown()));
      }
    }

    private static void Execute(string[] args, ILogger logger)
    {
      var commandLineArgument = Program.ParseCommandLineArguments(args)["input"];
      logger.LogInfo(nameof(Execute), $"args: {commandLineArgument}");
      if (!File.Exists(commandLineArgument))
        throw new ArgumentException($"Unable to locate the xml file: {commandLineArgument}");

      var useCase = TestCase.Load(Path.GetFullPath(commandLineArgument));
      logger.LogInfo(nameof(Execute), $"XML loaded. surface Filename: {JsonConvert.SerializeObject(useCase.Surface)}");
      
      var flag = false;
      using (var instance = ServiceLocator.Current.GetInstance<ILandLeveling>())
      {
        var surfaceInfo = (ISurfaceInfo)null;
        if (StringComparer.InvariantCultureIgnoreCase.Compare(Path.GetExtension(useCase.Surface), ".dxf") == 0)
          surfaceInfo = instance.ImportSurface(useCase.Surface, (Action<float>) null);

        //else if (StringComparer.InvariantCultureIgnoreCase.Compare(Path.GetExtension(useCase.Surface), ".xml") == 0)
        //{
        //  FieldLevelData fieldLevelData = FieldLevelData.Load(useCase.Surface);
        //  Point3D transform = fieldLevelData.Origin.Transform;
        //  if (Math.Abs(transform.X) < 1E-06 && Math.Abs(transform.Y) < 1E-06 && Math.Abs(transform.Z) < 1E-06)
        //  {
        //    transform.Z = fieldLevelData.Origin.Altitude;
        //    fieldLevelData.Origin.Transform = transform;
        //  }
        //  surfaceInfo = instance.CreateSurface(Path.GetFileNameWithoutExtension(useCase.Surface), fieldLevelData.BoundaryPoints.Union<Point3D>(fieldLevelData.SurveyPoints), (IList<int>)null, fieldLevelData.BoundaryPoints.Select<Point3D, Point>((Func<Point3D, Point>)(p => p.ToPoint())), (IEnumerable<IEnumerable<Point>>)null, double.NaN);
        //}
        //else if (StringComparer.InvariantCultureIgnoreCase.Compare(Path.GetExtension(useCase.Surface), ".gbg") == 0)
        //{
        //  flag = true;
        //  SurfaceInfoDump surfaceInfoDump = SurfaceInfoDump.LoadSurface(useCase.Surface);
        //  logger.LogVerbose("SkuTester.Execute", "Morph TestCase file: {0} Boundary: {1} Point: {2} Triangle: {3} ", (object)useCase.Surface, (object)surfaceInfoDump.Boundary.Count, (object)surfaceInfoDump.Points.Count, (object)surfaceInfoDump.Triangles.Count);
        //  surfaceInfo = instance.CreateSurface(Path.GetFileNameWithoutExtension(useCase.Surface), (IEnumerable<Point3D>)surfaceInfoDump.Points, surfaceInfoDump.Triangles, (IEnumerable<Point>)surfaceInfoDump.Boundary, (IEnumerable<IEnumerable<Point>>)null, double.NaN);
        //}
        //else
        //{
        //  MultiPlaneParser multiPlaneParser = new MultiPlaneParser(logger, new Morph.Core.Utility.UnitsManager.UnitsManager(ServiceLocator.Current.GetInstance<UnitConverter>(), (IPreferences)null));
        //  MultiPlaneSettings multiPlaneSettings = new MultiPlaneSettings()
        //  {
        //    CoordinateSystem = useCase.IsXYZ ? MulitplaneCoordinateSystems.Xyz : MulitplaneCoordinateSystems.Yxz,
        //    DistanceType = useCase.IsMetric ? MultiplaneDistanceTypes.Meters : MultiplaneDistanceTypes.Feet,
        //    HasId = useCase.HasPointIds
        //  };
        //  string surface = useCase.Surface;
        //  MultiPlaneSettings settings = multiPlaneSettings;
        //  List<Point3D> source;
        //  ref List<Point3D> local1 = ref source;
        //  Origin origin;
        //  ref Origin local2 = ref origin;
        //  IDesignEditImports designEditImports;
        //  ref IDesignEditImports local3 = ref designEditImports;
        //  List<Point3D> pointsFromTextFile = multiPlaneParser.GetPointsFromTextFile(surface, settings, out local1, out local2, out local3);
        //  surfaceInfo = instance.CreateSurface(Path.GetFileNameWithoutExtension(useCase.Surface), (IEnumerable<Point3D>)pointsFromTextFile, (IList<int>)null, source.Select<Point3D, Point>((Func<Point3D, Point>)(p => p.ToPoint())), (IEnumerable<IEnumerable<Point>>)null, double.NaN);
        //}

        if (surfaceInfo == null)
          throw new ArgumentException($"Unable to create Surface from: {useCase.Surface}");

        if (!useCase.IsMetric)
          useCase = useCase.AsMetric();
        Design design = (Design) null;
        switch (useCase.Compute)
        {
          //    case ComputeEnum.SinglePlaneBestFit:
          //      if (useCase.Sections.Count == 0)
          //        useCase.Sections.Add(new PlanesConstraints()
          //        {
          //          Boundary = useCase.Boundary.Points.Count > 2 ? useCase.Boundary : new Linestring((IEnumerable<Point>)surfaceInfo.Boundary),
          //          MinimumSlope = useCase.MinSlope,
          //          MaximumSlope = useCase.MaxSlope,
          //          Shrinkage = useCase.Shrinkage,
          //          Bulkage = useCase.Bulkage
          //        });
          //      else if (!flag)
          //      {
          //        foreach (PlanesConstraints section in useCase.Sections)
          //        {
          //          section.Shrinkage = useCase.Shrinkage;
          //          section.Bulkage = useCase.Bulkage;
          //        }
          //      }
          //      design = instance.ComputePlanes((IList<PlanesConstraints>)useCase.Sections, (IEnumerable<Linestring>)useCase.ExclusionZones, useCase.ExportVolume, (Predicate<float>)null);
          //      break;

          case ComputeEnum.SurfaceBestFit:
            logger.LogInfo(nameof(Execute), $"Compute SurfaceBestFit. surface Name: {surfaceInfo.Name} pointCount: {surfaceInfo.Points.Count}");
            SurfaceConstraints constraints1 = new SurfaceConstraints();
            constraints1.Resolution = useCase.Resolution;
            constraints1.Boundary = useCase.Boundary;
            constraints1.TargetDitches = useCase.TargetDitches;
            constraints1.ExclusionZones.AddRange((IEnumerable<Linestring>) useCase.ExclusionZones);
            constraints1.Areas.AddRange((IEnumerable<AreaConstraints>) useCase.Areas);
            if (!constraints1.Areas.Any<AreaConstraints>())
              constraints1.Areas.Add(new AreaConstraints()
              {
                Tag = "Field",
                Boundary = useCase.Boundary.Points.Count > 2
                  ? useCase.Boundary
                  : new Linestring((IEnumerable<Point>) surfaceInfo.Boundary),
                MinimumSlope = useCase.MinSlope,
                MaximumSlope = useCase.MaxSlope,
                MaximumCutDepth = useCase.MaxCutDepth,
                MaximumFillHeight = useCase.MaxFillHeight,
                Shrinkage = useCase.Shrinkage,
                Bulkage = useCase.Bulkage,
                ExportVolume = useCase.ExportVolume
              });
            foreach (AreaConstraints area in useCase.Areas)
            {
              area.Shrinkage = useCase.Shrinkage;
              area.Bulkage = useCase.Bulkage;
            }

            for (int index = 0; index < constraints1.Areas.Count; ++index)
            {
              if (string.IsNullOrEmpty(constraints1.Areas[index].Tag))
                constraints1.Areas[index].Tag = $"Area{(object) (index + 1):00}";
            }

            design = instance.ComputeSurface(constraints1, (Predicate<float>) null);
            if (design == null)
              throw new ArgumentException($"Unable to create design from Surface: {useCase.Surface}");

            break;
          //    case ComputeEnum.Furrows:
          //      RowsConstraints constraints2 = new RowsConstraints();
          //      constraints2.Boundary = useCase.Boundary;
          //      constraints2.MinimumSlope = useCase.MinSlope;
          //      constraints2.MaximumSlope = useCase.MaxSlope;
          //      constraints2.MaximumSlopeChange = useCase.MaxSlopeChange;
          //      constraints2.MinimumCrossSlope = useCase.MinCrossSlope;
          //      constraints2.MaximumCrossSlope = useCase.MaxCrossSlope;
          //      constraints2.MaximumCrossSlopeChange = useCase.MaxCrossSlopeChange;
          //      constraints2.MaximumCutDepth = useCase.MaxCutDepth;
          //      constraints2.Pipeline = useCase.Pipeline;
          //      constraints2.RowsDirection = Morph.Services.Core.DataModel.Utils.NormalizeAngleRad(-useCase.FurrowHeading * (Math.PI / 180.0) + Math.PI / 2.0, -1.0 * Math.PI);
          //      constraints2.Resolution = useCase.Resolution;
          //      constraints2.Shrinkage = useCase.Shrinkage;
          //      constraints2.Bulkage = useCase.Bulkage;
          //      constraints2.ExportVolume = useCase.ExportVolume;
          //      constraints2.ExclusionZones.AddRange((IEnumerable<Linestring>)useCase.ExclusionZones);
          //      design = instance.ComputeRows(constraints2, (Predicate<float>)null);
          //      break;
          //    case ComputeEnum.Subzones:
          //      ZonesConstraints constraints3 = new ZonesConstraints();
          //      constraints3.Boundary = useCase.Boundary;
          //      constraints3.MainDirection = Morph.Services.Core.DataModel.Utils.NormalizeAngleRad(-useCase.MainHeading * (Math.PI / 180.0) + Math.PI / 2.0, -1.0 * Math.PI);
          //      constraints3.Resolution = useCase.Resolution;
          //      constraints3.ExclusionZones.AddRange((IEnumerable<Linestring>)useCase.ExclusionZones);
          //      constraints3.Subzones.AddRange((IEnumerable<SubzoneConstraints>)useCase.Zones);
          //      foreach (SubzoneConstraints zone in useCase.Zones)
          //      {
          //        zone.Shrinkage = useCase.Shrinkage;
          //        zone.Bulkage = useCase.Bulkage;
          //      }
          //      for (int index = 0; index < constraints3.Subzones.Count; ++index)
          //      {
          //        if (string.IsNullOrEmpty(constraints3.Subzones[index].Tag))
          //          constraints3.Subzones[index].Tag = string.Format("Subzone{0:00}", (object)(index + 1));
          //      }
          //      design = instance.ComputeZones(constraints3, (Predicate<float>)null);
          //      break;
          //    case ComputeEnum.Basins:
          //      BasinConstraints constraints4 = new BasinConstraints();
          //      constraints4.BasinBoundary = useCase.Boundary;
          //      constraints4.ExitPoint = useCase.ExitPoint;
          //      constraints4.Resolution = useCase.Resolution;
          //      constraints4.MinimumSlope = useCase.MinSlope;
          //      constraints4.MaximumSlope = useCase.MaxSlope;
          //      constraints4.ExportVolume = useCase.ExportVolume;
          //      constraints4.ExclusionZones.AddRange((IEnumerable<Linestring>)useCase.ExclusionZones);
          //      constraints4.Shrinkage = useCase.Shrinkage;
          //      constraints4.Bulkage = useCase.Bulkage;
          //      design = instance.ComputeBasin(constraints4, (Predicate<float>)null);
          //      break;
        }

        //string str =
        //  Path.ChangeExtension(
        //    Path.Combine(Path.GetDirectoryName(Path.GetFullPath(commandLineArgument)),
        //      Path.GetFileNameWithoutExtension(commandLineArgument)), "skp");
        //Program.CreateSketchupFile(str, surfaceInfo, design, useCase);
        //Morph.Services.Core.DataModel.Utils.LaunchSketchup(str);

      }
    }

    public static IDictionary<string, string> ParseCommandLineArguments(string[] args)
    {
      SortedList<string, string> sortedList = new SortedList<string, string>(args.Length);
      for (int index1 = 0; index1 < args.Length; ++index1)
      {
        if (args[index1].StartsWith("/") || args[index1].StartsWith("-"))
        {
          string str1 = args[index1].Substring(1);
          if (str1.Length > 1)
          {
            string index2 = str1;
            string str2 = string.Empty;
            int length = str1.IndexOf(':');
            if (length > 0)
            {
              index2 = str1.Substring(0, length);
              str2 = str1.Substring(length + 1);
            }
            sortedList[index2] = str2;
          }
        }
        else
          sortedList["input"] = args[index1];
      }
      return (IDictionary<string, string>)sortedList;
    }

    private static void CreateWPFApp()
    {
      new Application()
      {
        ShutdownMode = ShutdownMode.OnExplicitShutdown
      }.Run();
    }

  //private static void CreateSketchupFile(string skuFile, ISurfaceInfo surfaceInfo, Design design, TestCase useCase)
  //  {
  //    using (SkuModel skuModel = new SkuModel(useCase.IsMetric))
  //    {
  //      skuModel.Name = Path.GetFileNameWithoutExtension(skuFile);
  //      BitmapSource texture1 = (BitmapSource)null;
  //      if (useCase.OriginalVisualizationTools != null)
  //      {
  //        foreach (VizTool visualizationTool in useCase.OriginalVisualizationTools)
  //        {
  //          BitmapSource andSaveTexture = visualizationTool.GenerateAndSaveTexture(surfaceInfo, skuFile, "original");
  //          if (texture1 == null && andSaveTexture != null)
  //            texture1 = andSaveTexture;
  //        }
  //      }
  //      if (texture1 != null)
  //        skuModel.AddSurfaceWithHorizontalTexture(surfaceInfo.Points, surfaceInfo.Triangles, "surface", texture1, 0.75, "surface", (SkuModel.ReportProgressDelegate)null);
  //      else
  //        skuModel.AddSurface((IEnumerable<Point3D>)surfaceInfo.Points, surfaceInfo.Triangles, "surface", "BurlyWood", 0.75, "surface", (SkuModel.ReportProgressDelegate)null);
  //      if (useCase.Compute == ComputeEnum.SurfaceBestFit)
  //      {
  //        IList<Morph.Services.Core.Tools.Node> totalNodes = (IList<Morph.Services.Core.Tools.Node>)null;
  //        List<Flow3D> flowSegments = surfaceInfo.GenerateFlowSegments(design.CellSize, out totalNodes, (IEnumerable<IEnumerable<Point>>)null, (IEnumerable<IEnumerable<Point>>)null, new CancellationToken());
  //        skuModel.AddLinestrings((IEnumerable<IEnumerable<Point3D>>)flowSegments.Select<Flow3D, Point3D[]>((Func<Flow3D, Point3D[]>)(fs => new Point3D[2]
  //      {
  //          fs.Point1,
  //          fs.Point2
  //      })), "surface flows", "brown", 0.75, false, "");
  //      }
  //      BitmapSource texture2 = (BitmapSource)null;
  //      if (useCase.DesignVisualizationTools != null)
  //      {
  //        foreach (VizTool visualizationTool in useCase.DesignVisualizationTools)
  //        {
  //          BitmapSource andSaveTexture = visualizationTool.GenerateAndSaveTexture(design.Surface, skuFile, nameof(design));
  //          if (texture2 == null && andSaveTexture != null)
  //            texture2 = andSaveTexture;
  //        }
  //      }
  //      if (texture2 != null)
  //        skuModel.AddSurfaceWithHorizontalTexture(design.Surface.Points, design.Surface.Triangles, nameof(design), texture2, 0.75, nameof(design), (SkuModel.ReportProgressDelegate)null);
  //      else
  //        skuModel.AddSurface((IEnumerable<Point3D>)design.Surface.Points, design.Surface.Triangles, nameof(design), "Green", 0.75, nameof(design), (SkuModel.ReportProgressDelegate)null);
  //      if (useCase.Compute == ComputeEnum.SurfaceBestFit)
  //      {
  //        IList<Morph.Services.Core.Tools.Node> totalNodes = (IList<Morph.Services.Core.Tools.Node>)null;
  //        List<Flow3D> flowSegments = design.Surface.GenerateFlowSegments(design.CellSize, out totalNodes, (IEnumerable<IEnumerable<Point>>)null, (IEnumerable<IEnumerable<Point>>)null, new CancellationToken());
  //        skuModel.AddLinestrings((IEnumerable<IEnumerable<Point3D>>)flowSegments.Select<Flow3D, Point3D[]>((Func<Flow3D, Point3D[]>)(fs => new Point3D[2]
  //      {
  //          fs.Point1,
  //          fs.Point2
  //      })), "design flows", "DarkGreen", 0.75, false, "");
  //      }
  //      string fullPath = Path.GetFullPath(Path.Combine("Sketchup", "AltitudeTexture.png"));
  //      skuModel.AddSurfaceWithVerticalTexture(design.CutFillSurface.Points, design.CutFillSurface.Triangles, true, "cutfill", fullPath, 0.75, nameof(design), (SkuModel.ReportProgressDelegate)null);
  //      if (useCase.TargetDitches != null && useCase.TargetDitches.Any<Linestring>())
  //      {
  //        int num = 1;
  //        foreach (Linestring targetDitch in useCase.TargetDitches)
  //        {
  //          if (targetDitch.Points.Any<Point>())
  //            skuModel.AddLinestring(targetDitch.Points.Select<Point, Point3D>((Func<Point, Point3D>)(p => new Point3D(p.X, p.Y, 0.0))), string.Format("ditch-{0}", (object)num++), "Lime", 1.0, true, "");
  //        }
  //      }
  //      if (useCase.Pipeline != null && useCase.Pipeline.Points.Any<Point>())
  //        skuModel.AddLinestring(useCase.Pipeline.Points.Select<Point, Point3D>((Func<Point, Point3D>)(p => new Point3D(p.X, p.Y, 0.0))), "pipeline", "Lime", 1.0, true, "");
  //      if (design.Rows.Any<Linestring3D>())
  //        skuModel.AddLinestrings((IEnumerable<IEnumerable<Point3D>>)design.Rows.Select<Linestring3D, List<Point3D>>((Func<Linestring3D, List<Point3D>>)(ls => ls.Points)), "Rows", "Green", 1.0, false, "Rows");
  //      if (design.Columns.Any<Linestring3D>())
  //        skuModel.AddLinestrings((IEnumerable<IEnumerable<Point3D>>)design.Columns.Select<Linestring3D, List<Point3D>>((Func<Linestring3D, List<Point3D>>)(ls => ls.Points)), "Columns", "Green", 1.0, false, "Columns");
  //      foreach (Plane plane in (IEnumerable<Plane>)design.Planes)
  //        skuModel.AddPlane((IEnumerable<Point3D>)plane.Boundary.Points, plane.Tag, "Red", 0.75, "");
  //      skuModel.ZoomToExtents();
  //      skuModel.Save(skuFile, ModelVersion.SU2015);
  //    }
  //  }

  }
}
