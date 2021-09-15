// Decompiled with JetBrains decompiler
// Type: DigitalData.DDoc.Common.Api.IDdocStorage
// Assembly: DigitalData.DDoc.Common, Version=4.210.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 37D7F3E1-2CA5-48B0-AB22-FE81D67009C0
// Assembly location: C:\Users\herna\Downloads\ss\BradesCard_DEV\bin\DigitalData.DDoc.Common.dll

using System.IO;
using System.Threading.Tasks;

namespace DigitalData.Open.Common.Api
{
  public interface IDdocStorage
  {
    string Name { get; }

    Task SaveBytes(string documentId, string fileId, string fileExt, byte[] byteSource);

    Task SaveFile(string documentId, string fileId, string fileExt, string filePath);

    Task<MemoryStream> GetFileStream(string fileId, string fileExt);

    Task<byte[]> GetFileBytes(string fileId, string fileExt);

    Task DeleteFile(string fileId, string type);
  }
}
