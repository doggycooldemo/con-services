﻿namespace VSS.TRex.Servers
{
  /// <summary>
  /// Defines names of various role that servers can occupy in the grid
  /// </summary>
  public static class ServerRoles
  {
    /// <summary>
    /// The name of the attribute added to a node attributes to record its role
    /// </summary>
    public const string ROLE_ATTRIBUTE_NAME = "Role";

    /// <summary>
    /// The 'PSNode' role, meaning the server is a part of subgrid clustered processing engine
    /// </summary>
    public const string PSNODE = "PSNode";

    /// <summary>
    /// The 'ASNode', application service, role, meaning the server is a part of subgrid clustered processing engine
    /// </summary>
    public const string ASNODE = "ASNode";

    /// <summary>
    /// The 'ASNode' profiling role, meaning the server supports profiling operations
    /// </summary>
    public const string ASNODE_PROFILER = "ASNode-Profiler";

    /// <summary>
    /// The generaic 'ASNode', application service, client role
    /// </summary>
    public const string ASNODE_CLIENT = "ASNodeClient";

    /// <summary>
    /// A server responsible for processing TAG files into the production data models
    /// </summary>
    public const string TAG_PROCESSING_NODE = "TagProc";

    /// <summary>
    /// A server responsible for processing TAG files into the production data models
    /// </summary>
    public const string TAG_PROCESSING_NODE_CLIENT = "TagProcClient";

    /// <summary>
    /// A server responsible for rendering tiles from production data
    /// </summary>
    public const string TILE_RENDERING_NODE = "TileRendering";

    /// <summary>
    /// A server responsible for producing patches of subgrids for Patch requests
    /// </summary>
    public const string PATCH_REQUEST_ROLE = "Patches";

    /// <summary>
    /// A server responsible for computing various analytics, such as cut fill statistics, from production data
    /// </summary>
    public const string ANALYTICS_NODE = "Analytics";

    /// <summary>
    /// A server responsible for reporting various analytics, such as cut fill statistics, from production data
    /// </summary>
    public const string ANALYTICS_NODE_CLIENT = "AnalyticsClient";

    /// <summary>
    /// A server responsible for producing elevation subgrid information from design and surveyed surface topology models (TTMs)
    /// </summary>
    public const string DESIGN_PROFILER = "DesignProfiler";
  }
}
