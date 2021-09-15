
using System;
using System.IO;
using System.Threading.Tasks;

namespace DigitalData.Open.Common.Api
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
