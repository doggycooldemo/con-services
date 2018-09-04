﻿using System;
using Microsoft.Extensions.Logging;
using VSS.TRex.Filters.Interfaces;
using VSS.TRex.Geometry;
using VSS.TRex.Types;

namespace VSS.TRex.Filters
{
  public static class FilterUtilities
  {
    private static readonly ILogger Log = Logging.Logger.CreateLogger(nameof(FilterUtilities));

    /*
    private IExistenceMaps existenceMaps = null;
    private IExistenceMaps GetExistenceMaps() => existenceMaps ?? (existenceMaps = DIContext.Obtain<IExistenceMaps>());
    */

    /// <summary>
    /// Prepare a filter for use by performing any necessary coordinate conversions and requesting any
    /// supplemental information such as alignment design boundary calculations.
    /// </summary>
    /// <param name="Filter"></param>
    /// <param name="DataModelID"></param>
    /// <returns></returns>
    public static RequestErrorStatus PrepareFilterForUse(ICombinedFilter Filter, Guid DataModelID)
    {
      // XYZ[] NEECoords = null;
      XYZ[] LLHCoords; //: TCSConversionCoordinates;
      // Fence DesignBoundary = null;
      RequestErrorStatus Result = RequestErrorStatus.OK;

      //CoordConversionResult: TCoordServiceErrorStatus;
      //RequestResult: TDesignProfilerRequestResult;

      try
      {
        if (Filter == null)
        {
          return Result;
        }

        if (Filter.SpatialFilter != null)
        {
          if (!Filter.SpatialFilter.CoordsAreGrid)
          {
            // If the filter has a spatial or positional context, then convert the LLH values in the
            // spatial context into the NEE values consistent with the data model.
            if (Filter.SpatialFilter.HasSpatialOrPostionalFilters)
            {
              LLHCoords = new XYZ[Filter.SpatialFilter.Fence.NumVertices];

              // Note: Lat/Lons in filter fence boundaries are supplied to us in decimal degrees, not radians
              for (int FencePointIdx = 0; FencePointIdx < Filter.SpatialFilter.Fence.NumVertices; FencePointIdx++)
              {
                LLHCoords[FencePointIdx] = new XYZ(Filter.SpatialFilter.Fence[FencePointIdx].X * (Math.PI / 180), Filter.SpatialFilter.Fence[FencePointIdx].Y * (Math.PI / 180));
              }

              /* TODO - not yet supported
              CoordConversionResult = ASNodeImplInstance.CoordService.RequestCoordinateConversion(-1, DataModelID, cctLLHtoNEE, LLHCoords, EmptyStr, NEECoords);
              if (CoordConversionResult != csOK)
              {
                  return RequestErrorStatus.FailedToConvertClientWGSCoords;
              }

              for (int FencePointIdx = 0; FencePointIdx < Filter.SpatialFilter.Fence.NumVertices; FencePointIdx++)
              {
                  Filter.SpatialFilter.Fence[FencePointIdx].X = NEECoords[FencePointIdx].X;
                  Filter.SpatialFilter.Fence[FencePointIdx].Y = NEECoords[FencePointIdx].Y;
              }

              // Ensure that the bounding rectangle for the filter fence correctly encloses the newly calculated grid coordinates
              Filter.SpatialFilter.Fence.UpdateExtents();
              */

              throw new NotImplementedException();
            }

            if (Filter.SpatialFilter.HasSpatialOrPostionalFilters)
            {
              // Note: Lat/Lons in filter fence boundaries are supplied to us in decimal degrees, not radians
              LLHCoords = new[] {new XYZ(Filter.SpatialFilter.PositionX * (Math.PI / 180), Filter.SpatialFilter.PositionY * (Math.PI / 180))};

              /* TODO - not yet supported
              CoordConversionResult = ASNodeImplInstance.CoordService.RequestCoordinateConversion(-1, aDataModelID, cctLLHtoNEE, LLHCoords, EmptyStr, NEECoords);
              if (CoordConversionResult != csOK)
              {
                  return RequestErrorStatus.FailedToConvertClientWGSCoords;
              }

              Filter.SpatialFilter.PositionX = NEECoords[0].X;
              Filter.SpatialFilter.PositionY = NEECoords[0].Y;
              */

              throw new NotImplementedException();
            }

            Filter.SpatialFilter.CoordsAreGrid = true;
          }

          // Ensure that the bounding rectangle for the filter fence correctly encloses the newly calculated grid coordinates
          Filter.SpatialFilter?.Fence.UpdateExtents();

          // Is there an alignment file to look up
          if (Filter.SpatialFilter.HasAlignmentDesignMask())
          {
            throw new NotImplementedException();
            /* TODO - Not yet supported
            RequestResult = DesignProfilerLayerLoadBalancer.LoadBalancedDesignProfilerService.RequestDesignFilterBoundary
                (Construct_CalculateDesignFilterBoundary_Args(DataModelID,
                                                              Filter.SpatialFilter.ReferenceDesign,
                                                              Filter.SpatialFilter.StartStation, Filter.SpatialFilter.EndStation,
                                                              Filter.SpatialFilter.LeftOffset, Filter.SpatialFilter.RightOffset, dfbrtList),
                                                        DesignBoundary);
            if (RequestResult == dppiOK)
            {
                Filter.SpatialFilter.AlignmentFence.Assign(DesignBoundary);
            }
          }
          else 
          {
              Log.LogError($"PrepareFilterForUse: Failed to get boundary for alignment design ID:{Filter.SpatialFilter.AlignmentMaskDesignUID}");
          }
          */
          }

          // Is there a surface design to look up
          if (Filter.SpatialFilter.HasAlignmentDesignMask())
          {
            /* If the filter needs to retain a reference to the existence map, then do this...
            Filter.SpatialFilter.DesignMaskExistenceMap = GetExistenceMaps().GetSingleExistenceMap(DataModelID, Consts.EXISTANCE_MAP_DESIGN_DESCRIPTOR, Filter.SpatialFilter.SurfaceDesignMaskDesignUid);

            if (Filter.SpatialFilter.DesignMaskExistenceMap == null)
            {
              Log.LogError($"PrepareFilterForUse: Failed to get existence map for surface design ID:{Filter.SpatialFilter.SurfaceDesignMaskDesignUid}");
            }
            */
          }
        }
      }
      catch
      {
        // TODO readd when logging available
        //SIGLogMessage.Publish(Nil, Format('PrepareFilterForUse: Exception ''%s'' raised', [E.Message]), slmcException);
        Result = RequestErrorStatus.Unknown;
      }

      return Result;
    }

    /// <summary>
    /// Prepare a set of filter for use by performing any necessary coordinate conversions and requesting any
    /// supplemental information such as alignment design boundary calculations.
    /// </summary>
    /// <param name="Filters"></param>
    /// <param name="DataModelID"></param>
    /// <returns></returns>
    public static RequestErrorStatus PrepareFiltersForUse(ICombinedFilter[] Filters, Guid DataModelID)
    {
      foreach (ICombinedFilter filter in Filters)
      {
        if (filter == null)
          continue;

        RequestErrorStatus status = PrepareFilterForUse(filter, DataModelID);

        if (status != RequestErrorStatus.OK)
          return status;
      }

      return RequestErrorStatus.OK;
    }

    /// <summary>
    /// Performs filter preparation for a matched pair of filters being used by a request
    /// </summary>
    /// <param name="Filter1"></param>
    /// <param name="Filter2"></param>
    /// <param name="DataModelID"></param>
    /// <returns></returns>
    public static RequestErrorStatus PrepareFilterForUse(ICombinedFilter Filter1, ICombinedFilter Filter2, Guid DataModelID)
    {
      RequestErrorStatus Result = RequestErrorStatus.OK;

      if (Filter1 != null && !Filter1.AttributeFilter.AnyFilterSelections)
        Result = PrepareFilterForUse(Filter1, DataModelID);
 
      if (Result == RequestErrorStatus.OK && Filter2 != null && !Filter2.AttributeFilter.AnyFilterSelections)
        Result = PrepareFilterForUse(Filter2, DataModelID);

      return Result;
    }
  }
}
