// Decompiled with JetBrains decompiler
// Type: DigitalData.DDoc.Common.Api.DataAccess.IReportsDAL
// Assembly: DigitalData.DDoc.Common, Version=4.210.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 37D7F3E1-2CA5-48B0-AB22-FE81D67009C0
// Assembly location: C:\Users\herna\Downloads\ss\BradesCard_DEV\bin\DigitalData.DDoc.Common.dll

using DigitalData.Open.Common.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DigitalData.Open.Common.Api.DataAccess
{
  public interface IReportsDAL : ICommonDAL
  {
    Task<IEnumerable<DdocReportRecord>> GetDailyReport(
      DateTime fromDate,
      DateTime toDate);

    Task<IEnumerable<DdocReportRecord>> GetRangeDate(
      IEnumerable<int> securityGroups,
      DateTime fromDate,
      DateTime toDate);

    Task<IEnumerable<DdocReportRecord>> GetTotalReport(
      IEnumerable<int> securityGroups);

    Task<bool> ShowDefaultReports();
  }
}
