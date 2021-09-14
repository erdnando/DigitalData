// Decompiled with JetBrains decompiler
// Type: DigitalData.DDoc.Modules.DataAccess.AdminDAL
// Assembly: DigitalData.DDoc.Modules.DataAccess.SqlServer, Version=4.210.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EB5ECC0D-3654-4980-9D29-200BC39CB926
// Assembly location: D:\proyectos\ddocx\DigitalData.DDoc.UI.Webx\DigitalData.DDoc.UI.Web\bin\DigitalData.DDoc.Modules.DataAccess.SqlServer.dll

using Dapper;
using DigitalData.Common.DataAccess;
using DigitalData.Common.DataAccess.QueryExecution;
using DigitalData.Common.DataAccess.SqlServer;
using DigitalData.DDoc.Common.Api.DataAccess;
using DigitalData.DDoc.Common.Entities;
using DigitalData.DDoc.Common.Entities.Security;
using SqlKata;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace DigitalData.DDoc.Modules.DataAccess
{
  public class AdminDAL : SqlServerDAL, IAdminDAL, ICommonDAL
  {
    public AdminDAL(SqlConnection connection, SqlTransaction transaction = null)
      : base(connection, transaction)
    {
    }

        public Task<DdocGroup> GetUserGroup(int groupId)
        {
            return QueryExtensionsAsync.SingleOrDefaultAsync<DdocGroup>(base.db.Query("G_GRUPONT").Where("GGNT_ID", groupId).Select(new String[] { "GGNT_ID as Id", "NOMBRE as Name" }), null);
        }

        public Task<IEnumerable<DdocGroup>> GetUserGroups()
    {
      SqlConnection connection = ((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Connection;
      IDbTransaction transaction = ((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Transaction;
      CommandType? nullable1 = new CommandType?(CommandType.StoredProcedure);
      int? nullable2 = new int?();
      CommandType? nullable3 = nullable1;
      return (Task<IEnumerable<DdocGroup>>) SqlMapper.QueryAsync<DdocGroup>((IDbConnection) connection, "[ddocAdmin].[sp_GetUserGroups]", (object) null, transaction, nullable2, nullable3);
    }

    public async Task<int> CreateUserGroup(DdocGroup userGroup)
    {
      DynamicParameters parameters = new DynamicParameters();
      DynamicParameters dynamicParameters1 = parameters;
      string name = userGroup.Name;
      DbType? nullable1 = new DbType?(DbType.String);
      int? nullable2 = new int?(30);
      ParameterDirection? nullable3 = new ParameterDirection?();
      int? nullable4 = nullable2;
      byte? nullable5 = new byte?();
      byte? nullable6 = new byte?();
      dynamicParameters1.Add("@name", (object) name, nullable1, nullable3, nullable4, nullable5, nullable6);
      parameters.Add("@userGroupId", (object) null, new DbType?(DbType.Int32), new ParameterDirection?(ParameterDirection.Output), new int?(), new byte?(), new byte?());
      SqlConnection connection = ((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Connection;
      DynamicParameters dynamicParameters2 = parameters;
      IDbTransaction transaction = ((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Transaction;
      CommandType? nullable7 = new CommandType?(CommandType.StoredProcedure);
      int? nullable8 = new int?();
      CommandType? nullable9 = nullable7;
      int num1 = await SqlMapper.ExecuteAsync((IDbConnection) connection, "[ddocAdmin].[sp_CreateUserGroup]", (object) dynamicParameters2, transaction, nullable8, nullable9);
      int num2 = (int) parameters.Get<int>("@userGroupId");
      parameters = (DynamicParameters) null;
      return num2;
    }

        public Task<int> UpdateUserGroup(DdocGroup userGroup)
        {
            DynamicParameters dynamicParameter = new DynamicParameters();
            ParameterDirection? nullable = null;
            int? nullable1 = null;
            byte? nullable2 = null;
            byte? nullable3 = nullable2;
            nullable2 = null;
            dynamicParameter.Add("@userGroupId", userGroup.Id, new DbType?(DbType.Int32), nullable, nullable1, nullable3, nullable2);
            string name = userGroup.Name;
            DbType? nullable4 = new DbType?(DbType.String);
            nullable1 = new int?(30);
            nullable = null;
            nullable2 = null;
            byte? nullable5 = nullable2;
            nullable2 = null;
            dynamicParameter.Add("@name", name, nullable4, nullable, nullable1, nullable5, nullable2);
            SqlConnection connection = base.Connection;
            IDbTransaction transaction = base.Transaction;
            CommandType? nullable6 = new CommandType?(CommandType.StoredProcedure);
            nullable1 = null;
            Task<int> task = connection.ExecuteAsync("[ddocAdmin].[sp_UpdateUserGroup]", dynamicParameter, transaction, nullable1, nullable6);
            return task;
        }

        public Task DeleteUserGroup(int userGroupId)
    {
      DynamicParameters dynamicParameters1 = new DynamicParameters();
      dynamicParameters1.Add("@userGroupId", (object) userGroupId, new DbType?(DbType.Int32), new ParameterDirection?(), new int?(), new byte?(), new byte?());
      SqlConnection connection = ((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Connection;
      DynamicParameters dynamicParameters2 = dynamicParameters1;
      IDbTransaction transaction = ((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Transaction;
      CommandType? nullable1 = new CommandType?(CommandType.StoredProcedure);
      int? nullable2 = new int?();
      CommandType? nullable3 = nullable1;
      return (Task) SqlMapper.ExecuteAsync((IDbConnection) connection, "[ddocAdmin].[sp_DeleteUserGroup]", (object) dynamicParameters2, transaction, nullable2, nullable3);
    }

        public Task<DdocGroup> GetSecurityGroup(int groupId)
        {
            return QueryExtensionsAsync.SingleOrDefaultAsync<DdocGroup>(base.db.Query("G_SEGURIDAD").Where("GS_ID", groupId).Select(new String[] { "GS_ID as Id", "NOMBRE as Name" }), null);
        }
        public Task<IEnumerable<DdocGroup>> GetSecurityGroups()
    {
      SqlConnection connection = ((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Connection;
      CommandType? nullable1 = new CommandType?(CommandType.StoredProcedure);
      int? nullable2 = new int?();
      CommandType? nullable3 = nullable1;
      return (Task<IEnumerable<DdocGroup>>) SqlMapper.QueryAsync<DdocGroup>((IDbConnection) connection, "[ddocAdmin].[sp_GetSecurityGroups]", (object) null, (IDbTransaction) null, nullable2, nullable3);
    }

    public async Task<int> CreateSecurityGroup(DdocGroup securityGroup)
    {
      DynamicParameters parameters = new DynamicParameters();
      DynamicParameters dynamicParameters1 = parameters;
      string name = securityGroup.Name;
      DbType? nullable1 = new DbType?(DbType.String);
      int? nullable2 = new int?(30);
      ParameterDirection? nullable3 = new ParameterDirection?();
      int? nullable4 = nullable2;
      byte? nullable5 = new byte?();
      byte? nullable6 = new byte?();
      dynamicParameters1.Add("@name", (object) name, nullable1, nullable3, nullable4, nullable5, nullable6);
      parameters.Add("@securityGroupId", (object) null, new DbType?(DbType.Int32), new ParameterDirection?(ParameterDirection.Output), new int?(), new byte?(), new byte?());
      SqlConnection connection = ((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Connection;
      DynamicParameters dynamicParameters2 = parameters;
      IDbTransaction transaction = ((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Transaction;
      CommandType? nullable7 = new CommandType?(CommandType.StoredProcedure);
      int? nullable8 = new int?();
      CommandType? nullable9 = nullable7;
      int num1 = await SqlMapper.ExecuteAsync((IDbConnection) connection, "[ddocAdmin].[sp_CreateSecurityGroup]", (object) dynamicParameters2, transaction, nullable8, nullable9);
      int num2 = (int) parameters.Get<int>("@securityGroupId");
      parameters = (DynamicParameters) null;
      return num2;
    }

        public Task<int> UpdateSecurityGroup(DdocGroup securityGroup)
        {
            DynamicParameters dynamicParameter = new DynamicParameters();
            ParameterDirection? nullable = null;
            int? nullable1 = null;
            byte? nullable2 = null;
            byte? nullable3 = nullable2;
            nullable2 = null;
            dynamicParameter.Add("@securityGroupId", securityGroup.Id, new DbType?(DbType.Int32), nullable, nullable1, nullable3, nullable2);
            string name = securityGroup.Name;
            DbType? nullable4 = new DbType?(DbType.String);
            nullable1 = new int?(30);
            nullable = null;
            nullable2 = null;
            byte? nullable5 = nullable2;
            nullable2 = null;
            dynamicParameter.Add("@name", name, nullable4, nullable, nullable1, nullable5, nullable2);
            SqlConnection connection = base.Connection;
            IDbTransaction transaction = base.Transaction;
            CommandType? nullable6 = new CommandType?(CommandType.StoredProcedure);
            nullable1 = null;
            Task<int> task = connection.ExecuteAsync("[ddocAdmin].[sp_UpdateSecurityGroup]", dynamicParameter, transaction, nullable1, nullable6);
            return task;
        }

        public Task DeleteSecurityGroup(int securityGroupId)
    {
      DynamicParameters dynamicParameters1 = new DynamicParameters();
      dynamicParameters1.Add("@securityGroupId", (object) securityGroupId, new DbType?(DbType.Int32), new ParameterDirection?(), new int?(), new byte?(), new byte?());
      SqlConnection connection = ((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Connection;
      DynamicParameters dynamicParameters2 = dynamicParameters1;
      IDbTransaction transaction = ((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Transaction;
      CommandType? nullable1 = new CommandType?(CommandType.StoredProcedure);
      int? nullable2 = new int?();
      CommandType? nullable3 = nullable1;
      return (Task) SqlMapper.ExecuteAsync((IDbConnection) connection, "[ddocAdmin].[sp_DeleteSecurityGroup]", (object) dynamicParameters2, transaction, nullable2, nullable3);
    }

    public Task<IEnumerable<DdocPermission>> GetSecurityGroupPermissions(
      int groupId)
    {
      DynamicParameters dynamicParameters1 = new DynamicParameters();
      dynamicParameters1.Add("@securityGroupId", (object) groupId, new DbType?(DbType.Int32), new ParameterDirection?(), new int?(), new byte?(), new byte?());
      SqlConnection connection = ((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Connection;
      DynamicParameters dynamicParameters2 = dynamicParameters1;
      IDbTransaction transaction = ((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Transaction;
      CommandType? nullable1 = new CommandType?(CommandType.StoredProcedure);
      int? nullable2 = new int?();
      CommandType? nullable3 = nullable1;
      return (Task<IEnumerable<DdocPermission>>) SqlMapper.QueryAsync<DdocPermission>((IDbConnection) connection, "[ddocAdmin].[sp_GetSecurityGroupPermissions]", (object) dynamicParameters2, transaction, nullable2, nullable3);
    }

    public Task<IEnumerable<DdocPermission>> GetUserGroupPermissions(
      int groupId)
    {
      DynamicParameters dynamicParameters1 = new DynamicParameters();
      dynamicParameters1.Add("@userGroupId", (object) groupId, new DbType?(DbType.Int32), new ParameterDirection?(), new int?(), new byte?(), new byte?());
      SqlConnection connection = ((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Connection;
      DynamicParameters dynamicParameters2 = dynamicParameters1;
      IDbTransaction transaction = ((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Transaction;
      CommandType? nullable1 = new CommandType?(CommandType.StoredProcedure);
      int? nullable2 = new int?();
      CommandType? nullable3 = nullable1;
      return (Task<IEnumerable<DdocPermission>>) SqlMapper.QueryAsync<DdocPermission>((IDbConnection) connection, "[ddocAdmin].[sp_GetUserGroupPermissions]", (object) dynamicParameters2, transaction, nullable2, nullable3);
    }

    public Task<IEnumerable<DdocPermission>> GetAllPermissions()
    {
      SqlConnection connection = ((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Connection;
      CommandType? nullable1 = new CommandType?(CommandType.StoredProcedure);
      int? nullable2 = new int?();
      CommandType? nullable3 = nullable1;
      return (Task<IEnumerable<DdocPermission>>) SqlMapper.QueryAsync<DdocPermission>((IDbConnection) connection, "[ddocAdmin].[sp_GetAllPermissions]", (object) null, (IDbTransaction) null, nullable2, nullable3);
    }

    public async Task<int> CreatePermission(DdocPermission permission)
    {
      DynamicParameters parameters = new DynamicParameters();
      parameters.Add("@securityGroupId", (object) permission.SecurityGroupId, new DbType?(DbType.Int32), new ParameterDirection?(), new int?(), new byte?(), new byte?());
      parameters.Add("@userGroupId", (object) permission.UserGroupId, new DbType?(DbType.Int32), new ParameterDirection?(), new int?(), new byte?(), new byte?());
      parameters.Add("@writePermission", (object) permission.WritePermission, new DbType?(DbType.Boolean), new ParameterDirection?(), new int?(), new byte?(), new byte?());
      parameters.Add("@commentPermission", (object) permission.CommentPermission, new DbType?(DbType.Boolean), new ParameterDirection?(), new int?(), new byte?(), new byte?());
      parameters.Add("@exportPermission", (object) permission.ExportPermission, new DbType?(DbType.Boolean), new ParameterDirection?(), new int?(), new byte?(), new byte?());
      parameters.Add("@printPermission", (object) permission.PrintPermission, new DbType?(DbType.Boolean), new ParameterDirection?(), new int?(), new byte?(), new byte?());
      parameters.Add("@deletePermission", (object) permission.DeletePermission, new DbType?(DbType.Boolean), new ParameterDirection?(), new int?(), new byte?(), new byte?());
      parameters.Add("@permissionId", (object) null, new DbType?(DbType.Int32), new ParameterDirection?(ParameterDirection.Output), new int?(), new byte?(), new byte?());
      SqlConnection connection = ((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Connection;
      DynamicParameters dynamicParameters = parameters;
      IDbTransaction transaction = ((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Transaction;
      CommandType? nullable1 = new CommandType?(CommandType.StoredProcedure);
      int? nullable2 = new int?();
      CommandType? nullable3 = nullable1;
      int num1 = await SqlMapper.ExecuteAsync((IDbConnection) connection, "[ddocAdmin].[sp_CreatePermission]", (object) dynamicParameters, transaction, nullable2, nullable3);
      int num2 = (int) parameters.Get<int>("@permissionId");
      parameters = (DynamicParameters) null;
      return num2;
    }

    public Task<int> UpdatePermission(DdocPermission permission)
    {
      DynamicParameters dynamicParameters1 = new DynamicParameters();
      dynamicParameters1.Add("@permissionId", (object) permission.Id, new DbType?(DbType.Int32), new ParameterDirection?(), new int?(), new byte?(), new byte?());
      dynamicParameters1.Add("@securityGroupId", (object) permission.SecurityGroupId, new DbType?(DbType.Int32), new ParameterDirection?(), new int?(), new byte?(), new byte?());
      dynamicParameters1.Add("@userGroupId", (object) permission.UserGroupId, new DbType?(DbType.Int32), new ParameterDirection?(), new int?(), new byte?(), new byte?());
      dynamicParameters1.Add("@readPermission", (object) permission.ReadPermission, new DbType?(DbType.Boolean), new ParameterDirection?(), new int?(), new byte?(), new byte?());
      dynamicParameters1.Add("@writePermission", (object) permission.WritePermission, new DbType?(DbType.Boolean), new ParameterDirection?(), new int?(), new byte?(), new byte?());
      dynamicParameters1.Add("@commentPermission", (object) permission.CommentPermission, new DbType?(DbType.Boolean), new ParameterDirection?(), new int?(), new byte?(), new byte?());
      dynamicParameters1.Add("@exportPermission", (object) permission.ExportPermission, new DbType?(DbType.Boolean), new ParameterDirection?(), new int?(), new byte?(), new byte?());
      dynamicParameters1.Add("@printPermission", (object) permission.PrintPermission, new DbType?(DbType.Boolean), new ParameterDirection?(), new int?(), new byte?(), new byte?());
      dynamicParameters1.Add("@deletePermission", (object) permission.DeletePermission, new DbType?(DbType.Boolean), new ParameterDirection?(), new int?(), new byte?(), new byte?());
      SqlConnection connection = ((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Connection;
      DynamicParameters dynamicParameters2 = dynamicParameters1;
      IDbTransaction transaction = ((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Transaction;
      CommandType? nullable1 = new CommandType?(CommandType.StoredProcedure);
      int? nullable2 = new int?();
      CommandType? nullable3 = nullable1;
      return SqlMapper.ExecuteAsync((IDbConnection) connection, "[ddocAdmin].[sp_UpdatePermission]", (object) dynamicParameters2, transaction, nullable2, nullable3);
    }

    public Task DeletePermission(int permissionId)
    {
      DynamicParameters dynamicParameters1 = new DynamicParameters();
      dynamicParameters1.Add("@permissionId", (object) permissionId, new DbType?(DbType.Int32), new ParameterDirection?(), new int?(), new byte?(), new byte?());
      SqlConnection connection = ((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Connection;
      DynamicParameters dynamicParameters2 = dynamicParameters1;
      IDbTransaction transaction = ((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Transaction;
      CommandType? nullable1 = new CommandType?(CommandType.StoredProcedure);
      int? nullable2 = new int?();
      CommandType? nullable3 = nullable1;
      return (Task) SqlMapper.ExecuteAsync((IDbConnection) connection, "[ddocAdmin].[sp_DeletePermission]", (object) dynamicParameters2, transaction, nullable2, nullable3);
    }

    public Task<DdocConfiguration> GetConfiguration()
    {
      SqlConnection connection = ((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Connection;
      IDbTransaction transaction = ((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Transaction;
      CommandType? nullable1 = new CommandType?(CommandType.StoredProcedure);
      int? nullable2 = new int?();
      CommandType? nullable3 = nullable1;
      return (Task<DdocConfiguration>) SqlMapper.QuerySingleAsync<DdocConfiguration>((IDbConnection) connection, "[ddocAdmin].[sp_GetConfiguration]", (object) null, transaction, nullable2, nullable3);
    }

    public Task<int> SaveConfiguration(DdocConfiguration configuration)
    {
      DynamicParameters dynamicParameters1 = new DynamicParameters();
      dynamicParameters1.Add("@passwordMaxLength", (object) configuration.PasswordMaxLength, new DbType?(DbType.Int32), new ParameterDirection?(), new int?(), new byte?(), new byte?());
      dynamicParameters1.Add("@passwordMinLength", (object) configuration.PasswordMinLength, new DbType?(DbType.Int32), new ParameterDirection?(), new int?(), new byte?(), new byte?());
      dynamicParameters1.Add("@failedTries", (object) configuration.FailedTries, new DbType?(DbType.Int32), new ParameterDirection?(), new int?(), new byte?(), new byte?());
      dynamicParameters1.Add("@inactivityDays", (object) configuration.InactivityDays, new DbType?(DbType.Int32), new ParameterDirection?(), new int?(), new byte?(), new byte?());
      dynamicParameters1.Add("@enablePasswordHistory", (object) configuration.EnablePasswordHistory, new DbType?(DbType.Boolean), new ParameterDirection?(), new int?(), new byte?(), new byte?());
      dynamicParameters1.Add("@pastPasswordCheck", (object) configuration.PastPasswordCheck, new DbType?(DbType.Int32), new ParameterDirection?(), new int?(), new byte?(), new byte?());
      dynamicParameters1.Add("@expirationDays", (object) configuration.ExpirationDays, new DbType?(DbType.Int32), new ParameterDirection?(), new int?(), new byte?(), new byte?());
      dynamicParameters1.Add("@daysForWarning", (object) configuration.DaysForWarning, new DbType?(DbType.Int32), new ParameterDirection?(), new int?(), new byte?(), new byte?());
      dynamicParameters1.Add("@enablePasswordExpiration", (object) configuration.DaysForWarning, new DbType?(DbType.Boolean), new ParameterDirection?(), new int?(), new byte?(), new byte?());
      SqlConnection connection = ((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Connection;
      DynamicParameters dynamicParameters2 = dynamicParameters1;
      IDbTransaction transaction = ((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Transaction;
      CommandType? nullable1 = new CommandType?(CommandType.StoredProcedure);
      int? nullable2 = new int?();
      CommandType? nullable3 = nullable1;
      return SqlMapper.ExecuteAsync((IDbConnection) connection, "[ddocAdmin].[sp_SaveConfiguration]", (object) dynamicParameters2, transaction, nullable2, nullable3);
    }

        public Task<User> GetUser(string username)
        {
            Query query = base.db.Query("G_USUARIOS");
            String[] strArrays = new String[] { "Usuario_ID as Username", "Nombre as Name", "Email" };
            int? nullable = null;
            return QueryExtensionsAsync.SingleOrDefaultAsync<User>(query.Select(strArrays).Where("Usuario_ID", username), nullable);
        }
        public Task<IEnumerable<User>> GetUsers()
    {
      SqlConnection connection = ((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Connection;
      IDbTransaction transaction = ((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Transaction;
      CommandType? nullable1 = new CommandType?(CommandType.StoredProcedure);
      int? nullable2 = new int?();
      CommandType? nullable3 = nullable1;
      return (Task<IEnumerable<User>>) SqlMapper.QueryAsync<User>((IDbConnection) connection, "[ddocAdmin].[sp_GetUsers]", (object) null, transaction, nullable2, nullable3);
    }

    public Task<int> CreateUser(User user)
    {
      DynamicParameters dynamicParameters1 = new DynamicParameters();
      DynamicParameters dynamicParameters2 = dynamicParameters1;
      string username = user.Username;
      DbType? nullable1 = new DbType?(DbType.String);
      int? nullable2 = new int?(50);
      ParameterDirection? nullable3 = new ParameterDirection?();
      int? nullable4 = nullable2;
      byte? nullable5 = new byte?();
      byte? nullable6 = new byte?();
      dynamicParameters2.Add("@username", (object) username, nullable1, nullable3, nullable4, nullable5, nullable6);
      DynamicParameters dynamicParameters3 = dynamicParameters1;
      string name = user.Name;
      DbType? nullable7 = new DbType?(DbType.String);
      nullable2 = new int?(50);
      ParameterDirection? nullable8 = new ParameterDirection?();
      int? nullable9 = nullable2;
      byte? nullable10 = new byte?();
      byte? nullable11 = new byte?();
      dynamicParameters3.Add("@name", (object) name, nullable7, nullable8, nullable9, nullable10, nullable11);
      DynamicParameters dynamicParameters4 = dynamicParameters1;
      string email = user.Email;
      DbType? nullable12 = new DbType?(DbType.String);
      nullable2 = new int?(50);
      ParameterDirection? nullable13 = new ParameterDirection?();
      int? nullable14 = nullable2;
      byte? nullable15 = new byte?();
      byte? nullable16 = new byte?();
      dynamicParameters4.Add("@email", (object) email, nullable12, nullable13, nullable14, nullable15, nullable16);
      DynamicParameters dynamicParameters5 = dynamicParameters1;
      string password = user.Password;
      DbType? nullable17 = new DbType?(DbType.String);
      nullable2 = new int?(50);
      ParameterDirection? nullable18 = new ParameterDirection?();
      int? nullable19 = nullable2;
      byte? nullable20 = new byte?();
      byte? nullable21 = new byte?();
      dynamicParameters5.Add("@password", (object) password, nullable17, nullable18, nullable19, nullable20, nullable21);
      DynamicParameters dynamicParameters6 = dynamicParameters1;
      string str = string.Join<int>(",", user.Profile.Select<DdocGroup, int>((Func<DdocGroup, int>) (g => g.Id)));
      DbType? nullable22 = new DbType?(DbType.String);
      nullable2 = new int?((int) byte.MaxValue);
      ParameterDirection? nullable23 = new ParameterDirection?();
      int? nullable24 = nullable2;
      byte? nullable25 = new byte?();
      byte? nullable26 = new byte?();
      dynamicParameters6.Add("@userGroupsCsv", (object) str, nullable22, nullable23, nullable24, nullable25, nullable26);
      SqlConnection connection = ((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Connection;
      DynamicParameters dynamicParameters7 = dynamicParameters1;
      IDbTransaction transaction = ((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Transaction;
      CommandType? nullable27 = new CommandType?(CommandType.StoredProcedure);
      int? nullable28 = new int?();
      CommandType? nullable29 = nullable27;
      return SqlMapper.ExecuteAsync((IDbConnection) connection, "[ddocAdmin].[sp_CreateUser]", (object) dynamicParameters7, transaction, nullable28, nullable29);
    }

    public Task<int> UpdateUser(User user)
    {
      DynamicParameters dynamicParameters1 = new DynamicParameters();
      DynamicParameters dynamicParameters2 = dynamicParameters1;
      string username = user.Username;
      DbType? nullable1 = new DbType?(DbType.String);
      int? nullable2 = new int?(50);
      ParameterDirection? nullable3 = new ParameterDirection?();
      int? nullable4 = nullable2;
      byte? nullable5 = new byte?();
      byte? nullable6 = new byte?();
      dynamicParameters2.Add("@username", (object) username, nullable1, nullable3, nullable4, nullable5, nullable6);
      DynamicParameters dynamicParameters3 = dynamicParameters1;
      string name = user.Name;
      DbType? nullable7 = new DbType?(DbType.String);
      nullable2 = new int?(50);
      ParameterDirection? nullable8 = new ParameterDirection?();
      int? nullable9 = nullable2;
      byte? nullable10 = new byte?();
      byte? nullable11 = new byte?();
      dynamicParameters3.Add("@name", (object) name, nullable7, nullable8, nullable9, nullable10, nullable11);
      DynamicParameters dynamicParameters4 = dynamicParameters1;
      string email = user.Email;
      DbType? nullable12 = new DbType?(DbType.String);
      nullable2 = new int?(50);
      ParameterDirection? nullable13 = new ParameterDirection?();
      int? nullable14 = nullable2;
      byte? nullable15 = new byte?();
      byte? nullable16 = new byte?();
      dynamicParameters4.Add("@email", (object) email, nullable12, nullable13, nullable14, nullable15, nullable16);
      SqlConnection connection = ((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Connection;
      DynamicParameters dynamicParameters5 = dynamicParameters1;
      IDbTransaction transaction = ((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Transaction;
      CommandType? nullable17 = new CommandType?(CommandType.StoredProcedure);
      int? nullable18 = new int?();
      CommandType? nullable19 = nullable17;
      return SqlMapper.ExecuteAsync((IDbConnection) connection, "[ddocAdmin].[sp_UpdateUser]", (object) dynamicParameters5, transaction, nullable18, nullable19);
    }

    public Task<IEnumerable<DdocGroup>> GetUserProfile(string username)
    {
      DynamicParameters dynamicParameters1 = new DynamicParameters();
      DynamicParameters dynamicParameters2 = dynamicParameters1;
      string str = username;
      DbType? nullable1 = new DbType?(DbType.String);
      int? nullable2 = new int?(50);
      ParameterDirection? nullable3 = new ParameterDirection?();
      int? nullable4 = nullable2;
      byte? nullable5 = new byte?();
      byte? nullable6 = new byte?();
      dynamicParameters2.Add("@username", (object) str, nullable1, nullable3, nullable4, nullable5, nullable6);
      SqlConnection connection = ((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Connection;
      DynamicParameters dynamicParameters3 = dynamicParameters1;
      IDbTransaction transaction = ((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Transaction;
      CommandType? nullable7 = new CommandType?(CommandType.StoredProcedure);
      int? nullable8 = new int?();
      CommandType? nullable9 = nullable7;
      return (Task<IEnumerable<DdocGroup>>) SqlMapper.QueryAsync<DdocGroup>((IDbConnection) connection, "[ddocAdmin].[sp_GetUserProfile]", (object) dynamicParameters3, transaction, nullable8, nullable9);
    }

    public Task UpdateUserProfile(string username, string userGroupsCsv)
    {
      DynamicParameters dynamicParameters1 = new DynamicParameters();
      DynamicParameters dynamicParameters2 = dynamicParameters1;
      string str1 = username;
      DbType? nullable1 = new DbType?(DbType.String);
      int? nullable2 = new int?(50);
      ParameterDirection? nullable3 = new ParameterDirection?();
      int? nullable4 = nullable2;
      byte? nullable5 = new byte?();
      byte? nullable6 = new byte?();
      dynamicParameters2.Add("@username", (object) str1, nullable1, nullable3, nullable4, nullable5, nullable6);
      DynamicParameters dynamicParameters3 = dynamicParameters1;
      string str2 = userGroupsCsv;
      DbType? nullable7 = new DbType?(DbType.String);
      nullable2 = new int?((int) byte.MaxValue);
      ParameterDirection? nullable8 = new ParameterDirection?();
      int? nullable9 = nullable2;
      byte? nullable10 = new byte?();
      byte? nullable11 = new byte?();
      dynamicParameters3.Add("@userGroupsCsv", (object) str2, nullable7, nullable8, nullable9, nullable10, nullable11);
      SqlConnection connection = ((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Connection;
      DynamicParameters dynamicParameters4 = dynamicParameters1;
      IDbTransaction transaction = ((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Transaction;
      CommandType? nullable12 = new CommandType?(CommandType.StoredProcedure);
      int? nullable13 = new int?();
      CommandType? nullable14 = nullable12;
      return (Task) SqlMapper.ExecuteAsync((IDbConnection) connection, "[ddocAdmin].[sp_UpdateUserProfile]", (object) dynamicParameters4, transaction, nullable13, nullable14);
    }

    public Task<int> UpdatePassword(string username, string password)
    {
      DynamicParameters dynamicParameters1 = new DynamicParameters();
      DynamicParameters dynamicParameters2 = dynamicParameters1;
      string str1 = username;
      DbType? nullable1 = new DbType?(DbType.String);
      int? nullable2 = new int?(50);
      ParameterDirection? nullable3 = new ParameterDirection?();
      int? nullable4 = nullable2;
      byte? nullable5 = new byte?();
      byte? nullable6 = new byte?();
      dynamicParameters2.Add("@username", (object) str1, nullable1, nullable3, nullable4, nullable5, nullable6);
      DynamicParameters dynamicParameters3 = dynamicParameters1;
      string str2 = password;
      DbType? nullable7 = new DbType?(DbType.String);
      nullable2 = new int?(32);
      ParameterDirection? nullable8 = new ParameterDirection?();
      int? nullable9 = nullable2;
      byte? nullable10 = new byte?();
      byte? nullable11 = new byte?();
      dynamicParameters3.Add("@password", (object) str2, nullable7, nullable8, nullable9, nullable10, nullable11);
      SqlConnection connection = ((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Connection;
      DynamicParameters dynamicParameters4 = dynamicParameters1;
      IDbTransaction transaction = ((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Transaction;
      CommandType? nullable12 = new CommandType?(CommandType.StoredProcedure);
      int? nullable13 = new int?();
      CommandType? nullable14 = nullable12;
      return SqlMapper.ExecuteAsync((IDbConnection) connection, "[ddocAdmin].[sp_UpdatePassword]", (object) dynamicParameters4, transaction, nullable13, nullable14);
    }

    public Task DeleteUser(string username)
    {
      DynamicParameters dynamicParameters1 = new DynamicParameters();
      DynamicParameters dynamicParameters2 = dynamicParameters1;
      string str = username;
      DbType? nullable1 = new DbType?(DbType.String);
      int? nullable2 = new int?(50);
      ParameterDirection? nullable3 = new ParameterDirection?();
      int? nullable4 = nullable2;
      byte? nullable5 = new byte?();
      byte? nullable6 = new byte?();
      dynamicParameters2.Add("@username", (object) str, nullable1, nullable3, nullable4, nullable5, nullable6);
      SqlConnection connection = ((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Connection;
      DynamicParameters dynamicParameters3 = dynamicParameters1;
      IDbTransaction transaction = ((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Transaction;
      CommandType? nullable7 = new CommandType?(CommandType.StoredProcedure);
      int? nullable8 = new int?();
      CommandType? nullable9 = nullable7;
      return (Task) SqlMapper.ExecuteAsync((IDbConnection) connection, "[ddocAdmin].[sp_DeleteUser]", (object) dynamicParameters3, transaction, nullable8, nullable9);
    }

    public Task<int> UnlockUser(string username)
    {
      DynamicParameters dynamicParameters1 = new DynamicParameters();
      DynamicParameters dynamicParameters2 = dynamicParameters1;
      string str = username;
      DbType? nullable1 = new DbType?(DbType.String);
      int? nullable2 = new int?(50);
      ParameterDirection? nullable3 = new ParameterDirection?();
      int? nullable4 = nullable2;
      byte? nullable5 = new byte?();
      byte? nullable6 = new byte?();
      dynamicParameters2.Add("@username", (object) str, nullable1, nullable3, nullable4, nullable5, nullable6);
      SqlConnection connection = ((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Connection;
      DynamicParameters dynamicParameters3 = dynamicParameters1;
      IDbTransaction transaction = ((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Transaction;
      CommandType? nullable7 = new CommandType?(CommandType.StoredProcedure);
      int? nullable8 = new int?();
      CommandType? nullable9 = nullable7;
      return SqlMapper.ExecuteAsync((IDbConnection) connection, "[ddocAdmin].[sp_UnlockUser]", (object) dynamicParameters3, transaction, nullable8, nullable9);
    }

        public Task<bool> FileServerExists(string fileServerId)
        {
            int? nullable = null;
            return QueryExtensionsAsync.ExistsAsync(base.db.Query("G_SERVERS").Where("SERVERID", fileServerId), nullable);
        }

        public Task<IEnumerable<DdocFileServer>> GetFileServers()
    {
      SqlConnection connection = ((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Connection;
      IDbTransaction transaction = ((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Transaction;
      CommandType? nullable1 = new CommandType?(CommandType.StoredProcedure);
      int? nullable2 = new int?();
      CommandType? nullable3 = nullable1;
      return (Task<IEnumerable<DdocFileServer>>) SqlMapper.QueryAsync<DdocFileServer>((IDbConnection) connection, "[ddocAdmin].[sp_GetFileServers]", (object) null, transaction, nullable2, nullable3);
    }

    public async Task<string> CreateFileServer(DdocFileServer fileServer)
    {
      DynamicParameters parameters = new DynamicParameters();
      DynamicParameters dynamicParameters1 = parameters;
      string name = fileServer.Name;
      DbType? nullable1 = new DbType?(DbType.String);
      int? nullable2 = new int?(30);
      ParameterDirection? nullable3 = new ParameterDirection?();
      int? nullable4 = nullable2;
      byte? nullable5 = new byte?();
      byte? nullable6 = new byte?();
      dynamicParameters1.Add("@name", (object) name, nullable1, nullable3, nullable4, nullable5, nullable6);
      DynamicParameters dynamicParameters2 = parameters;
      string url = fileServer.Url;
      DbType? nullable7 = new DbType?(DbType.String);
      int? nullable8 = new int?((int) byte.MaxValue);
      ParameterDirection? nullable9 = new ParameterDirection?();
      int? nullable10 = nullable8;
      byte? nullable11 = new byte?();
      byte? nullable12 = new byte?();
      dynamicParameters2.Add("@url", (object) url, nullable7, nullable9, nullable10, nullable11, nullable12);
      DynamicParameters dynamicParameters3 = parameters;
      DbType? nullable13 = new DbType?(DbType.String);
      int? nullable14 = new int?(2);
      ParameterDirection? nullable15 = new ParameterDirection?(ParameterDirection.Output);
      int? nullable16 = nullable14;
      byte? nullable17 = new byte?();
      byte? nullable18 = new byte?();
      dynamicParameters3.Add("@serverId", (object) null, nullable13, nullable15, nullable16, nullable17, nullable18);
      SqlConnection connection = ((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Connection;
      DynamicParameters dynamicParameters4 = parameters;
      IDbTransaction transaction = ((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Transaction;
      CommandType? nullable19 = new CommandType?(CommandType.StoredProcedure);
      int? nullable20 = new int?();
      CommandType? nullable21 = nullable19;
      int num = await SqlMapper.ExecuteAsync((IDbConnection) connection, "[ddocAdmin].[sp_CreateFileServer]", (object) dynamicParameters4, transaction, nullable20, nullable21);
      string str = (string) parameters.Get<string>("@serverId");
      parameters = (DynamicParameters) null;
      return str;
    }

    public Task<int> UpdateFileServer(DdocFileServer fileServer)
    {
      DynamicParameters dynamicParameters1 = new DynamicParameters();
      DynamicParameters dynamicParameters2 = dynamicParameters1;
      string id = fileServer.Id;
      DbType? nullable1 = new DbType?(DbType.String);
      int? nullable2 = new int?(2);
      ParameterDirection? nullable3 = new ParameterDirection?();
      int? nullable4 = nullable2;
      byte? nullable5 = new byte?();
      byte? nullable6 = new byte?();
      dynamicParameters2.Add("@serverId", (object) id, nullable1, nullable3, nullable4, nullable5, nullable6);
      DynamicParameters dynamicParameters3 = dynamicParameters1;
      string name = fileServer.Name;
      DbType? nullable7 = new DbType?(DbType.String);
      nullable2 = new int?(30);
      ParameterDirection? nullable8 = new ParameterDirection?();
      int? nullable9 = nullable2;
      byte? nullable10 = new byte?();
      byte? nullable11 = new byte?();
      dynamicParameters3.Add("@name", (object) name, nullable7, nullable8, nullable9, nullable10, nullable11);
      DynamicParameters dynamicParameters4 = dynamicParameters1;
      string url = fileServer.Url;
      DbType? nullable12 = new DbType?(DbType.String);
      nullable2 = new int?((int) byte.MaxValue);
      ParameterDirection? nullable13 = new ParameterDirection?();
      int? nullable14 = nullable2;
      byte? nullable15 = new byte?();
      byte? nullable16 = new byte?();
      dynamicParameters4.Add("@url", (object) url, nullable12, nullable13, nullable14, nullable15, nullable16);
      SqlConnection connection = ((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Connection;
      DynamicParameters dynamicParameters5 = dynamicParameters1;
      IDbTransaction transaction = ((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Transaction;
      CommandType? nullable17 = new CommandType?(CommandType.StoredProcedure);
      int? nullable18 = new int?();
      CommandType? nullable19 = nullable17;
      return SqlMapper.ExecuteAsync((IDbConnection) connection, "[ddocAdmin].[sp_UpdateFileServer]", (object) dynamicParameters5, transaction, nullable18, nullable19);
    }

    public Task DeleteFileServer(string fileServerId)
    {
      DynamicParameters dynamicParameters1 = new DynamicParameters();
      DynamicParameters dynamicParameters2 = dynamicParameters1;
      string str = fileServerId;
      DbType? nullable1 = new DbType?(DbType.String);
      int? nullable2 = new int?(2);
      ParameterDirection? nullable3 = new ParameterDirection?();
      int? nullable4 = nullable2;
      byte? nullable5 = new byte?();
      byte? nullable6 = new byte?();
      dynamicParameters2.Add("@serverId", (object) str, nullable1, nullable3, nullable4, nullable5, nullable6);
      SqlConnection connection = ((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Connection;
      DynamicParameters dynamicParameters3 = dynamicParameters1;
      IDbTransaction transaction = ((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Transaction;
      CommandType? nullable7 = new CommandType?(CommandType.StoredProcedure);
      int? nullable8 = new int?();
      CommandType? nullable9 = nullable7;
      return (Task) SqlMapper.ExecuteAsync((IDbConnection) connection, "[ddocAdmin].[sp_DeleteServer]", (object) dynamicParameters3, transaction, nullable8, nullable9);
    }

        public Task<bool> WarehouseExists(int warehouseId)
        {
            int? nullable = null;
            return QueryExtensionsAsync.ExistsAsync(base.db.Query("G_ALMACEN").Where("ALMACEN_ID", warehouseId), nullable);
        }
        public Task<DdocWarehouse> GetWarehouse(int warehouseId)
    {
      DynamicParameters dynamicParameters1 = new DynamicParameters();
      dynamicParameters1.Add("@warehouseId", (object) warehouseId, new DbType?(DbType.Int32), new ParameterDirection?(), new int?(), new byte?(), new byte?());
      SqlConnection connection = ((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Connection;
      DynamicParameters dynamicParameters2 = dynamicParameters1;
      IDbTransaction transaction = ((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Transaction;
      CommandType? nullable1 = new CommandType?(CommandType.StoredProcedure);
      int? nullable2 = new int?();
      CommandType? nullable3 = nullable1;
      return (Task<DdocWarehouse>) SqlMapper.QuerySingleOrDefaultAsync<DdocWarehouse>((IDbConnection) connection, "[ddocAdmin].[sp_GetWarehouse]", (object) dynamicParameters2, transaction, nullable2, nullable3);
    }

    public Task<IEnumerable<DdocWarehouse>> GetWarehouses()
    {
      SqlConnection connection = ((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Connection;
      IDbTransaction transaction = ((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Transaction;
      CommandType? nullable1 = new CommandType?(CommandType.StoredProcedure);
      int? nullable2 = new int?();
      CommandType? nullable3 = nullable1;
      return (Task<IEnumerable<DdocWarehouse>>) SqlMapper.QueryAsync<DdocWarehouse>((IDbConnection) connection, "[ddocAdmin].[sp_GetWarehouses]", (object) null, transaction, nullable2, nullable3);
    }

        public Task<bool> WarehousePathExists(int warehousePathId)
        {
            int? nullable = null;
            return QueryExtensionsAsync.ExistsAsync(base.db.Query("G_PATH").Where("PATH_ID", warehousePathId), nullable);
        }

        public Task<IEnumerable<DdocPath>> GetWarehousePaths(int warehouseId)
    {
      DynamicParameters dynamicParameters1 = new DynamicParameters();
      dynamicParameters1.Add("@warehouseId", (object) warehouseId, new DbType?(DbType.Int32), new ParameterDirection?(), new int?(), new byte?(), new byte?());
      SqlConnection connection = ((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Connection;
      DynamicParameters dynamicParameters2 = dynamicParameters1;
      IDbTransaction transaction = ((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Transaction;
      CommandType? nullable1 = new CommandType?(CommandType.StoredProcedure);
      int? nullable2 = new int?();
      CommandType? nullable3 = nullable1;
      return (Task<IEnumerable<DdocPath>>) SqlMapper.QueryAsync<DdocPath>((IDbConnection) connection, "[ddocAdmin].[sp_GetWarehousePaths]", (object) dynamicParameters2, transaction, nullable2, nullable3);
    }

    public async Task<int> CreateWarehouse(DdocWarehouse warehouse)
    {
      DynamicParameters parameters = new DynamicParameters();
      DynamicParameters dynamicParameters1 = parameters;
      string serverId = warehouse.ServerId;
      DbType? nullable1 = new DbType?(DbType.String);
      int? nullable2 = new int?(2);
      ParameterDirection? nullable3 = new ParameterDirection?();
      int? nullable4 = nullable2;
      byte? nullable5 = new byte?();
      byte? nullable6 = new byte?();
      dynamicParameters1.Add("@serverId", (object) serverId, nullable1, nullable3, nullable4, nullable5, nullable6);
      DynamicParameters dynamicParameters2 = parameters;
      string name = warehouse.Name;
      DbType? nullable7 = new DbType?(DbType.String);
      int? nullable8 = new int?(30);
      ParameterDirection? nullable9 = new ParameterDirection?();
      int? nullable10 = nullable8;
      byte? nullable11 = new byte?();
      byte? nullable12 = new byte?();
      dynamicParameters2.Add("@name", (object) name, nullable7, nullable9, nullable10, nullable11, nullable12);
      DynamicParameters dynamicParameters3 = parameters;
      string activePath = warehouse.ActivePath;
      DbType? nullable13 = new DbType?(DbType.String);
      int? nullable14 = new int?((int) byte.MaxValue);
      ParameterDirection? nullable15 = new ParameterDirection?();
      int? nullable16 = nullable14;
      byte? nullable17 = new byte?();
      byte? nullable18 = new byte?();
      dynamicParameters3.Add("@rootPath", (object) activePath, nullable13, nullable15, nullable16, nullable17, nullable18);
      parameters.Add("@warehouseId", (object) null, new DbType?(DbType.Int32), new ParameterDirection?(ParameterDirection.Output), new int?(), new byte?(), new byte?());
      SqlConnection connection = ((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Connection;
      DynamicParameters dynamicParameters4 = parameters;
      IDbTransaction transaction = ((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Transaction;
      CommandType? nullable19 = new CommandType?(CommandType.StoredProcedure);
      int? nullable20 = new int?();
      CommandType? nullable21 = nullable19;
      int num1 = await SqlMapper.ExecuteAsync((IDbConnection) connection, "[ddocAdmin].[sp_CreateWarehouse]", (object) dynamicParameters4, transaction, nullable20, nullable21);
      int num2 = (int) parameters.Get<int>("@warehouseId");
      parameters = (DynamicParameters) null;
      return num2;
    }

    public async Task<int> AddWarehousePath(int warehouseId, DdocPath ddocPath)
    {
      DynamicParameters parameters = new DynamicParameters();
      parameters.Add("@warehouseId", (object) warehouseId, new DbType?(DbType.Int32), new ParameterDirection?(), new int?(), new byte?(), new byte?());
      DynamicParameters dynamicParameters1 = parameters;
      string rootPath = ddocPath.RootPath;
      DbType? nullable1 = new DbType?(DbType.String);
      int? nullable2 = new int?((int) byte.MaxValue);
      ParameterDirection? nullable3 = new ParameterDirection?();
      int? nullable4 = nullable2;
      byte? nullable5 = new byte?();
      byte? nullable6 = new byte?();
      dynamicParameters1.Add("@rootPath", (object) rootPath, nullable1, nullable3, nullable4, nullable5, nullable6);
      parameters.Add("@pathId", (object) null, new DbType?(DbType.Int32), new ParameterDirection?(ParameterDirection.Output), new int?(), new byte?(), new byte?());
      SqlConnection connection = ((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Connection;
      DynamicParameters dynamicParameters2 = parameters;
      IDbTransaction transaction = ((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Transaction;
      CommandType? nullable7 = new CommandType?(CommandType.StoredProcedure);
      int? nullable8 = new int?();
      CommandType? nullable9 = nullable7;
      int num1 = await SqlMapper.ExecuteAsync((IDbConnection) connection, "[ddocAdmin].[sp_AddWarehousePath]", (object) dynamicParameters2, transaction, nullable8, nullable9);
      int num2 = (int) parameters.Get<int>("@pathId");
      parameters = (DynamicParameters) null;
      return num2;
    }

        public Task<int> UpdateWarehousePath(DdocPath path)
        {
            DynamicParameters dynamicParameter = new DynamicParameters();
            ParameterDirection? nullable = null;
            int? nullable1 = null;
            byte? nullable2 = null;
            byte? nullable3 = nullable2;
            nullable2 = null;
            dynamicParameter.Add("@pathId", path.Id, new DbType?(DbType.Int32), nullable, nullable1, nullable3, nullable2);
            string rootPath = path.RootPath;
            DbType? nullable4 = new DbType?(DbType.String);
            nullable1 = new int?(255);
            nullable = null;
            nullable2 = null;
            byte? nullable5 = nullable2;
            nullable2 = null;
            dynamicParameter.Add("@rootPath", rootPath, nullable4, nullable, nullable1, nullable5, nullable2);
            SqlConnection connection = base.Connection;
            IDbTransaction transaction = base.Transaction;
            CommandType? nullable6 = new CommandType?(CommandType.StoredProcedure);
            nullable1 = null;
            Task<int> task = connection.ExecuteAsync("[ddocAdmin].[sp_UpdateWarehousePath]", dynamicParameter, transaction, nullable1, nullable6);
            return task;
        }

        public Task<int> DeleteWarehousePath(int pathId)
    {
      DynamicParameters dynamicParameters1 = new DynamicParameters();
      dynamicParameters1.Add("@pathId", (object) pathId, new DbType?(DbType.Int32), new ParameterDirection?(), new int?(), new byte?(), new byte?());
      SqlConnection connection = ((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Connection;
      DynamicParameters dynamicParameters2 = dynamicParameters1;
      IDbTransaction transaction = ((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Transaction;
      CommandType? nullable1 = new CommandType?(CommandType.StoredProcedure);
      int? nullable2 = new int?();
      CommandType? nullable3 = nullable1;
      return SqlMapper.ExecuteAsync((IDbConnection) connection, "[ddocAdmin].[sp_DeleteWarehousePath]", (object) dynamicParameters2, transaction, nullable2, nullable3);
    }

        public Task<int> UpdateWarehouse(DdocWarehouse warehouse)
        {
            DynamicParameters dynamicParameter = new DynamicParameters();
            ParameterDirection? nullable = null;
            int? nullable1 = null;
            byte? nullable2 = null;
            byte? nullable3 = nullable2;
            nullable2 = null;
            dynamicParameter.Add("@warehouseId", warehouse.Id, new DbType?(DbType.Int32), nullable, nullable1, nullable3, nullable2);
            string name = warehouse.Name;
            DbType? nullable4 = new DbType?(DbType.String);
            nullable1 = new int?(30);
            nullable = null;
            nullable2 = null;
            byte? nullable5 = nullable2;
            nullable2 = null;
            dynamicParameter.Add("@name", name, nullable4, nullable, nullable1, nullable5, nullable2);
            SqlConnection connection = base.Connection;
            IDbTransaction transaction = base.Transaction;
            CommandType? nullable6 = new CommandType?(CommandType.StoredProcedure);
            nullable1 = null;
            Task<int> task = connection.ExecuteAsync("[ddocAdmin].[sp_UpdateWarehouse]", dynamicParameter, transaction, nullable1, nullable6);
            return task;
        }

        public Task<int> SetWarehouseActivePath(int warehouseId, int pathId)
    {
      DynamicParameters dynamicParameters1 = new DynamicParameters();
      dynamicParameters1.Add("@warehouseId", (object) warehouseId, new DbType?(DbType.Int32), new ParameterDirection?(), new int?(), new byte?(), new byte?());
      dynamicParameters1.Add("@pathId", (object) pathId, new DbType?(DbType.Int32), new ParameterDirection?(), new int?(), new byte?(), new byte?());
      SqlConnection connection = ((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Connection;
      DynamicParameters dynamicParameters2 = dynamicParameters1;
      IDbTransaction transaction = ((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Transaction;
      CommandType? nullable1 = new CommandType?(CommandType.StoredProcedure);
      int? nullable2 = new int?();
      CommandType? nullable3 = nullable1;
      return SqlMapper.ExecuteAsync((IDbConnection) connection, "[ddocAdmin].[sp_SetWarehouseActivePath]", (object) dynamicParameters2, transaction, nullable2, nullable3);
    }

    public Task DeleteWarehouse(int warehouseId)
    {
      DynamicParameters dynamicParameters1 = new DynamicParameters();
      dynamicParameters1.Add("@warehouseId", (object) warehouseId, new DbType?(DbType.Int32), new ParameterDirection?(), new int?(), new byte?(), new byte?());
      SqlConnection connection = ((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Connection;
      DynamicParameters dynamicParameters2 = dynamicParameters1;
      IDbTransaction transaction = ((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Transaction;
      CommandType? nullable1 = new CommandType?(CommandType.StoredProcedure);
      int? nullable2 = new int?();
      CommandType? nullable3 = nullable1;
      return (Task) SqlMapper.ExecuteAsync((IDbConnection) connection, "[ddocAdmin].[sp_DeleteWarehouse]", (object) dynamicParameters2, transaction, nullable2, nullable3);
    }


       
        //IDbTransaction ICommonDAL.get_Transaction()
        //{
        //    return base.Transaction;
        //}

        //void DigitalData.DDoc.Common.Api.DataAccess.ICommonDAL.set_Transaction(IDbTransaction value)
        //{
        //    base.Transaction = value;
        //}

        //[SpecialName]
        //IDbTransaction ICommonDAL.get_Transaction() => ((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Transaction;

        //[SpecialName]
        //void ICommonDAL.set_Transaction(IDbTransaction value) => ((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Transaction = value;
    }
}
