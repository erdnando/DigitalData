
using System.Runtime.Serialization;

namespace DigitalData.Open.Common.Entities
{
  [DataContract]
  public enum PageType
  {
    [EnumMember] Unknown,
    [EnumMember] Tif,
    [EnumMember] Pdf,
    [EnumMember] Xml,
  }
}
