// Decompiled with JetBrains decompiler
// Type: DigitalData.DDoc.Modules.DataAccess.SecurityDAL
// Assembly: DigitalData.DDoc.Modules.DataAccess.SqlServer, Version=4.210.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EB5ECC0D-3654-4980-9D29-200BC39CB926
// Assembly location: D:\proyectos\ddocx\DigitalData.DDoc.UI.Webx\DigitalData.DDoc.UI.Web\bin\DigitalData.DDoc.Modules.DataAccess.SqlServer.dll

using DigitalData.Common.DataAccess;
using DigitalData.Common.DataAccess.QueryExecution;
using DigitalData.Common.DataAccess.SqlServer;
using DigitalData.Open.Common.Api.DataAccess;
using DigitalData.Open.Common.Entities;
using DigitalData.Open.Common.Entities.Security;
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
  public class SecurityDAL : SqlServerDAL, ISecurityDAL, ICommonDAL
  {
    public SecurityDAL(SqlConnection connection, SqlTransaction transaction = null)
      : base(connection, transaction)
    {
    }

        public Task<bool> UserExists(string username)
        {
            int? nullable = null;
            return QueryExtensionsAsync.ExistsAsync(base.db.Query("G_USUARIOS").Where("Usuario_ID", username), nullable);
        }
        public Task DisableLogon(string user)
        {
            return QueryExtensionsAsync.UpdateAsync(base.db.Query("G_CONTRASENA").Where("Usuario_ID", user), (IReadOnlyDictionary<string, object>)(new Dictionary<string, object>()
            {
                { "ContrasenaActiva", false }
            }), null);
        }

        public Task<DdocPermission> GetPermission(int permissionId)
        {
            return QueryExtensionsAsync.SingleOrDefaultAsync<DdocPermission>(base.db.Query("G_PERMISOS").Where("PERMISOID", permissionId).Select(new String[] { "PERMISOID as Id", "GSID as SecurityGroupId", "GGNT_ID as UserGroupId", "G_READ as ReadPermission", "G_WRITE as WritePermission", "G_COMMENT as CommentPermission", "G_EXPORT as ExportPermission", "G_PRINT as PrintPermission", "G_DELETE as DeletePermission" }), null);
        }
        public async Task<DdocPermission> GetCollectionEffectivePermissions(string collectionId, string userGroupList, string ddocGroupList = null)
        {
            Query query = base.db.Query("G_COLECCIONES").Where("GID", collectionId);
            Query query1 = query.Select(new String[] { "COLECCION_SEGURIDAD" });
            if (!String.IsNullOrEmpty(ddocGroupList))
            {
                Query query2 = query1;
                string str = ddocGroupList;
                Char[] chrArray = new Char[] { ',' };
                query2.WhereIn<string>("COLECCION_SEGURIDAD", str.Split(chrArray));
            }
            int? nullable = null;
            int num = await QueryExtensionsAsync.FirstAsync<int>(query1, nullable);
            Query query3 = base.db.Query("G_PERMISOS").Where("GSID", num);
            string str1 = userGroupList;
            Char[] chrArray1 = new Char[] { ',' };
            Query query4 = query3.WhereIn<string>("GGNT_ID", str1.Split(chrArray1));
            String[] strArrays = new String[] { "PERMISOID as Id", "GSID as SecurityGroupId", "GGNT_ID as UserGroupId", "G_READ as ReadPermission", "G_WRITE as WritePermission", "G_COMMENT as CommentPermission", "G_EXPORT as ExportPermission", "G_PRINT as PrintPermission", "G_DELETE as DeletePermission" };
            nullable = null;
            DdocPermission ddocPermission = await QueryExtensionsAsync.FirstOrDefaultAsync<DdocPermission>(query4.Select(strArrays), nullable);
            DdocPermission ddocPermission1 = ddocPermission;
            query1 = null;
            return ddocPermission1;
        }

        public async Task<DdocPermission> GetFolderEffectivePermissions(string folderId, string userGroupList, string ddocGroupList = null)
        {
            Query query = base.db.Query("G_FOLDERS").Where("GID", folderId);
            Query query1 = query.Select(new String[] { "SEGURIDAD" });
            if (!String.IsNullOrEmpty(ddocGroupList))
            {
                Query query2 = query1;
                string str = ddocGroupList;
                Char[] chrArray = new Char[] { ',' };
                query2.WhereIn<string>("SEGURIDAD", str.Split(chrArray));
            }
            int? nullable = null;
            int num = await QueryExtensionsAsync.FirstAsync<int>(query1, nullable);
            Query query3 = base.db.Query("G_PERMISOS").Where("GSID", num);
            string str1 = userGroupList;
            Char[] chrArray1 = new Char[] { ',' };
            Query query4 = query3.WhereIn<string>("GGNT_ID", str1.Split(chrArray1));
            String[] strArrays = new String[] { "PERMISOID as Id", "GSID as SecurityGroupId", "GGNT_ID as UserGroupId", "G_READ as ReadPermission", "G_WRITE as WritePermission", "G_COMMENT as CommentPermission", "G_EXPORT as ExportPermission", "G_PRINT as PrintPermission", "G_DELETE as DeletePermission" };
            nullable = null;
            DdocPermission ddocPermission = await QueryExtensionsAsync.FirstOrDefaultAsync<DdocPermission>(query4.Select(strArrays), nullable);
            DdocPermission ddocPermission1 = ddocPermission;
            query1 = null;
            return ddocPermission1;
        }

        public async Task<DdocPermission> GetDocumentEffectivePermissions(string documentId, string userGroupList, string ddocGroupList)
        {
            Query query = base.db.Query("G_DOCUMENTOS").Where("GID", documentId);
            Query query1 = query.Select(new String[] { "SEGURIDAD" });
            if (!String.IsNullOrEmpty(ddocGroupList))
            {
                Query query2 = query1;
                string str = ddocGroupList;
                Char[] chrArray = new Char[] { ',' };
                query2.WhereIn<string>("SEGURIDAD", str.Split(chrArray));
            }
            int? nullable = null;
            int num = await QueryExtensionsAsync.FirstAsync<int>(query1, nullable);
            Query query3 = base.db.Query("G_PERMISOS").Where("GSID", num);
            string str1 = userGroupList;
            Char[] chrArray1 = new Char[] { ',' };
            Query query4 = query3.WhereIn<string>("GGNT_ID", str1.Split(chrArray1));
            String[] strArrays = new String[] { "PERMISOID as Id", "GSID as SecurityGroupId", "GGNT_ID as UserGroupId", "G_READ as ReadPermission", "G_WRITE as WritePermission", "G_COMMENT as CommentPermission", "G_EXPORT as ExportPermission", "G_PRINT as PrintPermission", "G_DELETE as DeletePermission" };
            nullable = null;
            DdocPermission ddocPermission = await QueryExtensionsAsync.FirstOrDefaultAsync<DdocPermission>(query4.Select(strArrays), nullable);
            DdocPermission ddocPermission1 = ddocPermission;
            query1 = null;
            return ddocPermission1;
        }

        public async Task<Permissions> GetUserGroupPermissions(List<DdocGroup> groups)
    {
      Permissions effectivePermissions = Permissions.None;
      foreach (DdocGroup group1 in groups)
      {
        DdocGroup group = group1;
        IEnumerable<DdocPermission> permissions = await (Task<IEnumerable<DdocPermission>>) QueryExtensionsAsync.GetAsync<DdocPermission>(base.db.Query("G_PERMISOS").Where("GGNT_ID", (object) group.Id).Select("PERMISOID as Id", "GSID as SecurityGroupId", "GGNT_ID as UserGroupId", "G_READ as ReadPermission", "G_WRITE as WritePermission", "G_COMMENT as CommentPermission", "G_EXPORT as ExportPermission", "G_PRINT as PrintPermission", "G_DELETE as DeletePermission"), new int?());
        effectivePermissions = permissions.Aggregate<DdocPermission, Permissions>(effectivePermissions, (Func<Permissions, DdocPermission, Permissions>) ((current, permission) => current | permission.AsToken()));
        permissions = (IEnumerable<DdocPermission>) null;
        group = (DdocGroup) null;
      }
      return effectivePermissions;
    }

    public async Task<string> GetSecurityGroupsByUserGroups(string groupIdList)
    {
      IEnumerable<string> securityGroupIds = await (Task<IEnumerable<string>>) QueryExtensionsAsync.GetAsync<string>(base.db.Query("G_PERMISOS").WhereIn<string>("GGNT_ID", (IEnumerable<string>) groupIdList.Split(',')).Select("GSID"), new int?());
      string str = string.Join(",", securityGroupIds);
      securityGroupIds = (IEnumerable<string>) null;
      return str;
    }

    public Task<IEnumerable<DdocGroup>> GetUserProfile(string userName) => (Task<IEnumerable<DdocGroup>>) QueryExtensionsAsync.GetAsync<DdocGroup>(base.db.Query("G_PERFILES as P").Join("G_GRUPONT as UG", "UG.GGNT_ID", "P.GGNT_ID").Where("P.Usuario_ID", (object) userName).Select("P.GGNT_ID as Id", "UG.NOMBRE as Name"), new int?());

    public Task<DdocLoginConfig> GetLoginConfig() => (Task<DdocLoginConfig>) QueryExtensionsAsync.SingleAsync<DdocLoginConfig>(base.db.Query("G_CONFIGURACION").Select("DiasAviso as LoginWarningDays", "DiasVencimiento as LoginExpiryDays", "DiasdeInactividad as InactivityDays", "IntentosFallidos as Tries", "HabilitarVencimiento as LoginExpirationEnabled"), new int?());

    public Task<DdocLoginData> GetUserLoginData(string username) => (Task<DdocLoginData>) QueryExtensionsAsync.SingleOrDefaultAsync<DdocLoginData>(
        base.db.Query("G_USUARIOS as U").Join("G_CONTRASENA as P", "P.Usuario_ID", "U.Usuario_ID")
        .Where("U.Usuario_ID", (object) username).Select("P.ContrasenaActiva as Active", "P.Intentos as Tries", "P.FechaCreacion as CreationDate", "P.FechaVisita as LastLogin", "U.Usuario_ID as Username", "P.Contrasena as Password"), new int?());

    public async Task<int> IncrementFailedLogon(string user)
    {
      int actualTries = await (Task<int>) QueryExtensionsAsync.SingleAsync<int>(base.db.Query("G_CONTRASENA").Where("Usuario_ID", (object) user).Select("Intentos"), new int?());
      Query query = base.db.Query("G_CONTRASENA").Where("Usuario_ID", (object) user);
      Dictionary<string, object> dictionary = new Dictionary<string, object>();
      dictionary.Add("Intentos", (object) (actualTries + 1));
      string[] strArray = new string[1]
      {
        "inserted.Intentos"
      };
      int? nullable = new int?();
      IEnumerable<int> source = await (Task<IEnumerable<int>>) QueryExtensionsAsync.UpdateWithOutputAsync<int>(query, (IReadOnlyDictionary<string, object>) dictionary, (IEnumerable<string>) strArray, (string) null, nullable);
      return source.Single<int>();
    }

    public Task SetLogonInfo(string user)
    {
      Query query = base.db.Query("G_CONTRASENA").Where("Usuario_ID", (object) user);
      Dictionary<string, object> dictionary = new Dictionary<string, object>();
      dictionary.Add("ContrasenaActiva", (object) true);
      dictionary.Add("FechaVisita", (object) DateTime.Now);
      dictionary.Add("Intentos", (object) 0);
      int? nullable = new int?();
      return (Task) QueryExtensionsAsync.UpdateAsync(query, (IReadOnlyDictionary<string, object>) dictionary, nullable);
    }

   
        public Task<IEnumerable<DdocGroup>> GetSecurityGroups(string securityGroupsCsv = null)
        {
            Query query = base.db.Query("G_SEGURIDAD").Select(new String[] { "GS_ID as Id", "Nombre as Name" });
            if (!String.IsNullOrEmpty(securityGroupsCsv))
            {
                query.WhereIn<string>("GS_ID", securityGroupsCsv.Split(new Char[] { ',' }));
            }
            return QueryExtensionsAsync.GetAsync<DdocGroup>(query, null);
        }

        //[SpecialName]
        //IDbTransaction ICommonDAL.get_Transaction() => ((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Transaction;

        //[SpecialName]
        //void ICommonDAL.set_Transaction(IDbTransaction value) => ((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Transaction = value;
    }
}
