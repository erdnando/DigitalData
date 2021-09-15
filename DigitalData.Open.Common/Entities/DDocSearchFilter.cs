
using DigitalData.Common.Entities;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace DigitalData.Open.Common.Entities
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
