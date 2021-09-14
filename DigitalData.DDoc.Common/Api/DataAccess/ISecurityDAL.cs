// Decompiled with JetBrains decompiler
// Type: DigitalData.DDoc.Common.Api.DataAccess.ISecurityDAL
// Assembly: DigitalData.DDoc.Common, Version=4.210.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 37D7F3E1-2CA5-48B0-AB22-FE81D67009C0
// Assembly location: C:\Users\herna\Downloads\ss\BradesCard_DEV\bin\DigitalData.DDoc.Common.dll

using DigitalData.DDoc.Common.Entities;
using DigitalData.DDoc.Common.Entities.Security;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DigitalData.DDoc.Common.Api.DataAccess
{
  public interface ISecurityDAL : ICommonDAL
  {
    Task DisableLogon(string user);

    Task<DdocPermission> GetPermission(int permissionId);

    Task<DdocPermission> GetCollectionEffectivePermissions(
      string collectionId,
      string userGroupList,
      string ddocGroupList = null);

    Task<DdocPermission> GetDocumentEffectivePermissions(
      string documentId,
      string userGroupList,
      string ddocGroupList = null);

    Task<DdocPermission> GetFolderEffectivePermissions(
      string folderId,
      string userGroupList,
      string ddocGroupList = null);

    Task<DdocLoginConfig> GetLoginConfig();

    Task<IEnumerable<DdocGroup>> GetSecurityGroups(string securityGroupsCsv = null);

    Task<string> GetSecurityGroupsByUserGroups(string groupIdList);

    Task<Permissions> GetUserGroupPermissions(List<DdocGroup> groups);

    Task<DdocLoginData> GetUserLoginData(string username);

    Task<bool> UserExists(string username);

    Task<IEnumerable<DdocGroup>> GetUserProfile(string userName);

    Task<int> IncrementFailedLogon(string user);

    Task SetLogonInfo(string user);
  }
}
