
using System.Runtime.Serialization;

namespace DigitalData.Open.Common.Entities.Security
{
  [DataContract]
  public class DdocServer
  {
    [DataMember]
    public string DefaultDomain { get; set; }

    [DataMember]
    public int Id { get; set; }

    [DataMember]
    public string LdapPath { get; set; }

    [DataMember]
    public LogonType LogonType { get; set; }

    [DataMember]
    public string Name { get; set; }

    [DataMember]
    public bool TextIndexing { get; set; }
  }
}
