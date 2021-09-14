// Decompiled with JetBrains decompiler
// Type: DigitalData.DDoc.Common.Entities.DdocConfiguration
// Assembly: DigitalData.DDoc.Common, Version=4.210.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 37D7F3E1-2CA5-48B0-AB22-FE81D67009C0
// Assembly location: C:\Users\herna\Downloads\ss\BradesCard_DEV\bin\DigitalData.DDoc.Common.dll

using System.ComponentModel;
using System.Runtime.Serialization;

namespace DigitalData.DDoc.Common.Entities
{
  [DataContract]
  public class DdocConfiguration
  {
    [DataMember]
    [DisplayName("Aviso de vencimiento: ")]
    public int DaysForWarning { get; set; }

    [DataMember]
    [DisplayName("Habilitar vigencia de contraseña")]
    public bool EnablePasswordExpiration { get; set; }

    [DataMember]
    [DisplayName("Habilitar historial de contraseña")]
    public bool EnablePasswordHistory { get; set; }

    [DataMember]
    [DisplayName("Vigencia de contraseña: ")]
    public int ExpirationDays { get; set; }

    [DataMember]
    [DisplayName("Intentos fallidos: ")]
    public int FailedTries { get; set; }

    [DataMember]
    [DisplayName("Días de inactividad: ")]
    public int InactivityDays { get; set; }

    [DataMember]
    [DisplayName("Longitud máxima: ")]
    public int PasswordMaxLength { get; set; }

    [DataMember]
    [DisplayName("Longitud mínima: ")]
    public int PasswordMinLength { get; set; }

    [DataMember]
    [DisplayName("Contraseñas pasadas a validar: ")]
    public int PastPasswordCheck { get; set; }
  }
}
