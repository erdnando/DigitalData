﻿// Decompiled with JetBrains decompiler
// Type: DigitalData.DDoc.Common.Entities.Helpers.CollectionChangeRequest
// Assembly: DigitalData.DDoc.Common, Version=4.210.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 37D7F3E1-2CA5-48B0-AB22-FE81D67009C0
// Assembly location: C:\Users\herna\Downloads\ss\BradesCard_DEV\bin\DigitalData.DDoc.Common.dll

using System.Collections.Generic;

namespace DigitalData.DDoc.Common.Entities.Helpers
{
  public class CollectionChangeRequest
  {
    public List<DdocField> NewFields { get; set; }

    public string CollectionId { get; set; }

    public List<FieldCorrespondence> FieldLinks { get; set; }
  }
}
