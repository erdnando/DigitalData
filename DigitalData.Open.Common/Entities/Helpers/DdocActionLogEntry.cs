
using System;

namespace DigitalData.Open.Common.Entities.Helpers
{
  public class DdocActionLogEntry
  {
    public string Action { get; set; }

    public string Details { get; set; }

    public DateTime EventDate { get; } = DateTime.Now;

    public string Module { get; set; }

    public string User { get; set; }
  }
}
