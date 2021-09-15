
using DigitalData.Open.Common.Api;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace DigitalData.DDoc.UI.Web.Modules
{
  [HubName("DDocHub")]
  public class NotificationHub : Hub
  {
    private readonly IDdocService ddoc;

    public NotificationHub(IDdocService ddoc) => this.ddoc = ddoc;
  }
}
