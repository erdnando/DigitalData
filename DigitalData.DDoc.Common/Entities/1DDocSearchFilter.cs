// Decompiled with JetBrains decompiler

using DigitalData.Common.Entities;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace DigitalData.Open.Common.Entities
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
