
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
