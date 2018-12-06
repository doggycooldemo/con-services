﻿using System.Collections.Generic;
using Apache.Ignite.Core.Binary;
using VSS.TRex.Common;
using VSS.TRex.GridFabric.Interfaces;
using VSS.TRex.Profiling.Interfaces;

namespace VSS.TRex.Profiling.GridFabric.Responses
{
  /// <summary>
  /// Contains the set of profile cells generated by a profile analysis executor
  /// </summary>
  public class ProfileRequestResponse : SubGridsPipelinedReponseBase, IAggregateWith<ProfileRequestResponse>
  {
    public List<IProfileCell> ProfileCells { get; set; } = new List<IProfileCell>();

    /// <summary>
    /// Aggregates an other response with this response
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public ProfileRequestResponse AggregateWith(ProfileRequestResponse other)
    {
      ProfileCells.AddRange(other.ProfileCells);

      return this;
    }

    /// <summary>
    /// Serializes content to the writer
    /// </summary>
    /// <param name="writer"></param>
    public override void ToBinary(IBinaryRawWriter writer)
    {
      base.ToBinary(writer);

      writer.WriteBoolean(ProfileCells != null);
      if (ProfileCells != null)
      {
        writer.WriteInt(ProfileCells.Count);

        foreach (var profileCell in ProfileCells)
          profileCell?.ToBinary(writer);
      }
    }

    /// <summary>
    /// Serializes content from the writer
    /// </summary>
    /// <param name="reader"></param>
    public override void FromBinary(IBinaryRawReader reader)
    {
      base.FromBinary(reader);

      if (ProfileCells == null)
        ProfileCells = new List<IProfileCell>();

      if (reader.ReadBoolean())
      {
        var numberOfProfileCells = reader.ReadInt();

        if (numberOfProfileCells > 0)
        {
          for (var i = 1; i <= numberOfProfileCells; i++)
          {
            var profileCell = new ProfileCell();

            profileCell.FromBinary(reader);
            ProfileCells.Add(profileCell);
          }
        }
      }
    }
  }
}

