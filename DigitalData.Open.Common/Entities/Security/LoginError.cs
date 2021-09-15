
using System;
using System.Runtime.Serialization;

namespace DigitalData.Open.Common.Entities.Security
{
  [DataContract]
  [Serializable]
  public class LoginError
  {
    [DataMember]
    public DateTime ErrorDateTime { get; set; } = DateTime.Now;

    [DataMember]
    public string ErrorMessage { get; set; }
  }
}
