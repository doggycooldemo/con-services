﻿using Apache.Ignite.Core;
using Apache.Ignite.Core.Resource;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using VSS.VisionLink.Raptor.GridFabric.Grids;

namespace VSS.VisionLink.Raptor.GridFabric.Requests
{
    /// <summary>
    /// The base class for requests. This provides common aspects such as the injected Ignite instance
    /// </summary>
    public class BaseRaptorRequest : BaseRaptorIgniteClass
    {
        /// <summary>
        /// Default no-arg constructor
        /// </summary>
        public BaseRaptorRequest() : base()
        {
        }

        /// <summary>
        /// Constructor accepting a role for the request that may identify a cluster group of nodes in the grid
        /// </summary>
        /// <param name="Role"></param>
        public BaseRaptorRequest(string Role) : base(Role)
        {
        }
    }
}
