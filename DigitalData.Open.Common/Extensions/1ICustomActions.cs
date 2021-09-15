
using System.Collections.Generic;

namespace DigitalData.Open.Common.Extensions
{
  public interface ICustomActions
  {
    IEnumerable<DdocCustomAction> CustomActions { get; set; }
  }
}
