
using System.Runtime.Serialization;

namespace DigitalData.Open.Common.Entities
{
  [DataContract]
  public enum FieldType
  {
    [EnumMember] Text,
    [EnumMember] Number,
    [EnumMember] Date,
    [EnumMember] Boolean,
    [EnumMember] Money,
  }
}
