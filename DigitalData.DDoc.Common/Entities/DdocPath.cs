
using System.ComponentModel;
using System.Runtime.Serialization;

namespace DigitalData.Open.Common.Entities
{
  [DataContract]
  public class DdocPath
  {
    [DataMember]
    public bool Active { get; set; }

    [DataMember]
    public int Id { get; set; }

    [DataMember]
    public bool IsNew { get; set; }

    [DataMember]
    [DisplayName("Ruta raíz: ")]
    public string RootPath { get; set; }
  }
}
