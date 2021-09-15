
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace DigitalData.Open.Common.Entities
{
  [DataContract]
  public class DdocWarehouse
  {
    [DataMember]
    [DisplayName("Ruta: ")]
    public string ActivePath { get; set; }

    [DataMember]
    public int ActivePathId { get; set; }

    [DataMember]
    public int Id { get; set; }

    [DataMember]
    public bool IsNew { get; set; }

    [DataMember]
    [DisplayName("Nombre: ")]
    public string Name { get; set; }

    [DataMember]
    public List<DdocPath> Paths { get; set; }

    [DataMember]
    [DisplayName("Servidor: ")]
    public string ServerId { get; set; }
  }
}
