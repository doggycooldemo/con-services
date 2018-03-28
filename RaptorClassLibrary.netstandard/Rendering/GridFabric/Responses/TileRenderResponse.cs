﻿using System;
using System.Collections.Generic;
using System.Text;
using VSS.TRex.Rendering.Abstractions;
using VSS.VisionLink.Raptor.GridFabric.Requests.Interfaces;

namespace VSS.VisionLink.Raptor.Rendering.GridFabric.Responses
{
    /// <summary>
    /// Contains the response bitmap for a tile request. Supports compositing of another bitmap with this one
    /// </summary>
    public class TileRenderResponse : IAggregateWith<TileRenderResponse>
    {
        public IBitmap Bitmap { get; set; } = null;

        public TileRenderResponse AggregateWith(TileRenderResponse other)
        {
            // Composite the bitmap held in this response with the bitmap held in 'other'

            throw new NotImplementedException("Bitmap compositing not implemented");
        }
    }
}
