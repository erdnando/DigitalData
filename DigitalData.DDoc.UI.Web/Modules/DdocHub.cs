// Decompiled with JetBrains decompiler
// Type: DigitalData.DDoc.UI.Web.Modules.NotificationHub
// Assembly: DigitalData.DDoc.UI.Web, Version=4.210.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1482FEFA-CA2F-44DF-981C-51EA2DF7CBB5
// Assembly location: C:\Users\herna\Downloads\ss\BradesCard_DEV\bin\DigitalData.DDoc.UI.Web.dll

using DigitalData.DDoc.Common.Api;
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
