// Decompiled with JetBrains decompiler
// Type: DigitalData.DDoc.Common.Entities.DdocEntity
// Assembly: DigitalData.DDoc.Common, Version=4.210.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 37D7F3E1-2CA5-48B0-AB22-FE81D67009C0
// Assembly location: C:\Users\herna\Downloads\ss\BradesCard_DEV\bin\DigitalData.DDoc.Common.dll

using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace DigitalData.DDoc.Common.Entities
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
