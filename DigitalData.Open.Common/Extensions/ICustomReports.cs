// Decompiled with JetBrains decompiler
// Type: DigitalData.DDoc.Common.Extensions.NullCustomReports
// Assembly: DigitalData.DDoc.Common, Version=4.210.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 37D7F3E1-2CA5-48B0-AB22-FE81D67009C0
// Assembly location: C:\Users\herna\Downloads\ss\BradesCard_DEV\bin\DigitalData.DDoc.Common.dll

using DigitalData.Open.Common.Entities.Helpers;
using System.Collections.Generic;

namespace DigitalData.Open.Common.Extensions
{
  public class NullCustomReports : ICustomReports
  {
    public IEnumerable<DdocReport> ReportList { get; set; }
  }
}
