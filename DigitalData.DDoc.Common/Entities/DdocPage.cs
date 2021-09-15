// Decompiled with JetBrains decompiler
// Type: DigitalData.DDoc.Common.Entities.DdocPage
// Assembly: DigitalData.DDoc.Common, Version=4.210.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 37D7F3E1-2CA5-48B0-AB22-FE81D67009C0
// Assembly location: C:\Users\herna\Downloads\ss\BradesCard_DEV\bin\DigitalData.DDoc.Common.dll

using System.Runtime.Serialization;

namespace DigitalData.Open.Common.Entities
{
  [DataContract]
  public class DdocPage : DdocEntity
  {
    [DataMember]
    public bool Indexed { get; set; }

    [DataMember]
    public string DocumentId { get; set; }

    [DataMember]
    public int ImageCount { get; set; }

    [DataMember]
    public int? Path { get; set; }

    [DataMember]
    public int Sequence { get; set; }

    [DataMember]
    public string Type { get; set; }
  }
}
