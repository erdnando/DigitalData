
using System.ComponentModel;
using System.Runtime.Serialization;

namespace DigitalData.Open.Common.Entities
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
