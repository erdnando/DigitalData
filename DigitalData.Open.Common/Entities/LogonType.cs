
using System.Runtime.Serialization;

namespace DigitalData.Open.Common.Entities
{
  [DataContract]
  public enum LogonType
  {
    [EnumMember] None,
    [EnumMember] Native,
    [EnumMember] Domain,
  }
}
