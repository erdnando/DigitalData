
using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace DigitalData.Open.Common.Entities
{
  [DataContract]
  [KnownType(typeof (DdocChildEntity))]
  [KnownType(typeof (DdocFolder))]
  [KnownType(typeof (DdocDocument))]
  [KnownType(typeof (DdocField))]
  public class DdocEntity : IEquatable<DdocEntity>
  {
    [DataMember]
    public string Id { get; set; }

    [DataMember]
    [DisplayName("Nombre: ")]
    public string Name { get; set; }

    [DataMember]
    [DisplayName("Grupo de Seguridad: ")]
    public int SecurityGroupId { get; set; }

    public bool Equals(DdocEntity other) => string.Equals(this.Id, other.Id, StringComparison.OrdinalIgnoreCase);

    public override bool Equals(object obj) => obj is DdocEntity other && this.Equals(other);

    public override int GetHashCode() => this.Id == null ? 0 : StringComparer.OrdinalIgnoreCase.GetHashCode(this.Id);
  }
}
