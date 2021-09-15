// Decompiled with JetBrains decompiler
// Type: DigitalData.DDoc.Common.Api.DataAccess.IAdminDAL
// Assembly: DigitalData.DDoc.Common, Version=4.210.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 37D7F3E1-2CA5-48B0-AB22-FE81D67009C0
// Assembly location: C:\Users\herna\Downloads\ss\BradesCard_DEV\bin\DigitalData.DDoc.Common.dll

using DigitalData.Open.Common.Entities;
using DigitalData.Open.Common.Entities.Security;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DigitalData.Open.Common.Api.DataAccess
{
  public interface IAdminDAL : ICommonDAL
  {
    Task<int> AddWarehousePath(int warehouseId, DdocPath ddocPath);

    Task<bool> FileServerExists(string fileServerId);

    Task<string> CreateFileServer(DdocFileServer fileServer);

    Task<int> CreatePermission(DdocPermission permission);

    Task<int> CreateSecurityGroup(DdocGroup securityGroup);

    Task<int> CreateUser(User user);

    Task<int> CreateUserGroup(DdocGroup userGroup);

    Task<bool> WarehouseExists(int warehouseId);

    Task<bool> WarehousePathExists(int warehousePathId);

    Task<int> CreateWarehouse(DdocWarehouse warehouse);

    Task DeleteFileServer(string fileServerId);

    Task DeletePermission(int permissionId);

    Task DeleteSecurityGroup(int securityGroupId);

    Task DeleteUser(string username);

    Task DeleteUserGroup(int userGroupId);

    Task DeleteWarehouse(int warehouseId);

    Task<int> DeleteWarehousePath(int pathId);

    Task<IEnumerable<DdocPermission>> GetAllPermissions();

    Task<DdocConfiguration> GetConfiguration();

    Task<IEnumerable<DdocFileServer>> GetFileServers();

    Task<IEnumerable<DdocPermission>> GetSecurityGroupPermissions(
      int groupId);

    Task<DdocGroup> GetSecurityGroup(int groupId);

    Task<IEnumerable<DdocGroup>> GetSecurityGroups();

    Task<IEnumerable<DdocPermission>> GetUserGroupPermissions(
      int groupId);

    Task<DdocGroup> GetUserGroup(int groupId);

    Task<IEnumerable<DdocGroup>> GetUserGroups();

    Task<IEnumerable<DdocGroup>> GetUserProfile(string username);

    Task<User> GetUser(string username);

    Task<IEnumerable<User>> GetUsers();

    Task<DdocWarehouse> GetWarehouse(int warehouseId);

    Task<IEnumerable<DdocPath>> GetWarehousePaths(int warehouseId);

    Task<IEnumerable<DdocWarehouse>> GetWarehouses();

    Task<int> SaveConfiguration(DdocConfiguration configuration);

    Task<int> SetWarehouseActivePath(int warehouseId, int pathId);

    Task<int> UnlockUser(string username);

    Task<int> UpdateFileServer(DdocFileServer fileServer);

    Task<int> UpdatePassword(string username, string password);

    Task<int> UpdatePermission(DdocPermission permission);

    Task<int> UpdateSecurityGroup(DdocGroup securityGroup);

    Task<int> UpdateUser(User user);

    Task<int> UpdateUserGroup(DdocGroup userGroup);

    Task UpdateUserProfile(string username, string userGroupsCsv);

    Task<int> UpdateWarehouse(DdocWarehouse warehouse);

    Task<int> UpdateWarehousePath(DdocPath path);
  }
}
