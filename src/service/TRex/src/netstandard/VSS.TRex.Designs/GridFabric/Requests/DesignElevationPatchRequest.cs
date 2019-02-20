﻿using VSS.TRex.Designs.GridFabric.Arguments;
using VSS.TRex.Designs.GridFabric.ComputeFuncs;
using VSS.TRex.Designs.GridFabric.Responses;

namespace VSS.TRex.Designs.GridFabric.Requests
{
    public class DesignElevationPatchRequest : DesignProfilerRequest<CalculateDesignElevationPatchArgument, CalculateDesignElevationPatchResponse>
    {
        public override CalculateDesignElevationPatchResponse Execute(CalculateDesignElevationPatchArgument arg)
        {
            // Construct the function to be used
            var func = new CalculateDesignElevationPatchComputeFunc();

            return _Compute.Apply(func, arg);
        }
    }
}
