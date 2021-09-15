// Decompiled with JetBrains decompiler
// Type: DigitalData.DDoc.Common.Entities.DdocPath
// Assembly: DigitalData.DDoc.Common, Version=4.210.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 37D7F3E1-2CA5-48B0-AB22-FE81D67009C0
// Assembly location: C:\Users\herna\Downloads\ss\BradesCard_DEV\bin\DigitalData.DDoc.Common.dll

using System.ComponentModel;
using System.Runtime.Serialization;

namespace DigitalData.Open.Common.Entities
{
  [DataContract]
  public class DdocPath
  {
    [DataMember]
    public bool Active { get; set; }

    [DataMember]
    public int Id { get; set; }

    [DataMember]
    public bool IsNew { get; set; }

    [DataMember]
    [DisplayName("Ruta raíz: ")]
    public string RootPath { get; set; }
  }
}
