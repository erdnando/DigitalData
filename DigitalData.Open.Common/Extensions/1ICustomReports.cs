
using DigitalData.Open.Common.Entities.Helpers;
using System.Collections.Generic;

namespace DigitalData.Open.Common.Extensions
{
  public interface ICustomReports
  {
    IEnumerable<DdocReport> ReportList { get; set; }
  }
}
