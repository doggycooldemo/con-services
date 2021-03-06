﻿using System;
using System.IO;
using System.Text;
using VSS.TRex.Designs.TTM.Optimised.Exceptions;

namespace VSS.TRex.Designs.TTM.Optimised
{
  public class TrimbleTINModel
  {
    /// <summary>
    /// The full set of vertices that make up this TIN model
    /// </summary>
    public TriVertices Vertices { get; } = new TriVertices();

    /// <summary>
    /// The full set of triangles that make up this TIN model
    /// </summary>
    public Triangles Triangles { get; } = new Triangles();

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

    /// <summary>
    /// Reads a TrimbleTINModel using the provided reader
    /// </summary>
    public void Read(BinaryReader reader, byte[] bytes)
    {
      var LoadErrMsg = "";

      try
      {
        LoadErrMsg = "Error reading header";

        Header.Read(reader);

        var identifier = System.Text.Encoding.ASCII.GetString(Header.FileSignature);
        if (identifier != Consts.TTM_FILE_IDENTIFIER)
        {
          throw new TTMFileReadException("File is not a Trimble TIN Model.");
        }

        // Check file version
        if (Header.FileMajorVersion != Consts.TTM_MAJOR_VERSION
            || Header.FileMinorVersion != Consts.TTM_MINOR_VERSION)
        {
          throw new TTMFileReadException($"TTM_Optimized.Read(): Unable to read this version {Header.FileMajorVersion}: {Header.FileMinorVersion} of Trimble TIN Model file. Expected version: { Consts.TTM_MAJOR_VERSION}: {Consts.TTM_MINOR_VERSION}");
        }

        ModelName = ASCIIEncoding.ASCII.GetString(Header.DTMModelInternalName).TrimEnd(new[] { '\0' });

        LoadErrMsg = "Error reading vertices";
        reader.BaseStream.Position = Header.StartOffsetOfVertices;
        Vertices.Read(reader, Header);
        //Vertices.Read(bytes, Header.StartOffsetOfVertices, Header);

        LoadErrMsg = "Error reading triangles";
        //reader.BaseStream.Position = Header.StartOffsetOfTriangles;
        //Triangles.Read(reader, Header);
        Triangles.Read(bytes, Header.StartOffsetOfTriangles, Header);

        LoadErrMsg = "Error reading edges";
        reader.BaseStream.Position = Header.StartOffsetOfEdgeList;
        Edges.Read(reader, Header);

        LoadErrMsg = "Error reading start points";
        reader.BaseStream.Position = Header.StartOffsetOfStartPoints;
        StartPoints.Read(reader, Header);
      }
      catch (TTMFileReadException)
      {
        throw; // pass it on
      }
      catch (Exception e)
      {
        throw new TTMFileReadException($"Exception at TTM loading phase {LoadErrMsg}", e);
      }
    }

    /// <summary>
    /// Loads a TrimbleTINModel from a stream
    /// </summary>
    public void LoadFromStream(Stream stream, byte [] bytes)
    {
      using var reader = new BinaryReader(stream);
      Read(reader, bytes);
    }

    /// <summary>
    /// Loads a TrimbleTINModel from a stream
    /// </summary>
    public void LoadFromFile(string fileName)
    {
      var bytes = File.ReadAllBytes(fileName);

      using var ms = new MemoryStream(bytes);
      LoadFromStream(ms, bytes);

      // FYI, This method sucks totally - don't use it
      //using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read, 2048))
      //{
      //    LoadFromStream(fs);
      //}

      if (ModelName.Length == 0)
        ModelName = Path.GetFileNameWithoutExtension(fileName);
    }


    public long SizeInCache()
    {
      return Vertices.SizeOf() +
             Triangles.SizeOf() +
             Edges.SizeOf() +
             StartPoints.SizeOf();
    }
  }
}
