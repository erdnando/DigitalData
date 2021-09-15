
using System;
using System.Runtime.Serialization;

namespace DigitalData.Open.Common.Entities
{
  [DataContract]
  [Flags]
  [Serializable]
  public enum Permissions
  {
    [EnumMember] None = 0,
    [EnumMember] Read = 1,
    [EnumMember] Write = 2,
    [EnumMember] Comment = 4,
    [EnumMember] Export = 8,
    [EnumMember] Print = 16, // 0x00000010
    [EnumMember] Delete = 32, // 0x00000020
    [EnumMember] All = Delete | Print | Export | Comment | Write | Read, // 0x0000003F
  }
}
