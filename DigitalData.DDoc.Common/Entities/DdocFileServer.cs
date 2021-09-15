
using System.ComponentModel;
using System.Runtime.Serialization;

namespace DigitalData.Open.Common.Entities
{
  [DataContract]
  public class DdocFileServer
  {
    [DataMember]
    public string Id { get; set; }

    [DataMember]
    public bool IsNew { get; set; }

    [DataMember]
    [DisplayName("Nombre")]
    public string Name { get; set; }

    [DataMember]
    [DisplayName("URL")]
    public string Url { get; set; }
  }
}
