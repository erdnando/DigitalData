
using DigitalData.Common.Entities;
using DigitalData.Common.WebUtils.AspNet4;
using DigitalData.Open.Common.Api;
using DigitalData.Open.Common.Entities;
using DigitalData.Open.Common.Entities.Helpers;
using DigitalData.Open.Common.Entities.UI;
using DigitalData.Open.Common.Extensions;
using DigitalData.Open.Common.WebExtensions;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace DigitalData.DDoc.UI.Web.Controllers
{
  public class NavigationController : DdocController
  {
    public NavigationController(
      IDdocService ddoc,
      IOptions<DiDaSettings> settings,
      ICustomActions customActions)
      : base(ddoc, settings.Value, customActions.CustomActions)
    {
    }

    [NoCache]
    public async Task<ActionResult> NavigationTree()
    {
      NavigationController navigationController = this;
      List<UiTreeNode> rootNodes = (await navigationController.Ddoc.GetRootCollections(navigationController.CurrentUserSession.UserGroups)).Select<DdocCollection, UiTreeNode>((Func<DdocCollection, UiTreeNode>) (collection => new UiTreeNode()
      {
        key = collection.Id,
        icon = "root",
        title = collection.Name
      })).ToList<UiTreeNode>();
      foreach (UiTreeNode node in rootNodes)
        await BuildCollectionTree(node);
      ActionResult actionResult = (ActionResult) navigationController.Json((object) rootNodes, JsonRequestBehavior.AllowGet);
      rootNodes = (List<UiTreeNode>) null;
      return actionResult;

      async Task BuildCollectionTree(UiTreeNode node)
      {
        NavigationController navigationControllerx = this;
        node.children = (await navigationControllerx.Ddoc.GetChildCollections(node.key, navigationControllerx.CurrentUserSession.UserGroups)).Select<DdocCollection, UiTreeNode>((Func<DdocCollection, UiTreeNode>) (collection =>
        {
          UiTreeNode uiTreeNode = new UiTreeNode();
          uiTreeNode.key = collection.Id;
          uiTreeNode.icon = collection.Type == CollectionType.F ? "folder" : "document";
          uiTreeNode.folder = collection.Type == CollectionType.F;
          uiTreeNode.title = collection.Name;
          CollectionType type = collection.Type;
          uiTreeNode.checkbox = (type == CollectionType.F ? 0 : (type != CollectionType.D ? 1 : 0)) == 0;
          uiTreeNode.unselectable = true;
          uiTreeNode.tooltip = collection.Description;
          return uiTreeNode;
        })).ToList<UiTreeNode>();
        foreach (UiTreeNode child in node.children)
          await BuildCollectionTree(child);
      }
    }

    [NoCache]
    public async Task<ActionResult> GetRootNodes()
    {
      NavigationController navigationController = this;
      List<UiTreeNode> list = (await navigationController.Ddoc.GetRootCollections(navigationController.CurrentUserSession.UserGroups)).Select<DdocCollection, UiTreeNode>((Func<DdocCollection, UiTreeNode>) (collection => new UiTreeNode()
      {
        key = collection.Id,
        icon = "root",
        lazy = true,
        title = collection.Name
      })).ToList<UiTreeNode>();
      return (ActionResult) navigationController.Json((object) list, JsonRequestBehavior.AllowGet);
    }

    [NoCache]
    public async Task<ActionResult> GetChildNodes(string collectionPath)
    {
      NavigationController navigationController = this;
      List<UiTreeNode> list = (await navigationController.Ddoc.GetChildCollections(collectionPath.Substring(collectionPath.LastIndexOf('/') + 1), navigationController.CurrentUserSession.UserGroups)).Select<DdocCollection, UiTreeNode>((Func<DdocCollection, UiTreeNode>) (collection => new UiTreeNode()
      {
        key = collection.Id,
        icon = collection.Type == CollectionType.F ? "folder" : "document",
        folder = collection.Type == CollectionType.F,
        lazy = true,
        title = collection.Name
      })).ToList<UiTreeNode>();
      return (ActionResult) navigationController.Json((object) list, JsonRequestBehavior.AllowGet);
    }

    public async Task<ActionResult> Navigate(string collectionId)
    {
      NavigationController navigationController = this;
      DdocCollection collection = await navigationController.Ddoc.GetCollection(collectionId);
      Permissions elementPermissions = await navigationController.Ddoc.GetElementPermissions(CollectionType.C, collectionId, new GroupFilters()
      {
        UserGroupList = navigationController.CurrentUserSession.UserGroups,
        DdocGroupList = navigationController.CurrentUserSession.DdocGroups
      });
      navigationController.TempData["SearchParameters"] = (object) new DdocSearchParameters()
      {
        CollectionId = collection.Id
      };
      navigationController.ViewData["CollectionId"] = (object) collection.Id;
      navigationController.ViewData["CollectionType"] = (object) (int) collection.Type;
      navigationController.ViewData["CollectionName"] = (object) collection.Name;
      navigationController.ViewData["CollectionPermissions"] = (object) elementPermissions;
      switch (collection.Type)
      {
        case CollectionType.F:
          navigationController.ViewData["ItemType"] = (object) "folder";
          break;
        case CollectionType.D:
          navigationController.ViewData["ItemType"] = (object) "documento";
          break;
        default:
          return (ActionResult) navigationController.RedirectToRoute("DDocMain");
      }
      await navigationController.Ddoc.ActivityLog(new DdocActionLogEntry()
      {
        Action = "OpenCollection",
        User = navigationController.CurrentUserSession.Username,
        Module = navigationController.ModuleName,
        Details = collectionId
      });
      return (ActionResult) navigationController.PartialView("NavigationResults");
    }

    public PartialViewResult MainMenu()
    {
      this.ViewData["EnableAdminMenu"] = (object) this.CurrentUserSession.Roles.Contains("Admin");
      this.ViewData["ReportsEnabled"] = (object) this.Ddoc.InstanceFeatures.ReportsEnabled;
      return this.PartialView();
    }

    public PartialViewResult AdminMenu() => this.PartialView();

    public async Task<ActionResult> OpenFolder(string folderId)
    {
      NavigationController navigationController = this;
      DdocFolder folder = await navigationController.Ddoc.GetFolder(folderId);
      navigationController.ViewData["CollectionId"] = (object) folder.CollectionId;
      ViewDataDictionary viewDataDictionary = navigationController.ViewData;
      viewDataDictionary["ResultCollections"] = (object) await navigationController.Ddoc.NavigateFolder(folderId);
      viewDataDictionary = (ViewDataDictionary) null;
      await navigationController.Ddoc.ActivityLog(new DdocActionLogEntry()
      {
        Action = nameof (OpenFolder),
        User = navigationController.CurrentUserSession.Username,
        Module = navigationController.ModuleName,
        Details = folderId
      });
      return (ActionResult) navigationController.PartialView("FolderContentResults");
    }
  }
}
