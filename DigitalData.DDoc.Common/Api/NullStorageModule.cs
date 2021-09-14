// Decompiled with JetBrains decompiler
// Type: DigitalData.DDoc.Common.Api.NullStorageModule
// Assembly: DigitalData.DDoc.Common, Version=4.210.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 37D7F3E1-2CA5-48B0-AB22-FE81D67009C0
// Assembly location: C:\Users\herna\Downloads\ss\BradesCard_DEV\bin\DigitalData.DDoc.Common.dll

using System;
using System.IO;
using System.Threading.Tasks;

namespace DigitalData.DDoc.Common.Api
{
  public class NullStorageModule : IDdocStorage
  {
    public string Name { get; } = "None";

    Task IDdocStorage.SaveBytes(
      string documentId,
      string fileId,
      string fileExt,
      byte[] byteSource)
    {
      throw new NotImplementedException();
    }

    Task IDdocStorage.SaveFile(
      string documentId,
      string fileId,
      string fileExt,
      string filePath)
    {
      throw new NotImplementedException();
    }

    Task<MemoryStream> IDdocStorage.GetFileStream(
      string fileId,
      string fileExt)
    {
      throw new NotImplementedException();
    }

    Task<byte[]> IDdocStorage.GetFileBytes(string fileId, string fileExt) => throw new NotImplementedException();

    Task IDdocStorage.DeleteFile(string fileId, string type) => throw new NotImplementedException();
  }
}
