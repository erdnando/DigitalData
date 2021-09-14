// Decompiled with JetBrains decompiler
// Type: DigitalData.DDoc.Common.Entities.DdocChildEntityComparer
// Assembly: DigitalData.DDoc.Common, Version=4.210.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 37D7F3E1-2CA5-48B0-AB22-FE81D67009C0
// Assembly location: C:\Users\herna\Downloads\ss\BradesCard_DEV\bin\DigitalData.DDoc.Common.dll

using System.Collections.Generic;

namespace DigitalData.DDoc.Common.Entities
{
  public class DdocChildEntityComparer : EqualityComparer<DdocChildEntity>
  {
    public override bool Equals(DdocChildEntity x, DdocChildEntity y) => x.Id == y.Id;

    public override int GetHashCode(DdocChildEntity obj) => this.GetHashCode();
  }
}
