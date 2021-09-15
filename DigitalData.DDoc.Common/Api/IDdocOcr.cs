
using System.Threading.Tasks;

namespace DigitalData.Open.Common.Api
{
  public interface IDdocOcr
  {
    Task<byte[]> PerformOcr(byte[] byteSource, string fileExtension);
  }
}
