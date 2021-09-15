
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
