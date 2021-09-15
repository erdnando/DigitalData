
using System.Runtime.Serialization;

namespace DigitalData.Open.Common.Entities
{
  [DataContract]
  public enum TextSearchType
  {
    [EnumMember] AllWords,
    [EnumMember] AnyWords,
    [EnumMember] ExactPhrase,
  }
}
