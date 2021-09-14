// Decompiled with JetBrains decompiler
// Type: DigitalData.DDoc.UI.Web.Controllers.MainController
// Assembly: DigitalData.DDoc.UI.Web, Version=4.210.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1482FEFA-CA2F-44DF-981C-51EA2DF7CBB5
// Assembly location: C:\Users\herna\Downloads\ss\BradesCard_DEV\bin\DigitalData.DDoc.UI.Web.dll

using DigitalData.Common.Entities;
using DigitalData.DDoc.Common.Api;
using DigitalData.DDoc.Common.WebExtensions;
using Microsoft.Extensions.Options;
using System.Web.Mvc;

namespace DigitalData.DDoc.UI.Web.Controllers
{
  public class MainController : DdocController
  {
    public MainController(IDdocService ddoc, IOptions<DiDaSettings> settings)
      : base(ddoc, settings.Value)
    {
    }

    public ViewResult Main() => this.View();

    public ViewResult Welcome() => this.View();
  }
}
