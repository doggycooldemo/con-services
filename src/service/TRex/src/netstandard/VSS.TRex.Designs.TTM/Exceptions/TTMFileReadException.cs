﻿using System;
using VSS.TRex.Common.Exceptions;

namespace VSS.TRex.Designs.TTM.Exceptions
{
  /// <summary>
  /// Generic TTM read exception thrown while reading in a TTM file
  /// </summary>
  public class TTMFileReadException : TRexException
  {
    public TTMFileReadException(string message) : base(message)
    {
    }

    public TTMFileReadException(string message, Exception E) : base(message, E)
    {
    }
  }
}
