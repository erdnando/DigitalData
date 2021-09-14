// Decompiled with JetBrains decompiler
// Type: DigitalData.DDoc.Common.Entities.DdocSearchFilter
// Assembly: DigitalData.DDoc.Common, Version=4.210.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 37D7F3E1-2CA5-48B0-AB22-FE81D67009C0
// Assembly location: C:\Users\herna\Downloads\ss\BradesCard_DEV\bin\DigitalData.DDoc.Common.dll

using DigitalData.Common.Entities;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace DigitalData.DDoc.Common.Entities
{
  [DataContract]
  public class DdocSearchFilter
  {
    [DataMember]
    [XmlAttribute("comparison")]
    public Comparison Comparison { get; set; }

    [DataMember]
    [XmlAttribute("fieldId")]
    public int FieldId { get; set; }

    [DataMember]
    [XmlAttribute("type")]
    public FieldType Type { get; set; }

    [DataMember]
    [XmlElement("value")]
    public string Value { get; set; }

    [DataMember]
    [XmlElement("value2")]
    public string Value2 { get; set; }
  }
}
