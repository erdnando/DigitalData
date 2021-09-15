
using DigitalData.Open.Common.Entities;
using DigitalData.Open.Common.Entities.Security;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DigitalData.Open.Common.Api.DataAccess
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
