
using System.Runtime.Serialization;

namespace DigitalData.Open.Common.Entities
{
  [DataContract]
  public enum GroupType
  {
    [EnumMember] Unknown,
    [EnumMember] UserGroup,
    [EnumMember] SecurityGroup,
  }
}
