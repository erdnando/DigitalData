
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace DigitalData.Open.Common.Entities
{
  [DataContract]
  public class DdocDocument : DdocChildEntity
  {
    [DataMember]
    public int PageCount { get; set; }

    [DataMember]
    public List<DdocPage> Pages { get; set; }

    [DataMember]
    public bool Commited { get; set; }
  }
}
