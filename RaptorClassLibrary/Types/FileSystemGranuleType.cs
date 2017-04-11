﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VSS.VisionLink.Raptor.Types
{
    /// <summary>
    /// Types of file system granules supported in a Raptor FS file
    /// </summary>
    public enum FileSystemGranuleType
    {
        Unknown,
        Header,
        Integrity,
        RootInfo,
        FreeSpaceList,
        FileList,
        Subgrid,
        SubgridSegment,
        SubgridSpatialIndex,
        EventList,
        SubgridExistenceMap,
        DesignNames,
        SiteModelInfo,
        SiteModelMachines,
        CoordinateSystemDefinition,
        GroundSurfaces,
        ReplicatedVerb,
        DirectoryShard,
        SubgridShard
    }
}
