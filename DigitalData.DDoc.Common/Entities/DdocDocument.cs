// Decompiled with JetBrains decompiler
// Type: DigitalData.DDoc.Common.Entities.DdocDocument
// Assembly: DigitalData.DDoc.Common, Version=4.210.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 37D7F3E1-2CA5-48B0-AB22-FE81D67009C0
// Assembly location: C:\Users\herna\Downloads\ss\BradesCard_DEV\bin\DigitalData.DDoc.Common.dll

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace DigitalData.DDoc.Common.Entities
{
  [DataContract]
  public class DdocDocument : DdocChildEntity
  {
    [DataMember]
    public int PageCount { get; set; }

    [DataMember]
    public List<DdocPage> Pages { get; set; }

    [DataMember]
    public bool Commited { get; set; }
  }
}
