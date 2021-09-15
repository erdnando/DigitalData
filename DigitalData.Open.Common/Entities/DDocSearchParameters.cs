
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace DigitalData.Open.Common.Entities
{
  [DataContract]
  [XmlRoot("ddocSearchParameters")]
  public class DdocSearchParameters
  {
    [DataMember]
    [XmlArray("clauseGroups")]
    [XmlArrayItem("group")]
    public List<DdocSearchFilterGroup> ClauseGroups { get; set; }

    [DataMember]
    [XmlAttribute("collectionId")]
    public string CollectionId { get; set; }

    [DataMember]
    [XmlIgnore]
    public TextSearchType SearchType { get; set; }

    [DataMember]
    [XmlIgnore]
    public string SecurityGroupsCsv { get; set; }

    [DataMember]
    [XmlAttribute("sortBy")]
    public string SortResultsBy { get; set; }

    [DataMember]
    [XmlAttribute("sortDirection")]
    public int SortDirection { get; set; }

    [DataMember]
    [XmlIgnore]
    public string TextQuery { get; set; }

    [DataMember]
    [XmlIgnore]
    public List<string> IncludedCollections { get; set; }
  }
}
