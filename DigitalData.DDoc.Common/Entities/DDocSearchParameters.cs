// Decompiled with JetBrains decompiler
// Type: DigitalData.DDoc.Common.Entities.DdocSearchParameters
// Assembly: DigitalData.DDoc.Common, Version=4.210.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 37D7F3E1-2CA5-48B0-AB22-FE81D67009C0
// Assembly location: C:\Users\herna\Downloads\ss\BradesCard_DEV\bin\DigitalData.DDoc.Common.dll

using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace DigitalData.DDoc.Common.Entities
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
