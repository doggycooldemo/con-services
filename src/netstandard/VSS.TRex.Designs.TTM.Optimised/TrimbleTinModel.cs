﻿using System;
using System.IO;

namespace VSS.TRex.Designs.TTM.Optimised
{
  public class TrimbleTINModel
  {
    /// <summary>
    /// The full set of vertices that make up this TIN model
    /// </summary>
    public TriVertices Vertices { get; set; } = new TriVertices();

    /// <summary>
    /// The full set of triangles that make up this TIN model
    /// </summary>
    public Triangles Triangles { get; set; } = new Triangles();

    /// <summary>
    /// The set of triangles that comprise the edge of the TIN
    /// </summary>
    public TTMEdges Edges { get; } = new TTMEdges();

    /// <summary>
    /// The set of start points defined for the TIN
    /// </summary>
    public TTMStartPoints StartPoints { get; } = new TTMStartPoints();

    /// <summary>
    /// The header information stored with a TIN surface in a TTM file
    /// </summary>
    public TTMHeader Header = TTMHeader.NewHeader();

    public string ModelName { get; set; }

    public TrimbleTINModel()
    {
    }

    /// <summary>
    /// Reads a TrimbleTINModel using the provided reader
    /// </summary>
    /// <param name="reader"></param>
    public void Read(BinaryReader reader)
    {
      string LoadErrMsg = "";

      try
      {
        LoadErrMsg = "Error reading header";

        Header.Read(reader);

        // Commented out for now...
        //if (FileSignatureToANSIString(FHeader.FileSignature) != kTTMFileIdentifier)
        //{
        //    Raise ETTMReadError.Create('File is not a Trimble TIN Model.');
        //}

        // Check file version
        if (Header.FileMajorVersion != Consts.TTMMajorVersion
            || Header.FileMinorVersion != Consts.TTMMinorVersion)
        {
          throw new Exception("Unable to read this version of Trimble TIN Model file.");
        }

        // ModelName = (String)(InternalNameToANSIString(fHeader.DTMModelInternalName));
        // Not handled for now
        ModelName = "Reading not implemented";

        LoadErrMsg = "Error reading vertices";
        reader.BaseStream.Position = Header.StartOffsetOfVertices;
        Vertices.Read(reader, Header);

        LoadErrMsg = "Error reading triangles";
        reader.BaseStream.Position = Header.StartOffsetOfTriangles;
        Triangles.Read(reader, Header);

        LoadErrMsg = "Error reading edges";
        reader.BaseStream.Position = Header.StartOffsetOfEdgeList;
        Edges.Read(reader, Header);

        LoadErrMsg = "Error reading start points";
        reader.BaseStream.Position = Header.StartOffsetOfStartPoints;
        StartPoints.Read(reader, Header);
      }
      catch (Exception E)
      {
        throw new Exception(LoadErrMsg + ": " + E.Message);
      }
    }

    /// <summary>
    /// Loads a TrimbleTINModel from a stream
    /// </summary>
    /// <param name="stream"></param>
    public void LoadFromStream(Stream stream)
    {
      using (BinaryReader reader = new BinaryReader(stream))
      {
        Read(reader);
      }
    }

    /// <summary>
    /// Loads a TrimbleTINModel from a stream
    /// </summary>
    /// <param name="FileName"></param>
    public void LoadFromFile(string FileName)
    {
      using (MemoryStream ms = new MemoryStream(File.ReadAllBytes(FileName)))
      {
        LoadFromStream(ms);
      }

      // FYI, This method sucks totally - don't use it
      //using (FileStream fs = new FileStream(FileName, FileMode.Open, FileAccess.Read, FileShare.Read, 2048))
      //{
      //    LoadFromStream(fs);
      //}

      if (ModelName.Length == 0)
      {
        ModelName = Path.ChangeExtension(Path.GetFileName(FileName), "");
      }
    }
  }
}
