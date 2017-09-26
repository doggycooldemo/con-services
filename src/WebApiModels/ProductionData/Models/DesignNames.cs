﻿
namespace VSS.Productivity3D.WebApi.Models.ProductionData.Models
{
  public class DesignNames 
    {
      /// <summary>
      /// The name of the design.
      /// </summary>
      /// <value>
      /// The name of the design.
      /// </value>
        public string designName { get; private set; }
        /// <summary>
        ///The Raptor design identifier.
        /// </summary>
        /// <value>
        /// The design identifier.
        /// </value>
        public long designId { get; private set; }

        public static DesignNames CreateDesignNames(string name, long id)
        {
            return new DesignNames {designId = id, designName = name};
        }
    }
}