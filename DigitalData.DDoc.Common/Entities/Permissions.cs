// Decompiled with JetBrains decompiler
// Type: DigitalData.DDoc.Common.Entities.Permissions
// Assembly: DigitalData.DDoc.Common, Version=4.210.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 37D7F3E1-2CA5-48B0-AB22-FE81D67009C0
// Assembly location: C:\Users\herna\Downloads\ss\BradesCard_DEV\bin\DigitalData.DDoc.Common.dll

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
