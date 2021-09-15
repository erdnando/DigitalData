
using System.Threading.Tasks;

namespace DigitalData.Open.Common.Api
{
  public class NullOcrModule : IDdocOcr
  {
    public Task<byte[]> PerformOcr(byte[] byteSource, string fileExtension) => Task.FromResult<byte[]>(byteSource);
  }
}
