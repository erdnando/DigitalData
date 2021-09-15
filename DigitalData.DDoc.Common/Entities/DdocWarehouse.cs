// Decompiled with JetBrains decompiler
// Type: DigitalData.DDoc.Common.Entities.DdocWarehouse
// Assembly: DigitalData.DDoc.Common, Version=4.210.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 37D7F3E1-2CA5-48B0-AB22-FE81D67009C0
// Assembly location: C:\Users\herna\Downloads\ss\BradesCard_DEV\bin\DigitalData.DDoc.Common.dll

using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace DigitalData.Open.Common.Entities
{
  [DataContract]
  public class DdocWarehouse
  {
    [DataMember]
    [DisplayName("Ruta: ")]
    public string ActivePath { get; set; }

    [DataMember]
    public int ActivePathId { get; set; }

    [DataMember]
    public int Id { get; set; }

    [DataMember]
    public bool IsNew { get; set; }

    [DataMember]
    [DisplayName("Nombre: ")]
    public string Name { get; set; }

    [DataMember]
    public List<DdocPath> Paths { get; set; }

    [DataMember]
    [DisplayName("Servidor: ")]
    public string ServerId { get; set; }
  }
}
