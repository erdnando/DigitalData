
using DigitalData.Common.Entities;

namespace DigitalData.Open.Common.Entities.Helpers
{
  public class DdocException : BLLException
  {
    public DdocException(string message)
      : base(message)
    {
    }
  }
}
