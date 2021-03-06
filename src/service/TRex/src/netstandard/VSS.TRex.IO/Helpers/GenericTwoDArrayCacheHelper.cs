﻿namespace VSS.TRex.IO.Helpers
{
    public static class GenericTwoDArrayCacheHelper<T>
    {
      private static readonly object lockObj = new object();

      public const int DEFAULT_TWOD_ARRAY_CACHE_SIZE = 10000;
      public const int DEFAULT_TWOD_DIMENSION_SIZE = 32;

      private static IGenericTwoDArrayCache<T> _caches;
      public static IGenericTwoDArrayCache<T> Caches()
      {
        if (_caches == null)
        {
          lock (lockObj)
          {
            if (_caches == null)
            {
              _caches = new GenericTwoDArrayCache<T>(DEFAULT_TWOD_DIMENSION_SIZE, DEFAULT_TWOD_DIMENSION_SIZE, DEFAULT_TWOD_ARRAY_CACHE_SIZE);

              GenericTwoDArrayCacheRegister.Add(_caches);
            }
          }
        }

        return _caches;
      }

      public static void Clear()
      {
        _caches = null;
      }
    }
}
