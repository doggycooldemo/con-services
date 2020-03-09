﻿using System.Collections.Generic;
using Apache.Ignite.Core.Cache;

namespace VSS.TRex.Storage.Interfaces
{
  /// <summary>
  /// Defines the subset of Ignite ICache APIs required to support storage proxy semantics in TRex
  /// </summary>
  /// <typeparam name="TK"></typeparam>
  /// <typeparam name="TV"></typeparam>
  public interface IStorageProxyCache<TK, TV> : IStorageProxyCacheCommit
  {
    TV Get(TK key);

    bool Remove(TK key);

    void RemoveAll(IEnumerable<TK> key);

    void Put(TK key, TV value);

    void PutAll(IEnumerable<KeyValuePair<TK, TV>> values);

    ICacheLock Lock(TK key);
  }
}
