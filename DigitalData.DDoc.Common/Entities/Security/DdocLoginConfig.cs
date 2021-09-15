
namespace DigitalData.Open.Common.Entities.Security
{
  public class DdocLoginConfig
  {
    public long InactivityDays { get; set; }

    public bool LoginExpirationEnabled { get; set; }

    public long LoginExpiryDays { get; set; }

    public long LoginWarningDays { get; set; }

    public long Tries { get; set; }
  }
}
