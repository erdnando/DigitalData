// Decompiled with JetBrains decompiler
// Type: DigitalData.Ddoc.Plugins.GalleryController
// Assembly: DigitalData.Ddoc.Plugins.BRADESCARD, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 76226255-9B51-4DC8-9719-6E02C225B847
// Assembly location: D:\proyectos\ddocx\DigitalData.DDoc.UI.Webx\DigitalData.DDoc.UI.Web\bin\DigitalData.Ddoc.Plugins.BRADESCARD.dll

using DigitalData.Common.Entities;
using DigitalData.Common.IoC;
using DigitalData.DDoc.Common.Api;
using DigitalData.DDoc.Common.Entities;
using DigitalData.DDoc.Common.Entities.Helpers;
using DigitalData.DDoc.Common.WebExtensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace DigitalData.Ddoc.Plugins
{
  public class GalleryController : DdocController
  {
    public static void ExtensionSetup(IServiceCollection services) => IoCContainer.AddTransient<GalleryDAL>();

    public GalleryController(IDdocService ddoc, IOptions<DiDaSettings> settings)
      : base(ddoc, settings.Value)
    {
    }

    public ActionResult ViewDocuments(string expNumber, string apiToken)
    {
      this.ViewData["ApiToken"] = (object) apiToken;
      this.ViewData["ExpNumber"] = (object) expNumber;
      this.ViewData["Editable"] = (object) false;
      return (ActionResult) this.PartialView("Gallery");
    }

    public async Task<ActionResult> EditDocuments(string expNumber, string apiToken)
    {
      GalleryController galleryController = this;
      string folderId;
      using (GalleryDAL dataAccess = IoCContainer.GetService<GalleryDAL>())
        folderId = await dataAccess.GetFolderId(expNumber);
      Permissions elementPermissions = await galleryController.Ddoc.GetElementPermissions(CollectionType.F, folderId, new GroupFilters()
      {
        UserGroupList = galleryController.CurrentUserSession.UserGroups,
        DdocGroupList = galleryController.CurrentUserSession.DdocGroups
      });
      galleryController.ViewData["CanRead"] = (object) elementPermissions.HasFlag((Enum) Permissions.Read);
      galleryController.ViewData["CanWrite"] = (object) elementPermissions.HasFlag((Enum) Permissions.Write);
      galleryController.ViewData["CanComment"] = (object) elementPermissions.HasFlag((Enum) Permissions.Comment);
      galleryController.ViewData["CanExport"] = (object) elementPermissions.HasFlag((Enum) Permissions.Export);
      galleryController.ViewData["CanPrint"] = (object) elementPermissions.HasFlag((Enum) Permissions.Print);
      galleryController.ViewData["CanDelete"] = (object) elementPermissions.HasFlag((Enum) Permissions.Delete);
      galleryController.ViewData["ApiToken"] = (object) apiToken;
      galleryController.ViewData["ExpNumber"] = (object) expNumber;
      galleryController.ViewData["EnableWebTwain"] = (object) galleryController.Ddoc.InstanceFeatures.WebTwain;
      galleryController.ViewData["Editable"] = (object) true;
      return (ActionResult) galleryController.PartialView("Gallery");
    }
  }
}
