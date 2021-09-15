
using System.ComponentModel;
using System.Runtime.Serialization;

namespace DigitalData.Open.Common.Entities.Security
{
  [DataContract]
  public class DdocGroup
  {
    [DataMember]
    public int Id { get; set; }

    [DataMember]
    public bool IsNew { get; set; }

    [DataMember]
    [DisplayName("Nombre: ")]
    public string Name { get; set; }

    [DataMember]
    public GroupType Type { get; set; }
  }
}
