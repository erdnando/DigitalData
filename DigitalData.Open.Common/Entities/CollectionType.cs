
using System.Runtime.Serialization;

namespace DigitalData.Open.Common.Entities
{
  [DataContract]
  public enum CollectionType
  {
    [EnumMember] R,
    [EnumMember] C,
    [EnumMember] F,
    [EnumMember] D,
  }
}
