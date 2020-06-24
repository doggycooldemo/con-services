﻿using System;
using Trimble.CsdManagementWrapper;

namespace CoreX.Wrapper.Types
{
  public class CSharpFileListCallback : CppFileListCallback
  {
    public CSharpFileListCallback()
      : base()
    { }

    public override bool FileListCallback(string arg0, ushort arg1, csmGeodataFileType arg2)
    {
      return true;
    }
  }

  public class CSharpEmbeddedDataCallback : CppEmbeddedDataCallback
  {
    public CSharpEmbeddedDataCallback()
      : base()
    { }

    public override CSMEmbeddedDataContainer EmbeddedDataCallback(string arg0, uint arg1, csmGeodataFileType arg2)
    {
      return new CSMEmbeddedDataContainer();
    }
  }

  public static class Utils
  {
    public static CSharpEmbeddedDataCallback EmbeddedDataCallback = new CSharpEmbeddedDataCallback();
    public static CSharpFileListCallback FileListCallBack = new CSharpFileListCallback();
    public const double MISSING_VALUE = -9.99e27;
    public const double MISSING_LIMIT = -9.99e26;
    public const double PI = 3.14159265358979323846;

    public static sbyte[] IntPtrToSByte(IntPtr value, int length)
    {
      var bytes = new byte[length];
      System.Runtime.InteropServices.Marshal.Copy(value, bytes, 0, length);

      var sbytes = new sbyte[bytes.Length];
      for (var i = 0; i < bytes.Length; i++)
      {
        sbytes[i] = (sbyte)bytes[i];
      }

      return sbytes;
    }
  }
}
