// Decompiled with JetBrains decompiler
// Type: DigitalData.DDoc.UI.Web.Controllers.FolderController
// Assembly: DigitalData.DDoc.UI.Web, Version=4.210.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1482FEFA-CA2F-44DF-981C-51EA2DF7CBB5
// Assembly location: C:\Users\herna\Downloads\ss\BradesCard_DEV\bin\DigitalData.DDoc.UI.Web.dll

using DigitalData.Common.Entities;
using DigitalData.Common.WebUtils.AspNet4;
using DigitalData.DDoc.Common.Api;
using DigitalData.DDoc.Common.Entities;
using DigitalData.DDoc.Common.Entities.Helpers;
using DigitalData.DDoc.Common.Extensions;
using DigitalData.DDoc.Common.WebExtensions;
using Microsoft.CSharp.RuntimeBinder;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace DigitalData.DDoc.UI.Web.Controllers
{
    public class FolderController : DdocController
    {
        private readonly FolderController.FolderPreProcessorFunction FolderPreProcessor;

        public FolderController(
          IDdocService ddoc,
          IOptions<DiDaSettings> settings,
          IFolderPreProcessor folderPreProcessor)
          : base(ddoc, settings.Value)
        {
            if (folderPreProcessor.PreProcessFolders == null)
                return;
            this.FolderPreProcessor = new FolderController.FolderPreProcessorFunction(folderPreProcessor.PreProcessFolders.Invoke);
        }

        [HttpPost]
        public async Task<ActionResult> Save(DdocFolder folder)
        {
            FolderController folderController = this;
            Response response1 = new Response();
            Response response2 = response1;
            response2.TextResult = await folderController.Ddoc.SaveFolder(folder);
            Response response3 = response1;
            response2 = (Response)null;
            response1 = (Response)null;
            if (folder.IsNew)
                await folderController.Ddoc.ActivityLog(new DdocActionLogEntry()
                {
                    Action = "NewFolder",
                    User = folderController.CurrentUserSession.Username,
                    Module = folderController.ModuleName,
                    Details = response3.TextResult
                });
            else
                await folderController.Ddoc.ActivityLog(new DdocActionLogEntry()
                {
                    Action = "UpdateFolder",
                    User = folderController.CurrentUserSession.Username,
                    Module = folderController.ModuleName,
                    Details = folder.Id
                });
            response3.Result = RequestResult.Success;
            ActionResult actionResult = (ActionResult)folderController.Json((object)response3);
            response3 = (Response)null;
            return actionResult;
        }

        [HttpPost]
        public async Task<ActionResult> Delete(string folderId)
        {
            FolderController folderController = this;
            Response response1 = new Response();
            Response response2 = response1;
            response2.Code = await folderController.Ddoc.DeleteFolder(folderId, folderController.CurrentUserSession.Username);
            Response response3 = response1;
            response2 = (Response)null;
            response1 = (Response)null;
            await folderController.Ddoc.ActivityLog(new DdocActionLogEntry()
            {
                Action = "DeleteFolder",
                User = folderController.CurrentUserSession.Username,
                Module = folderController.ModuleName,
                Details = folderId
            });
            response3.Result = RequestResult.Success;
            ActionResult actionResult = (ActionResult)folderController.Json((object)response3);
            response3 = (Response)null;
            return actionResult;
        }

        public async Task<ActionResult> New(string collectionId)
        {
            DdocCollection collection = await base.Ddoc.GetCollection(collectionId);
            List<DdocField> ddocFields = await base.Ddoc.GetCollectionFields(collectionId);
            DdocFolder ddocFolder = new DdocFolder()
            {
                CollectionId = collectionId,
                Name = collection.Name,
                SecurityGroupId = collection.SecurityGroupId,
                Data = ddocFields,
                IsNew = true
            };
            DdocFolder ddocFolder1 = ddocFolder;
            ((dynamic)base.ViewBag).CollectionName = collection.Name;
            ActionResult actionResult = this.PartialView("FolderEditor", ddocFolder1);
            collection = null;
            return actionResult;
        }


        [HttpPost]
        public async Task<ActionResult> Edit(string folderId)
        {
            DdocFolder folder = await base.Ddoc.GetFolder(folderId);
            dynamic viewBag = base.ViewBag;
            DdocCollection collection = await base.Ddoc.GetCollection(folder.CollectionId);
            viewBag.CollectionName = collection.Name;
            viewBag = null;
            viewBag = base.ViewBag;
            IDdocService ddoc = base.Ddoc;
            string id = folder.Id;
            GroupFilters groupFilter = new GroupFilters()
            {
                UserGroupList = base.CurrentUserSession.UserGroups,
                DdocGroupList = base.CurrentUserSession.DdocGroups
            };
            Permissions elementPermissions = await ddoc.GetElementPermissions(CollectionType.F, id, groupFilter);
            viewBag.SecurityPermissions = elementPermissions;
            viewBag = null;
            ActionResult actionResult = this.PartialView("FolderEditor", folder);
            folder = null;
            return actionResult;
        }

        [NoCache]
        public async Task<ActionResult> GetFolder(string folderId)
        {
            FolderController folderController = this;
            Response<DdocFolder> response1 = new Response<DdocFolder>();
            try
            {
                Response<DdocFolder> response2 = response1;
                response2.ObjectResult = await folderController.Ddoc.GetFolder(folderId);
                response2 = (Response<DdocFolder>)null;
                response1.Result = RequestResult.Success;
            }
            catch (Exception ex)
            {
                response1.Message = ex.Message;
            }
            ActionResult actionResult = (ActionResult)folderController.Json((object)response1, JsonRequestBehavior.AllowGet);
            response1 = (Response<DdocFolder>)null;
            return actionResult;
        }

        [NoCache]
        public async Task<ActionResult> CountFolders(string collectionId)
        {
            FolderController folderController = this;
            Response<DdocFolder> response = new Response<DdocFolder>();
            try
            {
                response.Total = (long)await folderController.Ddoc.CountFolders(collectionId, folderController.CurrentUserSession.DdocGroups);
                response.Result = RequestResult.Success;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            ActionResult actionResult = (ActionResult)folderController.Json((object)response, JsonRequestBehavior.AllowGet);
            response = (Response<DdocFolder>)null;
            return actionResult;
        }

        [NoCache]
        public async Task<ActionResult> GetFolders(
          string collectionId,
          int page = 1,
          int pageSize = 17,
          string sortBy = null,
          int sortDirection = 0)
        {
            FolderController folderController = this;
            Response<DdocFolder> response = new Response<DdocFolder>();
            try
            {
                List<DdocFolder> folders = await folderController.Ddoc.GetFolders(collectionId, folderController.CurrentUserSession.DdocGroups, page, pageSize, sortBy, sortDirection);
                if (folderController.FolderPreProcessor != null)
                    folders = await folderController.FolderPreProcessor(folders);
                response.List = folders;
                response.Result = RequestResult.Success;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            ActionResult actionResult = (ActionResult)folderController.Json((object)response, JsonRequestBehavior.AllowGet);
            response = (Response<DdocFolder>)null;
            return actionResult;
        }

        private delegate Task<List<DdocFolder>> FolderPreProcessorFunction(
          List<DdocFolder> folders);
    }
}
