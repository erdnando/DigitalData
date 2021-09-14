// Decompiled with JetBrains decompiler
// Type: DigitalData.DDoc.Common.Entities.CellType
// Assembly: DigitalData.DDoc.Common, Version=4.210.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 37D7F3E1-2CA5-48B0-AB22-FE81D67009C0
// Assembly location: C:\Users\herna\Downloads\ss\BradesCard_DEV\bin\DigitalData.DDoc.Common.dll

using System.Runtime.Serialization;

namespace DigitalData.DDoc.Common.Entities
{
  [DataContract]
  public enum CellType
  {
    [EnumMember] Normal,
    [EnumMember] Checkbox,
    [EnumMember] Radio,
    [EnumMember] Textbox,
    [EnumMember] Combo,
    [EnumMember] Button,
    [EnumMember] Image,
    [EnumMember] Action,
    [EnumMember] Textlink,
    [EnumMember] Iconlink,
    [EnumMember] Thumbnail,
    [EnumMember] Thumbnaillink,
  }
}
