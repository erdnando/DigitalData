// Decompiled with JetBrains decompiler
// Type: DigitalData.DDoc.Common.Entities.Security.User
// Assembly: DigitalData.DDoc.Common, Version=4.210.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 37D7F3E1-2CA5-48B0-AB22-FE81D67009C0
// Assembly location: C:\Users\herna\Downloads\ss\BradesCard_DEV\bin\DigitalData.DDoc.Common.dll

using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace DigitalData.Open.Common.Entities.Security
{
  [DataContract]
  public class User
  {
    [DataMember]
    [DisplayName("Email: ")]
    public string Email { get; set; }

    [DataMember]
    public DdocInstanceType InstanceType { get; set; }

    [DataMember]
    public bool IsNew { get; set; }

    [DataMember]
    [DisplayName("Nombre Completo: ")]
    public string Name { get; set; }

    [DataMember]
    [DisplayName("Contraseña actual: ")]
    public string OldPassword { get; set; }

    [DataMember]
    [DisplayName("Contraseña: ")]
    public string Password { get; set; }

    [DataMember]
    [DisplayName("Confirmar contraseña: ")]
    public string PasswordConfirmation { get; set; }

    [DataMember]
    public List<DdocGroup> Profile { get; set; } = new List<DdocGroup>();

    [DataMember]
    [DisplayName("Usuario: ")]
    public string Username { get; set; }
  }
}
