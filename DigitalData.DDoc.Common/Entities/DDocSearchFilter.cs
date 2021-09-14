// Decompiled with JetBrains decompiler
// Type: DigitalData.DDoc.Common.Entities.DdocSearchFilterGroup
// Assembly: DigitalData.DDoc.Common, Version=4.210.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 37D7F3E1-2CA5-48B0-AB22-FE81D67009C0
// Assembly location: C:\Users\herna\Downloads\ss\BradesCard_DEV\bin\DigitalData.DDoc.Common.dll

using DigitalData.Common.Entities;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace DigitalData.DDoc.Common.Entities
{
  [DataContract]
  public class DdocSearchFilterGroup
  {
    [DataMember]
    [XmlArray("clauses")]
    [XmlArrayItem("clause")]
    public List<DdocSearchFilter> Clauses { get; set; }

    [DataMember]
    [XmlAttribute("clausesOperator")]
    public LogicOperator ClausesOperator { get; set; }

    [DataMember]
    [XmlAttribute("groupOperator")]
    public LogicOperator GroupOperator { get; set; }
  }
}
