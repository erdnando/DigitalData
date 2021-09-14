// Decompiled with JetBrains decompiler
// Type: DigitalData.DDoc.Common.Entities.DdocReportRecord
// Assembly: DigitalData.DDoc.Common, Version=4.210.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 37D7F3E1-2CA5-48B0-AB22-FE81D67009C0
// Assembly location: C:\Users\herna\Downloads\ss\BradesCard_DEV\bin\DigitalData.DDoc.Common.dll

using System;

namespace DigitalData.DDoc.Common.Entities
{
  public class DdocReportRecord
  {
    public DateTime Date { get; set; }

    public string CollectionId { get; set; }

    public string CollectionName { get; set; }

    public int DocumentCount { get; set; }

    public int PageCount { get; set; }
  }
}
