﻿using System;
using Microsoft.Extensions.Configuration;

namespace VSS.Common.Abstractions.Configuration
{
  public interface IConfigurationStore
  {
    string GetValueString(string v);
    string GetValueString(string v, string defaultValue);
    bool? GetValueBool(string v);
    bool GetValueBool(string v, bool defaultValue);
    int GetValueInt(string v);
    int GetValueInt(string v, int defaultValue);
    uint GetValueUint(string v);
    uint GetValueUint(string v, uint defaultValue);
    long GetValueLong(string v);
    long GetValueLong(string v, long defaultValue);
    ulong GetValueUlong(string v);
    ulong GetValueUlong(string v, ulong defaultValue);
    double GetValueDouble(string v);
    double GetValueDouble(string v, double defaultValue);
    TimeSpan? GetValueTimeSpan(string v);
    TimeSpan GetValueTimeSpan(string v, TimeSpan defaultValue);
    DateTime? GetValueDateTime(string key);
    DateTime GetValueDateTime(string key, DateTime defaultValue);
    Guid GetValueGuid(string v);
    Guid GetValueGuid(string v, Guid defaultValue);
    string GetConnectionString(string connectionType, string databaseNameKey);
    string GetConnectionString(string connectionType);
    IConfigurationSection GetSection(string key);
    IConfigurationSection GetLoggingConfig();

    bool UseKubernetes { get; }
    string KubernetesConfigMapName { get; }
    string KubernetesNamespace { get; }
  }
}
