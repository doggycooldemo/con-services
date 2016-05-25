﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace LandfillService.Common.Models
{
  #region WEb API Models
  /// <summary>
    /// Project representation
    /// </summary>
    public class Project
    {
        public uint id { get; set; }
        public string name { get; set; }
        public string timeZoneName { get; set; }      // project time zone name (NodaTime)
        public int? daysToSubscriptionExpiry { get; set; }
        public string projectUid { get; set; }
        public string legacyTimeZoneName { get; set; }
    }

    /// <summary>
    /// Geofence representation
    /// </summary>
    public class Geofence
    {
      public Guid uid { get; set; }
      public string name { get; set; }
      public int type { get; set; }
    }

    /// <summary>
    /// Date and geofence for entries with no volume
    /// </summary>
    public class DateEntry
    {
      public DateTime date { get; set; }          // date of the entry; always in the project time zone
      public string geofenceUid { get; set; }

      /// <summary>
      /// ToString override
      /// </summary>
      /// <returns>A string representation of date entry</returns>
      public override string ToString()
      {
        return String.Format("date:{0}, geofence UID:{1}", date, geofenceUid);
      }
    }
    /// <summary>
    /// Weight entry submitted by the user
    /// </summary>
    public class WeightEntry
    {
        public DateTime date { get; set; }          // date of the entry; always in the project time zone
        public double weight { get; set; }

        /// <summary>
        /// ToString override
        /// </summary>
        /// <returns>A string representation of weight entry</returns>
        public override string ToString()
        {
            return String.Format("date:{0}, weight:{1}", date, weight);
        }
    }

    /// <summary>
    /// Data entry for a given date - part of project data sent to the client 
    /// </summary>
    public class DayEntry
    {
        public DateTime date { get; set; }
        public bool entryPresent { get; set; }    // true if the entry has at least the weight value
        public double weight { get; set; }
        public double volume { get; set; }    
    }

    /// <summary>
    /// Weight entry for a geofence
    /// </summary>
    public class GeofenceWeight
    {
      public Guid geofenceUid { get; set; }
      public double weight { get; set; }

      /// <summary>
      /// ToString override
      /// </summary>
      /// <returns>A string representation of weight entry</returns>
      public override string ToString()
      {
        return String.Format("geofenceUid:{0}, weight:{1}", geofenceUid, weight);
      }
    }

    /// <summary>
    /// Weight entry submitted by the user for a geofence
    /// </summary>
    public class GeofenceWeightEntry
    {
      public DateTime date { get; set; }
      public bool entryPresent { get; set; }    // true if any site has a weight present
      public IEnumerable<GeofenceWeight> geofenceWeights { get; set; }
    }

    /// <summary>
    /// Encapsulates weight data sent to the client  
    /// </summary>
    public class WeightData
    {
      public IEnumerable<GeofenceWeightEntry> entries { get; set; }
      public bool retrievingVolumes { get; set; }          // is the service currently retrieving volumes for this project?
      public Project project { get; set; }   
    }

    /// <summary>
    /// Encapsulates project data sent to the client 
    /// </summary>
    public class ProjectData
    {
        public IEnumerable<DayEntry> entries { get; set; }
        public bool retrievingVolumes { get; set; }          // is the service currently retrieving volumes for this project?
        public Project project { get; set; }   
    }

    /// <summary>
    /// An entry for CCA for a machine 
    /// </summary>
    public class CCAEntry
    {
      public DateTime date { get; set; }
      public double ccaPercent { get; set; }
    }

    /// <summary>
    /// CCA% representation for a machine
    /// </summary>
    public class CCAData
    {
      public string machineName { get; set; }
      public IEnumerable<CCAEntry> entries { get; set; }
    }

    /// <summary>
    /// Volume and time sumamry data 
    /// </summary>
    public class VolumeTime
    {
      public double currentWeekVolume { get; set; }
      public double currentMonthVolume { get; set; }
      public double remainingVolume { get; set; }
      public double remainingTime { get; set; }
    }

  #endregion

    #region Raptor API Models
    /// <summary>
    /// WGS point for volume summary requests sent to the Raptor API; see Raptor API documentation for details
    /// </summary>
    public class WGSPoint
    {
      public double Lat;
      public double Lon;

      /// <summary>
      /// ToString override
      /// </summary>
      /// <returns>A string representation of polygon filter params</returns>
      public override string ToString()
      {
        return String.Format("(Lat:{0}, Lon:{1})", Lat, Lon);
      }
    }

    /// <summary>
    /// Filter for volume summary requests sent to the Raptor API; see Raptor API documentation for details
    /// </summary>
    public class VolumeFilter
    {
        public DateTime startUTC;
        public DateTime endUTC;
        public bool returnEarliest;
        public List<WGSPoint> polygonLL;

        /// <summary>
        /// ToString override
        /// </summary>
        /// <returns>A string representation of volume filter params</returns>
        public override string ToString()
        {
          var poly = string.Empty;
          if (polygonLL != null)
          {
            foreach (WGSPoint pt in polygonLL)
            {
              poly = string.Format("{0}{1}", poly, pt);
            }
          }
          return string.Format("startUTC:{0}, endUTC:{1}, returnEarliest:{2}, polygonLL:{3}", startUTC, endUTC, returnEarliest, poly);
        }
    }

    /// <summary>
    /// Volume calculation parameters sent to the Raptor API; see Raptor API documentation for details
    /// </summary>
    public class VolumeParams
    {
        public long projectId;
        public int volumeCalcType;
        public VolumeFilter baseFilter;
        public VolumeFilter topFilter;

        /// <summary>
        /// ToString override
        /// </summary>
        /// <returns>A string representation of volume request params</returns>
        public override string ToString()
        {
            return String.Format("projectId:{0}, volumeCalcType:{1}, baseFilter:{2}, topFilter:{3}", projectId, volumeCalcType, baseFilter, topFilter);
        }

    }


    /// <summary>
    /// 3D bounding box - returned in volume summary results from the Raptor API
    /// </summary>
    public class BoundingBox3DGrid
    {
        /// <summary>
        /// Maximum X value, in the cartesian grid coordinate system, expressed in meters
        /// </summary>
        public double maxX { get; set; }

        /// <summary>
        /// Maximum Y value, in the cartesian grid coordinate system, expressed in meters
        /// </summary>
        public double maxY { get; set; }

        /// <summary>
        /// Maximum Z value, in the cartesian grid coordinate system, expressed in meters
        /// </summary>
        public double maxZ { get; set; }

        /// <summary>
        /// Minimum X value, in the cartesian grid coordinate system, expressed in meters
        /// </summary>
        public double minX { get; set; }

        /// <summary>
        /// Minimum Y value, in the cartesian grid coordinate system, expressed in meters
        /// </summary>
        public double minY { get; set; }

        /// <summary>
        /// Minimum Z value, in the cartesian grid coordinate system, expressed in meters
        /// </summary>
        public double minZ { get; set; }
    }


    /// <summary>
    /// Volume summary entry returned from the Raptor API
    /// </summary>
    public class SummaryVolumesResult 
    {
        /// <summary>
        /// Zone boundaries
        /// </summary>
        public BoundingBox3DGrid BoundingExtents { get; set; }
        /// <summary>
        /// Cut volume in m3
        /// </summary>
        public double Cut { get; set; }
        /// <summary>
        /// Fill volume in m3
        /// </summary>
        public double Fill { get; set; }
        /// <summary>
        /// Cut area in m2
        /// </summary>
        public double CutArea { get; set; }
        /// <summary>
        /// Fill area in m2
        /// </summary>
        public double FillArea { get; set; }
        /// <summary>
        /// Total coverage area (cut + fill + no change) in m2. 
        /// </summary>
        public double TotalCoverageArea { get; set; }

        /// <summary>
        /// ToString override
        /// </summary>
        /// <returns>A string representation of volume summary results</returns>
        public override string ToString()
        {
            return String.Format("cut:{0}, fill:{1}", Cut, Fill);
        }
    }

    /// <summary>
    /// Machine details sent to/received from the Raptor API; see Raptor API documentation for details
    /// </summary>
    public class MachineDetails
    {
      public long assetId;
      public string machineName;
      public bool isJohnDoe;

      /// <summary>
      /// ToString override
      /// </summary>
      /// <returns>A string representation of machine details params</returns>
      public override string ToString()
      {
        return string.Format("({0},{1},{2})", assetId, machineName, isJohnDoe);
      }
    }

    /// <summary>
    /// Filter for CCA summary requests sent to the Raptor API; see Raptor API documentation for details
    /// </summary>
    public class CCAFilter
    {
      public DateTime startUTC;
      public DateTime endUTC;
      public List<WGSPoint> polygonLL;
      public List<MachineDetails> contributingMachines;
      public int? layerNumber;

      /// <summary>
      /// ToString override
      /// </summary>
      /// <returns>A string representation of CCA filter params</returns>
      public override string ToString()
      {
        var poly = string.Empty;
        if (polygonLL != null)
        {
          foreach (WGSPoint pt in polygonLL)
          {
            poly = string.Format("{0}{1}", poly, pt);
          }
        }
        return string.Format("startUTC:{0}, endUTC:{1}, layerNumber:{2}, contributingMachines:{3}, polygonLL:{4}", 
          startUTC, endUTC, layerNumber, contributingMachines, poly);
      }
    }

    /// <summary>
    /// CCA summary parameters sent to the Raptor API; see Raptor API documentation for details
    /// </summary>
    public class CCASummaryParams
    {
      public long projectId;
      public CCAFilter filter;

      /// <summary>
      /// ToString override
      /// </summary>
      /// <returns>A string representation of volume request params</returns>
      public override string ToString()
      {
        return String.Format("projectId:{0}, filter:{1}", projectId, filter);
      }

    }

    /// <summary>
    /// CCA summary entry returned from the Raptor API
    /// </summary>
    public class CCASummaryResult
    {
      /// <summary>
      /// The percentage of cells that are complete within the target bounds
      /// </summary>
      public double completePercent { get; private set; }

      /// <summary>
      /// The percentage of the cells that are over-complete
      /// </summary>
      public double overCompletePercent { get; private set; }

      /// <summary>
      /// The internal result code of the request. Documented elsewhere.
      /// </summary>
      public short returnCode { get; private set; }

      /// <summary>
      /// The total area covered by non-null cells in the request area
      /// </summary>
      public double totalAreaCoveredSqMeters { get; private set; }

      /// <summary>
      /// The percentage of the cells that are under complete
      /// </summary>
      public double underCompletePercent { get; private set; }

      /// <summary>
      /// ToString override
      /// </summary>
      /// <returns>A string representation of CCA summary results</returns>
      public override string ToString()
      {
        return String.Format("under:{0}, complete:{1}, over:{2}", 
          underCompletePercent, completePercent, overCompletePercent);
      }
    }
    #endregion

  
}