﻿// Decompiled with JetBrains decompiler
// Type: DigitalData.DDoc.Common.Entities.DdocFileServer
// Assembly: DigitalData.DDoc.Common, Version=4.210.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 37D7F3E1-2CA5-48B0-AB22-FE81D67009C0
// Assembly location: C:\Users\herna\Downloads\ss\BradesCard_DEV\bin\DigitalData.DDoc.Common.dll

using System.ComponentModel;
using System.Runtime.Serialization;

namespace DigitalData.DDoc.Common.Entities
{
  [DataContract]
  public class DdocFileServer
  {
    [DataMember]
    public string Id { get; set; }

    [DataMember]
    public bool IsNew { get; set; }

    [DataMember]
    [DisplayName("Nombre")]
    public string Name { get; set; }

    [DataMember]
    [DisplayName("URL")]
    public string Url { get; set; }
  }
}