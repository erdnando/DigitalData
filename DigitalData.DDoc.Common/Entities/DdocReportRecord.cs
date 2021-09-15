
using System;

namespace DigitalData.Open.Common.Entities
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
