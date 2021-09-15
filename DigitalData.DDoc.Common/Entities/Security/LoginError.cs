// Decompiled with JetBrains decompiler
// Type: DigitalData.DDoc.Common.Entities.Security.LoginError
// Assembly: DigitalData.DDoc.Common, Version=4.210.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 37D7F3E1-2CA5-48B0-AB22-FE81D67009C0
// Assembly location: C:\Users\herna\Downloads\ss\BradesCard_DEV\bin\DigitalData.DDoc.Common.dll

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
