﻿using VSS.TRex.Designs.TTM;

namespace VSS.TRex.Exports.Surfaces.GridDecimator
{
  public struct TriListNode
  {
    public Triangle Tri;
    public bool NotAffected;      // set if on wrong side of static side
  }
}
