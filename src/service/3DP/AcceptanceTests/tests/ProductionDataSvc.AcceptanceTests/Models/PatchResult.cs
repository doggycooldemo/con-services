﻿// This file was generated by a tool; you should avoid making direct changes.
// Consider using 'partial classes' to extend these types
// Input: PatchResult.proto

#pragma warning disable CS1591, CS0612, CS3021, IDE1006
namespace ProductionDataSvc.AcceptanceTests.Models
{

  [global::ProtoBuf.ProtoContract()]
  public partial class PatchCellHeightResult : global::ProtoBuf.IExtensible
  {
    private global::ProtoBuf.IExtension __pbn__extensionData;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
        => global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);

    [global::ProtoBuf.ProtoMember(1, IsRequired = true)]
    public uint ElevationOffset { get; set; }

    [global::ProtoBuf.ProtoMember(2)]
    [global::System.ComponentModel.DefaultValue(0)]
    public uint TimeOffset
    {
      get { return __pbn__TimeOffset ?? 0; }
      set { __pbn__TimeOffset = value; }
    }
    public bool ShouldSerializeTimeOffset() => __pbn__TimeOffset != null;
    public void ResetTimeOffset() => __pbn__TimeOffset = null;
    private uint? __pbn__TimeOffset;

  }

  [global::ProtoBuf.ProtoContract()]
  public partial class PatchResult : global::ProtoBuf.IExtensible
  {
    private global::ProtoBuf.IExtension __pbn__extensionData;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
        => global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);

    [global::ProtoBuf.ProtoMember(1, IsRequired = true)]
    public double CellSize { get; set; }

    [global::ProtoBuf.ProtoMember(2, IsRequired = true)]
    public int NumSubgridsInPatch { get; set; }

    [global::ProtoBuf.ProtoMember(3, IsRequired = true)]
    public int TotalNumPatchesRequired { get; set; }

    [global::ProtoBuf.ProtoMember(4)]
    public global::System.Collections.Generic.List<PatchSubgridResultBase> Subgrids { get; } = new global::System.Collections.Generic.List<PatchSubgridResultBase>();

  }

  [global::ProtoBuf.ProtoContract()]
  public partial class PatchSubgridOriginProtobufResult : global::ProtoBuf.IExtensible
  {
    private global::ProtoBuf.IExtension __pbn__extensionData;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
        => global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);

    [global::ProtoBuf.ProtoMember(1, IsRequired = true)]
    public double SubgridOriginX { get; set; }

    [global::ProtoBuf.ProtoMember(2, IsRequired = true)]
    public double SubgridOriginY { get; set; }

    [global::ProtoBuf.ProtoMember(3)]
    public global::System.Collections.Generic.List<PatchCellHeightResult> Cells { get; } = new global::System.Collections.Generic.List<PatchCellHeightResult>();

    [global::ProtoBuf.ProtoMember(4, IsRequired = true)]
    public uint TimeOrigin { get; set; }

  }

  [global::ProtoBuf.ProtoContract()]
  public partial class PatchSubgridResultBase : global::ProtoBuf.IExtensible
  {
    private global::ProtoBuf.IExtension __pbn__extensionData;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
        => global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);

    [global::ProtoBuf.ProtoMember(1, IsRequired = true)]
    public float ElevationOrigin { get; set; }

    [global::ProtoBuf.ProtoMember(10)]
    public PatchSubgridOriginProtobufResult PatchSubgridOriginProtobufResult { get; set; }

  }

}

#pragma warning restore CS1591, CS0612, CS3021, IDE1006
