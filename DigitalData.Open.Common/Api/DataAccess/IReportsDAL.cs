
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
