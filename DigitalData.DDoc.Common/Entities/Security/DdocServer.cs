// Decompiled with JetBrains decompiler
// Type: DigitalData.DDoc.Common.Entities.Security.DdocServer
// Assembly: DigitalData.DDoc.Common, Version=4.210.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 37D7F3E1-2CA5-48B0-AB22-FE81D67009C0
// Assembly location: C:\Users\herna\Downloads\ss\BradesCard_DEV\bin\DigitalData.DDoc.Common.dll

using System.Runtime.Serialization;

namespace DigitalData.Open.Common.Entities.Security
{
  [DataContract]
  public class DdocServer
  {
    [DataMember]
    public string DefaultDomain { get; set; }

    [DataMember]
    public int Id { get; set; }

    [DataMember]
    public string LdapPath { get; set; }

    [DataMember]
    public LogonType LogonType { get; set; }

    [DataMember]
    public string Name { get; set; }

    [DataMember]
    public bool TextIndexing { get; set; }
  }
}
