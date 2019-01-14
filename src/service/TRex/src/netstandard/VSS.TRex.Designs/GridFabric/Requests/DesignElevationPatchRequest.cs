﻿using Apache.Ignite.Core.Compute;
using VSS.TRex.Designs.GridFabric.Arguments;
using VSS.TRex.Designs.GridFabric.ComputeFuncs;
using VSS.TRex.SubGridTrees.Client;
using VSS.TRex.SubGridTrees.Interfaces;

namespace VSS.TRex.Designs.GridFabric.Requests
{
    public class DesignElevationPatchRequest : DesignProfilerRequest<CalculateDesignElevationPatchArgument, ClientHeightLeafSubGrid>
    {
        public override ClientHeightLeafSubGrid Execute(CalculateDesignElevationPatchArgument arg)
        {
            // Construct the function to be used
            IComputeFunc<CalculateDesignElevationPatchArgument, byte[]> func = new CalculateDesignElevationPatchComputeFunc();

            byte[] result = _Compute.Apply(func, arg);

            if (result == null)
            {
                return null;
            }

            ClientHeightLeafSubGrid clientResult = new ClientHeightLeafSubGrid(null, null, SubGridTreeConsts.SubGridTreeLevels, SubGridTreeConsts.DefaultCellSize, SubGridTreeConsts.DefaultIndexOriginOffset);
            clientResult.FromBytes(result);
            return clientResult;
        }
    }
}
