// Decompiled with JetBrains decompiler
// Type: DigitalData.DDoc.UI.Web.Controllers.AdminController
// Assembly: DigitalData.DDoc.UI.Web, Version=4.210.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1482FEFA-CA2F-44DF-981C-51EA2DF7CBB5
// Assembly location: C:\Users\herna\Downloads\ss\BradesCard_DEV\bin\DigitalData.DDoc.UI.Web.dll

using DigitalData.Common.Entities;
using DigitalData.Common.WebUtils.AspNet4;
using DigitalData.Open.Common.Api;
using DigitalData.Open.Common.Entities;
using DigitalData.Open.Common.Entities.Security;
using DigitalData.Open.Common.Entities.UI;
using DigitalData.Open.Common.WebExtensions;
using Microsoft.CSharp.RuntimeBinder;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace DigitalData.DDoc.UI.Web.Controllers
{
  [UserAuthorization(new string[] {"Admin"})]
  public class AdminController : DdocController
  {
    public AdminController(IDdocService ddoc, IOptions<DiDaSettings> settings)
      : base(ddoc, settings.Value)
    {
    }

    public ViewResult Main() => this.View(nameof (Main));

    public ViewResult Welcome() => this.View(nameof (Welcome));

    public async Task<ActionResult> CollectionManagment()
    {
      AdminController adminController = this;
      ViewDataDictionary viewDataDictionary = adminController.ViewData;
      viewDataDictionary["CollectionLock"] = (object) await adminController.Ddoc.GetCollectionLockStatus();
      viewDataDictionary = (ViewDataDictionary) null;
      return (ActionResult) adminController.PartialView();
    }

    public async Task<ActionResult> NewRootCollection()
    {
      AdminController adminController = this;
      DdocCollection ddocCollection = new DdocCollection();
      ddocCollection.IsNew = true;
      ddocCollection.Type = CollectionType.C;
      ddocCollection.SecurityGroupId = 0;
      ddocCollection.WarehouseId = new int?(1);
      DdocCollection newCollection = ddocCollection;
      List<DdocGroup> securityGroupsList = new List<DdocGroup>();
      List<DdocGroup> ddocGroupList = securityGroupsList;
      ddocGroupList.AddRange((IEnumerable<DdocGroup>) await adminController.Ddoc.GetDdocGroups(GroupType.SecurityGroup));
      ddocGroupList = (List<DdocGroup>) null;
      adminController.ViewData["SecurityEntries"] = (object) securityGroupsList;
      ActionResult actionResult = (ActionResult) adminController.PartialView("CollectionEditor", (object) newCollection);
      newCollection = (DdocCollection) null;
      securityGroupsList = (List<DdocGroup>) null;
      return actionResult;
    }

    public async Task<ActionResult> NewFolderCollection(
      string parentCollectionId,
      int securityGroupId)
    {
      AdminController adminController = this;
      DdocCollection ddocCollection1 = new DdocCollection();
      ddocCollection1.IsNew = true;
      ddocCollection1.ParentId = parentCollectionId;
      ddocCollection1.SecurityGroupId = securityGroupId;
      ddocCollection1.Type = CollectionType.F;
      ddocCollection1.WarehouseId = new int?(1);
      DdocCollection newFolderCollection = ddocCollection1;
      DdocCollection ddocCollection2 = (await adminController.Ddoc.GetCollections()).Single<DdocCollection>((Func<DdocCollection, bool>) (c => c.Id == parentCollectionId));
      adminController.ViewData["ParentCollectionName"] = (object) ddocCollection2.Name;
      if (ddocCollection2.Type != CollectionType.C)
        adminController.ViewData["ParentCollectionId"] = (object) ddocCollection2.Id;
      ViewDataDictionary viewDataDictionary = adminController.ViewData;
      viewDataDictionary["SecurityEntries"] = (object) await adminController.Ddoc.GetDdocGroups(GroupType.SecurityGroup);
      viewDataDictionary = (ViewDataDictionary) null;
      ActionResult actionResult = (ActionResult) adminController.PartialView("CollectionEditor", (object) newFolderCollection);
      newFolderCollection = (DdocCollection) null;
      return actionResult;
    }

    public async Task<ActionResult> NewDocumentCollection(
      string parentCollectionId,
      int securityGroupId)
    {
      AdminController adminController = this;
      DdocCollection ddocCollection1 = new DdocCollection();
      ddocCollection1.IsNew = true;
      ddocCollection1.ParentId = parentCollectionId;
      ddocCollection1.SecurityGroupId = securityGroupId;
      ddocCollection1.Type = CollectionType.D;
      DdocCollection newDocumentCollection = ddocCollection1;
      DdocCollection ddocCollection2 = (await adminController.Ddoc.GetCollections()).Single<DdocCollection>((Func<DdocCollection, bool>) (c => c.Id == parentCollectionId));
      adminController.ViewData["ParentCollectionName"] = (object) ddocCollection2.Name;
      if (ddocCollection2.Type != CollectionType.C)
        adminController.ViewData["ParentCollectionId"] = (object) ddocCollection2.Id;
      ViewDataDictionary viewDataDictionary = adminController.ViewData;
      viewDataDictionary["Warehouses"] = (object) await adminController.Ddoc.GetWarehouses();
      viewDataDictionary = (ViewDataDictionary) null;
      viewDataDictionary = adminController.ViewData;
      viewDataDictionary["SecurityEntries"] = (object) await adminController.Ddoc.GetDdocGroups(GroupType.SecurityGroup);
      viewDataDictionary = (ViewDataDictionary) null;
      adminController.ViewData["EnableWarehouses"] = (object) adminController.Ddoc.Storage.Name.Equals("FileSystem");
      ActionResult actionResult = (ActionResult) adminController.PartialView("CollectionEditor", (object) newDocumentCollection);
      newDocumentCollection = (DdocCollection) null;
      return actionResult;
    }

    public async Task<ActionResult> EditCollection(DdocCollection collection)
    {
      AdminController adminController = this;
      ViewDataDictionary viewDataDictionary = adminController.ViewData;
      viewDataDictionary["Warehouses"] = (object) await adminController.Ddoc.GetWarehouses();
      viewDataDictionary = (ViewDataDictionary) null;
      viewDataDictionary = adminController.ViewData;
      viewDataDictionary["SecurityEntries"] = (object) await adminController.Ddoc.GetDdocGroups(GroupType.SecurityGroup);
      viewDataDictionary = (ViewDataDictionary) null;
      return (ActionResult) adminController.PartialView("CollectionEditor", (object) collection);
    }

    public PartialViewResult EditCollectionFilenameTemplate() => this.PartialView("CollectionFilenameTemplateEditor");

    public PartialViewResult NewCollectionField() => this.PartialView("CollectionFieldEditor", (object) new DdocField()
    {
      IsNew = true
    });

    public PartialViewResult EditCollectionField(DdocField field) => this.PartialView("CollectionFieldEditor", (object) field);

    public PartialViewResult NewAllowedValue() => this.PartialView("FieldAllowedValuesEditor");

    public PartialViewResult RulesList() => this.PartialView();

    public PartialViewResult NewRule() => this.PartialView("RuleEditor", (object) new DdocRule()
    {
      IsNew = true
    });

    public PartialViewResult EditRule(DdocRule rule) => this.PartialView("RuleEditor", (object) rule);

    [NoCache]
    public async Task<ActionResult> GetCollectionRules()
    {
      AdminController adminController = this;
      Response<DdocRule> response1 = new Response<DdocRule>();
      try
      {
        Response<DdocRule> response2 = response1;
        response2.List = await adminController.Ddoc.GetCollectionRules();
        response2 = (Response<DdocRule>) null;
        response1.Result = RequestResult.Success;
      }
      catch (Exception ex)
      {
        response1.Message = ex.Message;
      }
      ActionResult actionResult = (ActionResult) adminController.Json((object) response1, JsonRequestBehavior.AllowGet);
      response1 = (Response<DdocRule>) null;
      return actionResult;
    }

    [NoCache]
    public async Task<ActionResult> GetCollectionTree()
    {
      AdminController adminController = this;
      List<UiTreeNode> rootNodes = (await adminController.Ddoc.GetRootCollections((string) null)).Select<DdocCollection, UiTreeNode>((Func<DdocCollection, UiTreeNode>) (collection => new UiTreeNode()
      {
        key = collection.Id,
        expanded = true,
        collection = JsonConvert.SerializeObject((object) collection),
        icon = "root",
        title = collection.Name
      })).ToList<UiTreeNode>();
      foreach (UiTreeNode node in rootNodes)
        await BuildCollectionTree(node);
      ActionResult actionResult = (ActionResult) adminController.Json((object) rootNodes, JsonRequestBehavior.AllowGet);
      rootNodes = (List<UiTreeNode>) null;
      return actionResult;

      async Task BuildCollectionTree(UiTreeNode node)
      {
        AdminController adminControllerx = this;
        node.children = (await adminControllerx.Ddoc.GetChildCollections(node.key, (string) null)).Select<DdocCollection, UiTreeNode>((Func<DdocCollection, UiTreeNode>) (collection => new UiTreeNode()
        {
          key = collection.Id,
          expanded = true,
          collection = JsonConvert.SerializeObject((object) collection),
          icon = collection.Type == CollectionType.F ? "folder" : "document",
          folder = collection.Type == CollectionType.F,
          title = collection.Name
        })).ToList<UiTreeNode>();
        foreach (UiTreeNode child in node.children)
          await BuildCollectionTree(child);
      }
    }

    [HttpPost]
    public async Task<ActionResult> SaveCollectionRule(DdocRule rule)
    {
      AdminController adminController = this;
      Response<DdocRule> response1 = new Response<DdocRule>();
      try
      {
        Response<DdocRule> response2 = response1;
        response2.Code = await adminController.Ddoc.SaveCollectionRule(rule);
        response2 = (Response<DdocRule>) null;
        rule.Id = response1.Code;
        response1.ObjectResult = rule;
        response1.Result = RequestResult.Success;
      }
      catch (Exception ex)
      {
        response1.Message = ex.Message;
      }
      ActionResult actionResult = (ActionResult) adminController.Json((object) response1);
      response1 = (Response<DdocRule>) null;
      return actionResult;
    }

    [HttpPost]
    public async Task<ActionResult> DeleteCollectionRule(int ruleId)
    {
      AdminController adminController = this;
      Response response = new Response();
      try
      {
        await adminController.Ddoc.DeleteCollectionRule(ruleId);
        response.Result = RequestResult.Success;
      }
      catch (Exception ex)
      {
        response.Message = ex.Message;
      }
      ActionResult actionResult = (ActionResult) adminController.Json((object) response);
      response = (Response) null;
      return actionResult;
    }

    [HttpPost]
    public async Task<ActionResult> SaveCollection(DdocCollection collection)
    {
      AdminController adminController = this;
      Response<DdocCollection> response1 = new Response<DdocCollection>();
      try
      {
        Response<DdocCollection> response2 = response1;
        response2.TextResult = await adminController.Ddoc.SaveCollection(collection);
        response2 = (Response<DdocCollection>) null;
        collection.Id = response1.TextResult;
        response1.ObjectResult = collection;
        response1.Result = RequestResult.Success;
      }
      catch (Exception ex)
      {
        response1.Message = ex.Message;
      }
      ActionResult actionResult = (ActionResult) adminController.Json((object) response1);
      response1 = (Response<DdocCollection>) null;
      return actionResult;
    }

    [HttpPost]
    public async Task<ActionResult> SaveCollectionFilenameTemplate(
      string collectionId,
      string filenameTemplate)
    {
      AdminController adminController = this;
      Response response = new Response();
      try
      {
        int num = await adminController.Ddoc.SaveCollectionFilenameTemplate(collectionId, filenameTemplate);
        response.Result = RequestResult.Success;
      }
      catch (Exception ex)
      {
        response.Message = ex.Message;
      }
      ActionResult actionResult = (ActionResult) adminController.Json((object) response);
      response = (Response) null;
      return actionResult;
    }

    [HttpPost]
    public async Task<ActionResult> DeleteCollection(string collectionId)
    {
      AdminController adminController = this;
      Response response = new Response();
      try
      {
        await adminController.Ddoc.DeleteCollection(collectionId);
        response.Result = RequestResult.Success;
      }
      catch (Exception ex)
      {
        response.Message = ex.Message;
      }
      ActionResult actionResult = (ActionResult) adminController.Json((object) response);
      response = (Response) null;
      return actionResult;
    }

    [HttpPost]
    public async Task<ActionResult> SaveCollectionField(
      string collectionId,
      DdocField field)
    {
      AdminController adminController = this;
      Response response1 = new Response();
      try
      {
        Response response2 = response1;
        response2.Code = await adminController.Ddoc.SaveCollectionField(collectionId, field);
        response2 = (Response) null;
        response1.Result = RequestResult.Success;
      }
      catch (Exception ex)
      {
        response1.Message = ex.Message;
      }
      ActionResult actionResult = (ActionResult) adminController.Json((object) response1);
      response1 = (Response) null;
      return actionResult;
    }

    [HttpPost]
    public async Task<ActionResult> MoveCollectionField(int fieldId, bool direction)
    {
      AdminController adminController = this;
      Response response = new Response();
      try
      {
        await adminController.Ddoc.MoveCollectionField(fieldId, direction);
        response.Result = RequestResult.Success;
      }
      catch (Exception ex)
      {
        response.Message = ex.Message;
      }
      ActionResult actionResult = (ActionResult) adminController.Json((object) response);
      response = (Response) null;
      return actionResult;
    }

    [HttpPost]
    public async Task<ActionResult> DeleteCollectionField(int fieldId)
    {
      AdminController adminController = this;
      Response response = new Response();
      try
      {
        await adminController.Ddoc.DeleteCollectionField(fieldId);
        response.Result = RequestResult.Success;
      }
      catch (Exception ex)
      {
        response.Message = ex.Message;
      }
      ActionResult actionResult = (ActionResult) adminController.Json((object) response);
      response = (Response) null;
      return actionResult;
    }

    [HttpPost]
    public async Task<ActionResult> FinalizeCollections()
    {
      AdminController adminController = this;
      Response response = new Response();
      try
      {
        await adminController.Ddoc.FinalizeCollections();
        response.Result = RequestResult.Success;
      }
      catch (Exception ex)
      {
        response.Message = ex.Message;
      }
      ActionResult actionResult = (ActionResult) adminController.Json((object) response);
      response = (Response) null;
      return actionResult;
    }

    [NoCache]
    public async Task<ActionResult> GetCollectionLockStatus()
    {
      AdminController adminController = this;
      Response<bool> response = new Response<bool>();
      try
      {
        response.ObjectResult = await adminController.Ddoc.GetCollectionLockStatus();
        response.Result = RequestResult.Success;
      }
      catch (Exception ex)
      {
        response.Message = ex.Message;
      }
      ActionResult actionResult = (ActionResult) adminController.Json((object) response, JsonRequestBehavior.AllowGet);
      response = (Response<bool>) null;
      return actionResult;
    }

    public PartialViewResult SecurityManagment() => this.PartialView();

    public PartialViewResult NewDdocGroup(GroupType groupType) => this.PartialView("GroupEditor", (object) new DdocGroup()
    {
      IsNew = true,
      Type = groupType
    });

    public PartialViewResult EditDdocGroup(DdocGroup group) => this.PartialView("GroupEditor", (object) group);

        public PartialViewResult ViewDdocGroupPermissions(string groupName, GroupType groupType)
        {
            ((dynamic)base.ViewBag).GroupName = groupName;
            ((dynamic)base.ViewBag).GroupType = groupType;
            return base.PartialView("GroupPermissionList");
        }

        public PartialViewResult PermissionManagement() => this.PartialView(nameof (PermissionManagement));

    public async Task<ActionResult> NewPermission(
      int isGroupPermission,
      GroupType groupType)
    {
      AdminController adminController = this;
      DdocPermission newPermission = new DdocPermission()
      {
        IsNew = true
      };
      List<DdocGroup> userGroups = await adminController.Ddoc.GetDdocGroups(GroupType.UserGroup);
      List<DdocGroup> securityGroups = await adminController.Ddoc.GetDdocGroups(GroupType.SecurityGroup);
      if (isGroupPermission != 0)
      {
        switch (groupType)
        {
          case GroupType.UserGroup:
            newPermission.UserGroupId = isGroupPermission;
            newPermission.UserGroupName = userGroups.Single<DdocGroup>((Func<DdocGroup, bool>) (g => g.Id == isGroupPermission)).Name;
            List<DdocPermission> userGroupPermissions = await adminController.Ddoc.GetDdocGroupPermissions(isGroupPermission, GroupType.UserGroup);
            securityGroups = securityGroups.Where<DdocGroup>((Func<DdocGroup, bool>) (g => !userGroupPermissions.Select<DdocPermission, int>((Func<DdocPermission, int>) (ugp => ugp.SecurityGroupId)).Contains<int>(g.Id))).ToList<DdocGroup>();
            break;
          case GroupType.SecurityGroup:
            newPermission.SecurityGroupId = isGroupPermission;
            newPermission.SecurityGroupName = securityGroups.Single<DdocGroup>((Func<DdocGroup, bool>) (g => g.Id == isGroupPermission)).Name;
            List<DdocPermission> securityGroupPermissions = await adminController.Ddoc.GetDdocGroupPermissions(isGroupPermission, GroupType.SecurityGroup);
            userGroups = userGroups.Where<DdocGroup>((Func<DdocGroup, bool>) (g => !securityGroupPermissions.Select<DdocPermission, int>((Func<DdocPermission, int>) (sgp => sgp.UserGroupId)).Contains<int>(g.Id))).ToList<DdocGroup>();
            break;
        }
      }
      adminController.ViewData["UserGroups"] = (object) userGroups;
      adminController.ViewData["SecurityGroups"] = (object) securityGroups;
            ((dynamic)base.ViewBag).IsGroupPermission = isGroupPermission;
            ((dynamic)base.ViewBag).GroupType = groupType;
            ActionResult actionResult = this.PartialView("PermissionEditor", newPermission);
            newPermission = null;
            userGroups = null;
            securityGroups = null;
            return actionResult;
        }

        public async Task<ActionResult> EditPermission(DdocPermission permission, int isGroupPermission, GroupType groupType)
        {
            ViewDataDictionary viewData = base.ViewData;
            List<DdocGroup> ddocGroups = await base.Ddoc.GetDdocGroups(GroupType.UserGroup);
            viewData["UserGroups"] = ddocGroups;
            viewData = null;
            viewData = base.ViewData;
            List<DdocGroup> ddocGroups1 = await base.Ddoc.GetDdocGroups(GroupType.SecurityGroup);
            viewData["SecurityGroups"] = ddocGroups1;
            viewData = null;
            ((dynamic)base.ViewBag).IsGroupPermission = isGroupPermission;
            ((dynamic)base.ViewBag).GroupType = groupType;
            return this.PartialView("PermissionEditor", permission);
        }

        [NoCache]
    public async Task<ActionResult> GetDdocGroups(GroupType groupType)
    {
      AdminController adminController = this;
      Response<DdocGroup> response1 = new Response<DdocGroup>();
      try
      {
        Response<DdocGroup> response2 = response1;
        response2.List = await adminController.Ddoc.GetDdocGroups(groupType);
        response2 = (Response<DdocGroup>) null;
        response1.Result = RequestResult.Success;
      }
      catch (Exception ex)
      {
        response1.Message = ex.Message;
      }
      ActionResult actionResult = (ActionResult) adminController.Json((object) response1, JsonRequestBehavior.AllowGet);
      response1 = (Response<DdocGroup>) null;
      return actionResult;
    }

    [HttpPost]
    public async Task<ActionResult> SaveDdocGroup(DdocGroup group)
    {
      AdminController adminController = this;
      Response response1 = new Response();
      try
      {
        Response response2 = response1;
        response2.Code = await adminController.Ddoc.SaveDdocGroup(group);
        response2 = (Response) null;
        response1.Result = RequestResult.Success;
      }
      catch (Exception ex)
      {
        response1.Message = ex.Message;
      }
      ActionResult actionResult = (ActionResult) adminController.Json((object) response1);
      response1 = (Response) null;
      return actionResult;
    }

    [HttpPost]
    public async Task<ActionResult> DeleteDdocGroup(
      int groupId,
      GroupType groupType)
    {
      AdminController adminController = this;
      Response<DdocGroup> response = new Response<DdocGroup>();
      try
      {
        await adminController.Ddoc.DeleteDdocGroup(groupId, groupType);
        response.Result = RequestResult.Success;
      }
      catch (Exception ex)
      {
        response.Message = ex.Message;
      }
      ActionResult actionResult = (ActionResult) adminController.Json((object) response);
      response = (Response<DdocGroup>) null;
      return actionResult;
    }

    [NoCache]
    public async Task<ActionResult> GetDdocGroupPermissions(
      int groupId,
      GroupType groupType)
    {
      AdminController adminController = this;
      Response<DdocPermission> response1 = new Response<DdocPermission>();
      try
      {
        Response<DdocPermission> response2 = response1;
        response2.List = await adminController.Ddoc.GetDdocGroupPermissions(groupId, groupType);
        response2 = (Response<DdocPermission>) null;
        response1.Result = RequestResult.Success;
      }
      catch (Exception ex)
      {
        response1.Message = ex.Message;
      }
      ActionResult actionResult = (ActionResult) adminController.Json((object) response1, JsonRequestBehavior.AllowGet);
      response1 = (Response<DdocPermission>) null;
      return actionResult;
    }

    [NoCache]
    public async Task<ActionResult> GetAllPermissions()
    {
      AdminController adminController = this;
      Response<DdocPermission> response1 = new Response<DdocPermission>();
      try
      {
        Response<DdocPermission> response2 = response1;
        response2.List = await adminController.Ddoc.GetAllPermissions();
        response2 = (Response<DdocPermission>) null;
        response1.Result = RequestResult.Success;
      }
      catch (Exception ex)
      {
        response1.Message = ex.Message;
      }
      ActionResult actionResult = (ActionResult) adminController.Json((object) response1, JsonRequestBehavior.AllowGet);
      response1 = (Response<DdocPermission>) null;
      return actionResult;
    }

    [HttpPost]
    public async Task<ActionResult> SavePermission(DdocPermission permission)
    {
      AdminController adminController = this;
      Response<DdocPermission> response1 = new Response<DdocPermission>();
      try
      {
        Response<DdocPermission> response2 = response1;
        response2.Code = await adminController.Ddoc.SavePermission(permission);
        response2 = (Response<DdocPermission>) null;
        response1.ObjectResult = permission;
        response1.Result = RequestResult.Success;
      }
      catch (Exception ex)
      {
        response1.Message = ex.Message;
      }
      ActionResult actionResult = (ActionResult) adminController.Json((object) response1);
      response1 = (Response<DdocPermission>) null;
      return actionResult;
    }

    [HttpPost]
    public async Task<ActionResult> DeletePermission(int permissionId)
    {
      AdminController adminController = this;
      Response<DdocPermission> response = new Response<DdocPermission>();
      try
      {
        await adminController.Ddoc.DeletePermission(permissionId);
        response.Result = RequestResult.Success;
      }
      catch (Exception ex)
      {
        response.Message = ex.Message;
      }
      ActionResult actionResult = (ActionResult) adminController.Json((object) response);
      response = (Response<DdocPermission>) null;
      return actionResult;
    }

    public PartialViewResult UserManagement() => this.PartialView();

    [NoCache]
    public async Task<ActionResult> EditPasswordConfiguration()
    {
      AdminController adminController = this;
      DdocConfiguration configuration = await adminController.Ddoc.GetConfiguration();
      return (ActionResult) adminController.PartialView("PasswordConfigurationEditor", (object) configuration);
    }

    [HttpPost]
    public async Task<ActionResult> SaveConfiguration(
      DdocConfiguration configuration)
    {
      AdminController adminController = this;
      Response<DdocConfiguration> response1 = new Response<DdocConfiguration>();
      try
      {
        Response<DdocConfiguration> response2 = response1;
        response2.Result = (RequestResult) await adminController.Ddoc.SaveConfiguration(configuration);
        response2 = (Response<DdocConfiguration>) null;
      }
      catch (Exception ex)
      {
        response1.Message = ex.Message;
      }
      ActionResult actionResult = (ActionResult) adminController.Json((object) response1);
      response1 = (Response<DdocConfiguration>) null;
      return actionResult;
    }

    public async Task<ActionResult> NewUser()
    {
      AdminController adminController = this;
      User newUser = new User() { IsNew = true };
      ViewDataDictionary viewDataDictionary = adminController.ViewData;
      viewDataDictionary["PasswordConfig"] = (object) await adminController.Ddoc.GetConfiguration();
      viewDataDictionary = (ViewDataDictionary) null;
      ActionResult actionResult = (ActionResult) adminController.PartialView("UserEditor", (object) newUser);
      newUser = (User) null;
      return actionResult;
    }

    [HttpPost]
    public PartialViewResult EditUser(User user) => this.PartialView("UserEditor", (object) user);

    [HttpPost]
    public PartialViewResult EditUserProfile(User user) => this.PartialView("ProfileEditor", (object) user);

    [HttpPost]
    [OverrideAuthorization]
    [UserAuthorization(new string[] {"User"})]
    public async Task<ActionResult> ChangeUserPassword(bool? bypassCurrentPassword)
    {
      AdminController adminController = this;
      ViewDataDictionary viewDataDictionary = adminController.ViewData;
      viewDataDictionary["PasswordConfig"] = (object) await adminController.Ddoc.GetConfiguration();
      viewDataDictionary = (ViewDataDictionary) null;
      adminController.ViewData["BypassCurrentPassword"] = (object) bypassCurrentPassword;
      return (ActionResult) adminController.PartialView("PasswordEditor");
    }

    [NoCache]
    public async Task<ActionResult> GetUsers()
    {
      AdminController adminController = this;
      Response<User> response1 = new Response<User>();
      try
      {
        Response<User> response2 = response1;
        response2.List = await adminController.Ddoc.GetUsers();
        response2 = (Response<User>) null;
        response1.Result = RequestResult.Success;
      }
      catch (Exception ex)
      {
        response1.Message = ex.Message;
      }
      ActionResult actionResult = (ActionResult) adminController.Json((object) response1, JsonRequestBehavior.AllowGet);
      response1 = (Response<User>) null;
      return actionResult;
    }

    [HttpPost]
    public async Task<ActionResult> GetUserProfile(string username)
    {
      AdminController adminController = this;
      Response<DdocGroup> response1 = new Response<DdocGroup>();
      try
      {
        Response<DdocGroup> response2 = response1;
        response2.List = await adminController.Ddoc.GetUserProfile(username);
        response2 = (Response<DdocGroup>) null;
        response1.Result = RequestResult.Success;
      }
      catch (Exception ex)
      {
        response1.Message = ex.Message;
      }
      ActionResult actionResult = (ActionResult) adminController.Json((object) response1);
      response1 = (Response<DdocGroup>) null;
      return actionResult;
    }

    [HttpPost]
    public async Task<ActionResult> SaveUser(User user)
    {
      AdminController adminController = this;
      Response<User> response1 = new Response<User>();
      try
      {
        Response<User> response2 = response1;
        response2.Code = await adminController.Ddoc.SaveUser(user);
        response2 = (Response<User>) null;
        response1.ObjectResult = user;
        response1.Result = RequestResult.Success;
      }
      catch (Exception ex)
      {
        response1.Message = ex.Message;
      }
      ActionResult actionResult = (ActionResult) adminController.Json((object) response1);
      response1 = (Response<User>) null;
      return actionResult;
    }

    [HttpPost]
    public async Task<ActionResult> UpdateUserProfile(User user)
    {
      AdminController adminController = this;
      Response<User> response = new Response<User>();
      try
      {
        await adminController.Ddoc.UpdateUserProfile(user);
        response.ObjectResult = user;
        response.Result = RequestResult.Success;
      }
      catch (Exception ex)
      {
        response.Message = ex.Message;
      }
      ActionResult actionResult = (ActionResult) adminController.Json((object) response);
      response = (Response<User>) null;
      return actionResult;
    }

    [HttpPost]
    [OverrideAuthorization]
    [UserAuthorization(new string[] {"User"})]
    public async Task<ActionResult> UpdateUserPassword(User user)
    {
      AdminController adminController = this;
      Response<User> response1 = new Response<User>();
      try
      {
        Response<User> response2 = response1;
        response2.Code = await adminController.Ddoc.UpdateUserPassword(user);
        response2 = (Response<User>) null;
        response1.ObjectResult = user;
        response1.Result = RequestResult.Success;
      }
      catch (Exception ex)
      {
        response1.Message = ex.Message;
      }
      ActionResult actionResult = (ActionResult) adminController.Json((object) response1);
      response1 = (Response<User>) null;
      return actionResult;
    }

    [HttpPost]
    public async Task<ActionResult> UnlockUser(string username)
    {
      AdminController adminController = this;
      Response response1 = new Response();
      try
      {
        Response response2 = response1;
        response2.Code = await adminController.Ddoc.UnlockUser(username);
        response2 = (Response) null;
        response1.Result = RequestResult.Success;
      }
      catch (Exception ex)
      {
        response1.Message = ex.Message;
      }
      ActionResult actionResult = (ActionResult) adminController.Json((object) response1);
      response1 = (Response) null;
      return actionResult;
    }

    [HttpPost]
    public async Task<ActionResult> DeleteUser(string username)
    {
      AdminController adminController = this;
      Response response = new Response();
      try
      {
        await adminController.Ddoc.DeleteUser(username);
        response.Result = RequestResult.Success;
      }
      catch (Exception ex)
      {
        response.Message = ex.Message;
      }
      ActionResult actionResult = (ActionResult) adminController.Json((object) response);
      response = (Response) null;
      return actionResult;
    }

    public PartialViewResult StorageManagement() => this.PartialView();

    public PartialViewResult NewFileServer() => this.PartialView("FileServerEditor", (object) new DdocFileServer()
    {
      IsNew = true
    });

    public PartialViewResult EditFileServer(DdocFileServer server) => this.PartialView("FileServerEditor", (object) server);

    [NoCache]
    public async Task<ActionResult> GetFileServers()
    {
      AdminController adminController = this;
      Response<DdocFileServer> response1 = new Response<DdocFileServer>();
      try
      {
        Response<DdocFileServer> response2 = response1;
        response2.List = await adminController.Ddoc.GetFileServers();
        response2 = (Response<DdocFileServer>) null;
        response1.Result = RequestResult.Success;
      }
      catch (Exception ex)
      {
        response1.Message = ex.Message;
      }
      ActionResult actionResult = (ActionResult) adminController.Json((object) response1, JsonRequestBehavior.AllowGet);
      response1 = (Response<DdocFileServer>) null;
      return actionResult;
    }

    [HttpPost]
    public async Task<ActionResult> SaveFileServer(DdocFileServer server)
    {
      AdminController adminController = this;
      Response<DdocFileServer> response1 = new Response<DdocFileServer>();
      try
      {
        Response<DdocFileServer> response2 = response1;
        response2.Code = await adminController.Ddoc.SaveFileServer(server);
        response2 = (Response<DdocFileServer>) null;
        response1.ObjectResult = server;
        response1.Result = RequestResult.Success;
      }
      catch (Exception ex)
      {
        response1.Message = ex.Message;
      }
      ActionResult actionResult = (ActionResult) adminController.Json((object) response1);
      response1 = (Response<DdocFileServer>) null;
      return actionResult;
    }

    public async Task<ActionResult> DeleteFileServer(string fileServerId)
    {
      AdminController adminController = this;
      Response<DdocFileServer> response = new Response<DdocFileServer>();
      try
      {
        await adminController.Ddoc.DeleteFileServer(fileServerId);
        response.Result = RequestResult.Success;
      }
      catch (Exception ex)
      {
        response.Message = ex.Message;
      }
      ActionResult actionResult = (ActionResult) adminController.Json((object) response);
      response = (Response<DdocFileServer>) null;
      return actionResult;
    }

    public async Task<ActionResult> NewWarehouse()
    {
      AdminController adminController = this;
      DdocWarehouse newWarehouse = new DdocWarehouse()
      {
        IsNew = true
      };
      ViewDataDictionary viewDataDictionary = adminController.ViewData;
      viewDataDictionary["FileServers"] = (object) await adminController.Ddoc.GetFileServers();
      viewDataDictionary = (ViewDataDictionary) null;
      ActionResult actionResult = (ActionResult) adminController.PartialView("WarehouseEditor", (object) newWarehouse);
      newWarehouse = (DdocWarehouse) null;
      return actionResult;
    }

    public async Task<ActionResult> EditWarehouse(DdocWarehouse warehouse)
    {
      AdminController adminController = this;
      ViewDataDictionary viewDataDictionary = adminController.ViewData;
      viewDataDictionary["FileServers"] = (object) await adminController.Ddoc.GetFileServers();
      viewDataDictionary = (ViewDataDictionary) null;
      return (ActionResult) adminController.PartialView("WarehouseEditor", (object) warehouse);
    }

    public PartialViewResult EditWarehousePaths() => this.PartialView("WarehousePathsEditor");

    public PartialViewResult NewPath() => this.PartialView("PathEditor", (object) new DdocPath()
    {
      IsNew = true
    });

    public PartialViewResult EditPath(DdocPath path) => this.PartialView("PathEditor", (object) path);

    [NoCache]
    public async Task<ActionResult> GetWarehouses()
    {
      AdminController adminController = this;
      Response<DdocWarehouse> response1 = new Response<DdocWarehouse>();
      try
      {
        Response<DdocWarehouse> response2 = response1;
        response2.List = await adminController.Ddoc.GetWarehouses();
        response2 = (Response<DdocWarehouse>) null;
        response1.Result = RequestResult.Success;
      }
      catch (Exception ex)
      {
        response1.Message = ex.Message;
      }
      ActionResult actionResult = (ActionResult) adminController.Json((object) response1, JsonRequestBehavior.AllowGet);
      response1 = (Response<DdocWarehouse>) null;
      return actionResult;
    }

    [NoCache]
    public async Task<ActionResult> GetWarehousePaths(int warehouseId)
    {
      AdminController adminController = this;
      Response<DdocPath> response1 = new Response<DdocPath>();
      try
      {
        Response<DdocPath> response2 = response1;
        response2.List = await adminController.Ddoc.GetWarehousePaths(warehouseId);
        response2 = (Response<DdocPath>) null;
        response1.Result = RequestResult.Success;
      }
      catch (Exception ex)
      {
        response1.Message = ex.Message;
      }
      ActionResult actionResult = (ActionResult) adminController.Json((object) response1, JsonRequestBehavior.AllowGet);
      response1 = (Response<DdocPath>) null;
      return actionResult;
    }

    [HttpPost]
    public async Task<ActionResult> SaveWarehouse(DdocWarehouse warehouse)
    {
      AdminController adminController = this;
      Response<DdocWarehouse> response1 = new Response<DdocWarehouse>();
      try
      {
        Response<DdocWarehouse> response2 = response1;
        response2.Code = await adminController.Ddoc.SaveWarehouse(warehouse);
        response2 = (Response<DdocWarehouse>) null;
        response1.ObjectResult = warehouse;
        response1.Result = RequestResult.Success;
      }
      catch (Exception ex)
      {
        response1.Message = ex.Message;
      }
      ActionResult actionResult = (ActionResult) adminController.Json((object) response1);
      response1 = (Response<DdocWarehouse>) null;
      return actionResult;
    }

    [HttpPost]
    public async Task<ActionResult> SaveWarehousePath(
      int warehouseId,
      DdocPath path)
    {
      AdminController adminController = this;
      Response<DdocPath> response1 = new Response<DdocPath>();
      try
      {
        Response<DdocPath> response2 = response1;
        response2.Code = await adminController.Ddoc.SaveWarehousePath(warehouseId, path);
        response2 = (Response<DdocPath>) null;
        response1.ObjectResult = path;
        response1.Result = RequestResult.Success;
      }
      catch (Exception ex)
      {
        response1.Message = ex.Message;
      }
      ActionResult actionResult = (ActionResult) adminController.Json((object) response1);
      response1 = (Response<DdocPath>) null;
      return actionResult;
    }

    [HttpPost]
    public async Task<ActionResult> DeleteWarehousePath(int pathId)
    {
      AdminController adminController = this;
      Response response = new Response();
      try
      {
        int num = await adminController.Ddoc.DeleteWarehousePath(pathId);
        response.Result = RequestResult.Success;
      }
      catch (Exception ex)
      {
        response.Message = ex.Message;
      }
      ActionResult actionResult = (ActionResult) adminController.Json((object) response);
      response = (Response) null;
      return actionResult;
    }

    [HttpPost]
    public async Task<ActionResult> SetWarehouseActivePath(
      int warehouseId,
      int pathId)
    {
      AdminController adminController = this;
      Response response = new Response();
      try
      {
        int num = await adminController.Ddoc.SetWarehouseActivePath(warehouseId, pathId);
        response.Result = RequestResult.Success;
      }
      catch (Exception ex)
      {
        response.Message = ex.Message;
      }
      ActionResult actionResult = (ActionResult) adminController.Json((object) response);
      response = (Response) null;
      return actionResult;
    }

    [HttpPost]
    public async Task<ActionResult> DeleteWarehouse(int warehouseId)
    {
      AdminController adminController = this;
      Response response = new Response();
      try
      {
        await adminController.Ddoc.DeleteWarehouse(warehouseId);
        response.Result = RequestResult.Success;
      }
      catch (Exception ex)
      {
        response.Message = ex.Message;
      }
      ActionResult actionResult = (ActionResult) adminController.Json((object) response);
      response = (Response) null;
      return actionResult;
    }
  }
}
