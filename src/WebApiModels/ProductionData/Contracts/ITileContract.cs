﻿
using System.Web.Http;
using VSS.Raptor.Service.WebApiModels.ProductionData.Models;

namespace VSS.Raptor.Service.WebApiModels.ProductionData.Contracts
{

  /// <summary>
  /// Data contract representing tiles of rendered overlays from Raptor
  /// </summary>
  public interface ITileContract
  { 
    /// <summary>
    /// Supplies tiles of rendered overlays for a number of different thematic sets of data held in a project such as elevation, compaction, temperature, cut/fill, volumes etc
    /// </summary>
    /// <param name="request">A representation of the tile rendering request.</param>
    /// <returns>An HTTP response containing an error code is there is a failure, or a PNG image if the request suceeds.</returns>
      dynamic Post([FromBody] TileRequest request);

  }
}
