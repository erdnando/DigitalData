
using System.Runtime.Serialization;

namespace DigitalData.Open.Common.Entities
{
  [DataContract]
  public enum DdocInstanceType
  {
    [EnumMember] Unknown,
    [EnumMember] Web,
    [EnumMember] Mobile,
  }
}
