// Decompiled with JetBrains decompiler
// Type: DigitalData.DDoc.Modules.DataAccess.CollectionDAL
// Assembly: DigitalData.DDoc.Modules.DataAccess.SqlServer, Version=4.210.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EB5ECC0D-3654-4980-9D29-200BC39CB926
// Assembly location: D:\proyectos\ddocx\DigitalData.DDoc.UI.Webx\DigitalData.DDoc.UI.Web\bin\DigitalData.DDoc.Modules.DataAccess.SqlServer.dll

using Dapper;
using DigitalData.Common.DataAccess;
using DigitalData.Common.DataAccess.QueryExecution;
using DigitalData.Common.DataAccess.SqlServer;
using DigitalData.Open.Common.Api.DataAccess;
using DigitalData.Open.Common.Entities;
using SqlKata;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace DigitalData.Open.Modules.DataAccess
{
  public class CollectionDAL : SqlServerDAL, ICollectionDAL, ICommonDAL
  {
    public CollectionDAL(SqlConnection connection, SqlTransaction transaction = null)
      : base(connection, transaction)
    {
    }

        public Task<string> GetCollectionId(int fieldId)
        {
            return QueryExtensionsAsync.SingleOrDefaultAsync<string>(base.db.Query("G_CAMPOS").Where("CAMPO_ID").Select(new String[] { "GID" }), null);
        }
        public Task<bool> CollectionExists(string collectionId)
        {
            int? nullable = null;
            return QueryExtensionsAsync.ExistsAsync(base.db.Query("G_COLECCIONES").Where("GID", collectionId), nullable);
        }
        public Task<DdocCollection> GetCollection(string collectionId)
    {
      DynamicParameters dynamicParameters1 = new DynamicParameters();
      DynamicParameters dynamicParameters2 = dynamicParameters1;
      string str = collectionId;
      DbType? nullable1 = new DbType?(DbType.String);
      int? nullable2 = new int?(10);
      ParameterDirection? nullable3 = new ParameterDirection?();
      int? nullable4 = nullable2;
      byte? nullable5 = new byte?();
      byte? nullable6 = new byte?();
      dynamicParameters2.Add("@collectionId", (object) str, nullable1, nullable3, nullable4, nullable5, nullable6);
      SqlConnection connection = ((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Connection;
      DynamicParameters dynamicParameters3 = dynamicParameters1;
      IDbTransaction transaction = ((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Transaction;
      CommandType? nullable7 = new CommandType?(CommandType.StoredProcedure);
      int? nullable8 = new int?();
      CommandType? nullable9 = nullable7;
      return (Task<DdocCollection>) SqlMapper.QuerySingleOrDefaultAsync<DdocCollection>((IDbConnection) connection, "[ddoc].[sp_GetCollection]", (object) dynamicParameters3, transaction, nullable8, nullable9);
    }

    public Task<IEnumerable<DdocCollection>> SearchCollection(
      string name)
    {
      DynamicParameters dynamicParameters1 = new DynamicParameters();
      DynamicParameters dynamicParameters2 = dynamicParameters1;
      string str = name;
      DbType? nullable1 = new DbType?(DbType.String);
      int? nullable2 = new int?(100);
      ParameterDirection? nullable3 = new ParameterDirection?();
      int? nullable4 = nullable2;
      byte? nullable5 = new byte?();
      byte? nullable6 = new byte?();
      dynamicParameters2.Add("@collectionName", (object) str, nullable1, nullable3, nullable4, nullable5, nullable6);
      SqlConnection connection = ((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Connection;
      DynamicParameters dynamicParameters3 = dynamicParameters1;
      IDbTransaction transaction = ((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Transaction;
      CommandType? nullable7 = new CommandType?(CommandType.StoredProcedure);
      int? nullable8 = new int?();
      CommandType? nullable9 = nullable7;
      return (Task<IEnumerable<DdocCollection>>) SqlMapper.QueryAsync<DdocCollection>((IDbConnection) connection, "[ddoc].[sp_SearchCollection]", (object) dynamicParameters3, transaction, nullable8, nullable9);
    }

    public Task<IEnumerable<DdocCollection>> GetCollections()
    {
      SqlConnection connection = ((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Connection;
      IDbTransaction transaction = ((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Transaction;
      CommandType? nullable1 = new CommandType?(CommandType.StoredProcedure);
      int? nullable2 = new int?();
      CommandType? nullable3 = nullable1;
      return (Task<IEnumerable<DdocCollection>>) SqlMapper.QueryAsync<DdocCollection>((IDbConnection) connection, "[ddoc].[sp_GetCollections]", (object) null, transaction, nullable2, nullable3);
    }

        public Task<bool> CollectionFieldExists(int fieldId)
        {
            int? nullable = null;
            return QueryExtensionsAsync.ExistsAsync(base.db.Query("G_CAMPOS").Where("CAMPO_ID", fieldId), nullable);
        }
        public Task<DdocField> GetCollectionField(int fieldId)
        {
            return QueryExtensionsAsync.SingleOrDefaultAsync<DdocField>(base.db.Query("G_CAMPOS as C").Where("CAMPO_ID", fieldId).Select(new String[] { "C.CAMPO_ID AS Id", "C.CAMPO_NOMBRE AS Name", "C.CAMPO_NUMERO AS Sequence", "C.CAMPO_TIPO AS TypeString", "C.CAMPO_FORMATO AS Format", "C.CAMPO_OPCIONAL AS Nullable", "C.CAMPO_UNICO AS Unique", "C.CAMPO_VALORES AS AllowedValuesString", "C.CAMPO_IN_MASK AS InMask", "C.CAMPO_OUT_MASK AS OutMask", "C.CAMPO_HEREDABLE AS Inheritable", "C.CAMPO_OCULTO AS Hidden", "C.CAMPO_CALCULADO AS Computed", "C.CAMPO_BUSQUEDAGLOBAL AS IncludeInGlobalSearch" }), null);
        }
        public Task<IEnumerable<DdocField>> GetCollectionFields(
      string collectionId)
    {
      DynamicParameters dynamicParameters1 = new DynamicParameters();
      DynamicParameters dynamicParameters2 = dynamicParameters1;
      string str = collectionId;
      DbType? nullable1 = new DbType?(DbType.String);
      int? nullable2 = new int?(10);
      ParameterDirection? nullable3 = new ParameterDirection?();
      int? nullable4 = nullable2;
      byte? nullable5 = new byte?();
      byte? nullable6 = new byte?();
      dynamicParameters2.Add("@collectionId", (object) str, nullable1, nullable3, nullable4, nullable5, nullable6);
      SqlConnection connection = ((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Connection;
      DynamicParameters dynamicParameters3 = dynamicParameters1;
      IDbTransaction transaction = ((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Transaction;
      CommandType? nullable7 = new CommandType?(CommandType.StoredProcedure);
      int? nullable8 = new int?();
      CommandType? nullable9 = nullable7;
      return (Task<IEnumerable<DdocField>>) SqlMapper.QueryAsync<DdocField>((IDbConnection) connection, "[ddoc].[sp_GetCollectionFields]", (object) dynamicParameters3, transaction, nullable8, nullable9);
    }

    public Task<IEnumerable<DdocCollection>> GetRootCollections(
      string userGroupsCsv)
    {
      DynamicParameters dynamicParameters1 = new DynamicParameters();
      if (!string.IsNullOrEmpty(userGroupsCsv))
      {
        DynamicParameters dynamicParameters2 = dynamicParameters1;
        string str = userGroupsCsv;
        DbType? nullable1 = new DbType?(DbType.String);
        int? nullable2 = new int?(50);
        ParameterDirection? nullable3 = new ParameterDirection?();
        int? nullable4 = nullable2;
        byte? nullable5 = new byte?();
        byte? nullable6 = new byte?();
        dynamicParameters2.Add("@userGroupsCsv", (object) str, nullable1, nullable3, nullable4, nullable5, nullable6);
      }
      SqlConnection connection = ((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Connection;
      DynamicParameters dynamicParameters3 = dynamicParameters1;
      IDbTransaction transaction = ((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Transaction;
      CommandType? nullable7 = new CommandType?(CommandType.StoredProcedure);
      int? nullable8 = new int?();
      CommandType? nullable9 = nullable7;
      return (Task<IEnumerable<DdocCollection>>) SqlMapper.QueryAsync<DdocCollection>((IDbConnection) connection, "[ddoc].[sp_GetRootCollections]", (object) dynamicParameters3, transaction, nullable8, nullable9);
    }

    public Task<IEnumerable<DdocCollection>> GetSearchableCollections(
      string securityGroupsCsv,
      string collectionType = null)
    {
      DynamicParameters dynamicParameters1 = new DynamicParameters();
      DynamicParameters dynamicParameters2 = dynamicParameters1;
      string str = securityGroupsCsv;
      DbType? nullable1 = new DbType?(DbType.String);
      int? nullable2 = new int?(8000);
      ParameterDirection? nullable3 = new ParameterDirection?();
      int? nullable4 = nullable2;
      byte? nullable5 = new byte?();
      byte? nullable6 = new byte?();
      dynamicParameters2.Add("@securityGroupsCsv", (object) str, nullable1, nullable3, nullable4, nullable5, nullable6);
      if (!string.IsNullOrEmpty(collectionType))
        dynamicParameters1.Add("@collectionType", (object) collectionType[0], new DbType?(), new ParameterDirection?(), new int?(), new byte?(), new byte?());
      SqlConnection connection = ((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Connection;
      DynamicParameters dynamicParameters3 = dynamicParameters1;
      IDbTransaction transaction = ((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Transaction;
      CommandType? nullable7 = new CommandType?(CommandType.StoredProcedure);
      int? nullable8 = new int?();
      CommandType? nullable9 = nullable7;
      return (Task<IEnumerable<DdocCollection>>) SqlMapper.QueryAsync<DdocCollection>((IDbConnection) connection, "[ddoc].[sp_GetSearchableCollections]", (object) dynamicParameters3, transaction, nullable8, nullable9);
    }

    public async Task<IEnumerable<DdocCollection>> GetParentCollections(
      string collectionId,
      string userGroupsCsv = null)
    {
      DynamicParameters parameters = new DynamicParameters();
      DynamicParameters dynamicParameters1 = parameters;
      string str1 = collectionId;
      DbType? nullable1 = new DbType?(DbType.String);
      int? nullable2 = new int?(10);
      ParameterDirection? nullable3 = new ParameterDirection?();
      int? nullable4 = nullable2;
      byte? nullable5 = new byte?();
      byte? nullable6 = new byte?();
      dynamicParameters1.Add("@collectionId", (object) str1, nullable1, nullable3, nullable4, nullable5, nullable6);
      if (!string.IsNullOrEmpty(userGroupsCsv))
      {
        DynamicParameters dynamicParameters2 = parameters;
        string str2 = userGroupsCsv;
        DbType? nullable7 = new DbType?(DbType.String);
        int? nullable8 = new int?(50);
        ParameterDirection? nullable9 = new ParameterDirection?();
        int? nullable10 = nullable8;
        byte? nullable11 = new byte?();
        byte? nullable12 = new byte?();
        dynamicParameters2.Add("@userGroupsCsv", (object) str2, nullable7, nullable9, nullable10, nullable11, nullable12);
      }
      SqlConnection connection = ((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Connection;
      DynamicParameters dynamicParameters3 = parameters;
      IDbTransaction transaction = ((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Transaction;
      CommandType? nullable13 = new CommandType?(CommandType.StoredProcedure);
      int? nullable14 = new int?();
      CommandType? nullable15 = nullable13;
      IEnumerable<DdocCollection> collections = await (Task<IEnumerable<DdocCollection>>) SqlMapper.QueryAsync<DdocCollection>((IDbConnection) connection, "[ddoc].[sp_GetParentCollections]", (object) dynamicParameters3, transaction, nullable14, nullable15);
      IEnumerable<DdocCollection> ddocCollections = collections.Distinct<DdocCollection>();
      parameters = (DynamicParameters) null;
      collections = (IEnumerable<DdocCollection>) null;
      return ddocCollections;
    }

    public async Task<IEnumerable<DdocCollection>> GetChildCollections(
      string collectionId,
      string userGroupsCsv)
    {
      DynamicParameters parameters = new DynamicParameters();
      DynamicParameters dynamicParameters1 = parameters;
      string str1 = collectionId;
      DbType? nullable1 = new DbType?(DbType.String);
      int? nullable2 = new int?(10);
      ParameterDirection? nullable3 = new ParameterDirection?();
      int? nullable4 = nullable2;
      byte? nullable5 = new byte?();
      byte? nullable6 = new byte?();
      dynamicParameters1.Add("@collectionId", (object) str1, nullable1, nullable3, nullable4, nullable5, nullable6);
      if (!string.IsNullOrEmpty(userGroupsCsv))
      {
        DynamicParameters dynamicParameters2 = parameters;
        string str2 = userGroupsCsv;
        DbType? nullable7 = new DbType?(DbType.String);
        int? nullable8 = new int?(50);
        ParameterDirection? nullable9 = new ParameterDirection?();
        int? nullable10 = nullable8;
        byte? nullable11 = new byte?();
        byte? nullable12 = new byte?();
        dynamicParameters2.Add("@userGroupsCsv", (object) str2, nullable7, nullable9, nullable10, nullable11, nullable12);
      }
      SqlConnection connection = ((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Connection;
      DynamicParameters dynamicParameters3 = parameters;
      IDbTransaction transaction = ((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Transaction;
      CommandType? nullable13 = new CommandType?(CommandType.StoredProcedure);
      int? nullable14 = new int?();
      CommandType? nullable15 = nullable13;
      IEnumerable<DdocCollection> collections = await (Task<IEnumerable<DdocCollection>>) SqlMapper.QueryAsync<DdocCollection>((IDbConnection) connection, "[ddoc].[sp_GetChildCollections]", (object) dynamicParameters3, transaction, nullable14, nullable15);
      IEnumerable<DdocCollection> ddocCollections = collections.Distinct<DdocCollection>();
      parameters = (DynamicParameters) null;
      collections = (IEnumerable<DdocCollection>) null;
      return ddocCollections;
    }

    public Task<string> GetCollectionType(string collectionId)
    {
      DynamicParameters dynamicParameters1 = new DynamicParameters();
      DynamicParameters dynamicParameters2 = dynamicParameters1;
      string str = collectionId;
      DbType? nullable1 = new DbType?(DbType.String);
      int? nullable2 = new int?(10);
      ParameterDirection? nullable3 = new ParameterDirection?();
      int? nullable4 = nullable2;
      byte? nullable5 = new byte?();
      byte? nullable6 = new byte?();
      dynamicParameters2.Add("@collectionId", (object) str, nullable1, nullable3, nullable4, nullable5, nullable6);
      SqlConnection connection = ((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Connection;
      DynamicParameters dynamicParameters3 = dynamicParameters1;
      IDbTransaction transaction = ((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Transaction;
      CommandType? nullable7 = new CommandType?(CommandType.StoredProcedure);
      int? nullable8 = new int?();
      CommandType? nullable9 = nullable7;
      return (Task<string>) SqlMapper.QuerySingleOrDefaultAsync<string>((IDbConnection) connection, "[ddoc].[sp_GetCollectionType]", (object) dynamicParameters3, transaction, nullable8, nullable9);
    }

        public Task<string> GetCollectionId(string collectionName, char collectionType)
        {
            DynamicParameters dynamicParameter = new DynamicParameters();
            DbType? nullable = new DbType?(DbType.String);
            int? nullable1 = new int?(100);
            ParameterDirection? nullable2 = null;
            byte? nullable3 = null;
            byte? nullable4 = nullable3;
            nullable3 = null;
            dynamicParameter.Add("@collectionName", collectionName, nullable, nullable2, nullable1, nullable4, nullable3);
            object obj = collectionType;
            DbType? nullable5 = new DbType?(DbType.String);
            nullable1 = new int?(1);
            nullable2 = null;
            nullable3 = null;
            byte? nullable6 = nullable3;
            nullable3 = null;
            dynamicParameter.Add("@collectionType", obj, nullable5, nullable2, nullable1, nullable6, nullable3);
            SqlConnection connection = base.Connection;
            IDbTransaction transaction = base.Transaction;
            CommandType? nullable7 = new CommandType?(CommandType.StoredProcedure);
            nullable1 = null;
            Task<string> task = connection.QuerySingleOrDefaultAsync<string>("ddoc.sp_GetCollectionId", dynamicParameter, transaction, nullable1, nullable7);
            return task;
        }

        public Task<int> GetSecurityGroupId(string collectionName, char collectionType)
        {
            DynamicParameters dynamicParameter = new DynamicParameters();
            DbType? nullable = new DbType?(DbType.String);
            int? nullable1 = new int?(100);
            ParameterDirection? nullable2 = null;
            byte? nullable3 = null;
            byte? nullable4 = nullable3;
            nullable3 = null;
            dynamicParameter.Add("@collectionName", collectionName, nullable, nullable2, nullable1, nullable4, nullable3);
            object obj = collectionType;
            DbType? nullable5 = new DbType?(DbType.String);
            nullable1 = new int?(1);
            nullable2 = null;
            nullable3 = null;
            byte? nullable6 = nullable3;
            nullable3 = null;
            dynamicParameter.Add("@collectionType", obj, nullable5, nullable2, nullable1, nullable6, nullable3);
            SqlConnection connection = base.Connection;
            IDbTransaction transaction = base.Transaction;
            CommandType? nullable7 = new CommandType?(CommandType.StoredProcedure);
            nullable1 = null;
            Task<int> task = connection.QuerySingleOrDefaultAsync<int>("ddoc.sp_GetSecurityGroupId", dynamicParameter, transaction, nullable1, nullable7);
            return task;
        }

        public Task<bool> CollectionRuleExists(int ruleId)
        {
            int? nullable = null;
            return QueryExtensionsAsync.ExistsAsync(base.db.Query("G_REGLAS").Where("REGLA_ID", ruleId), nullable);
        }
        public Task<IEnumerable<DdocRule>> GetCollectionRules()
    {
      SqlConnection connection = ((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Connection;
      CommandType? nullable1 = new CommandType?(CommandType.StoredProcedure);
      int? nullable2 = new int?();
      CommandType? nullable3 = nullable1;
      return (Task<IEnumerable<DdocRule>>) SqlMapper.QueryAsync<DdocRule>((IDbConnection) connection, "[ddocAdmin].[sp_GetCollectionRules]", (object) null, (IDbTransaction) null, nullable2, nullable3);
    }

    public Task<IEnumerable<DdocRule>> GetRulesForParentCollection(
      string collectionId)
    {
      DynamicParameters dynamicParameters1 = new DynamicParameters();
      DynamicParameters dynamicParameters2 = dynamicParameters1;
      string str = collectionId;
      DbType? nullable1 = new DbType?(DbType.String);
      int? nullable2 = new int?(10);
      ParameterDirection? nullable3 = new ParameterDirection?();
      int? nullable4 = nullable2;
      byte? nullable5 = new byte?();
      byte? nullable6 = new byte?();
      dynamicParameters2.Add("@collectionId", (object) str, nullable1, nullable3, nullable4, nullable5, nullable6);
      SqlConnection connection = ((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Connection;
      DynamicParameters dynamicParameters3 = dynamicParameters1;
      IDbTransaction transaction = ((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Transaction;
      CommandType? nullable7 = new CommandType?(CommandType.StoredProcedure);
      int? nullable8 = new int?();
      CommandType? nullable9 = nullable7;
      return (Task<IEnumerable<DdocRule>>) SqlMapper.QueryAsync<DdocRule>((IDbConnection) connection, "[ddocAdmin].[sp_GetRulesForParentCollection]", (object) dynamicParameters3, transaction, nullable8, nullable9);
    }

    public Task<IEnumerable<DdocRule>> GetRulesForChildCollection(
      string collectionId)
    {
      DynamicParameters dynamicParameters1 = new DynamicParameters();
      DynamicParameters dynamicParameters2 = dynamicParameters1;
      string str = collectionId;
      DbType? nullable1 = new DbType?(DbType.String);
      int? nullable2 = new int?(10);
      ParameterDirection? nullable3 = new ParameterDirection?();
      int? nullable4 = nullable2;
      byte? nullable5 = new byte?();
      byte? nullable6 = new byte?();
      dynamicParameters2.Add("@collectionId", (object) str, nullable1, nullable3, nullable4, nullable5, nullable6);
      SqlConnection connection = ((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Connection;
      DynamicParameters dynamicParameters3 = dynamicParameters1;
      IDbTransaction transaction = ((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Transaction;
      CommandType? nullable7 = new CommandType?(CommandType.StoredProcedure);
      int? nullable8 = new int?();
      CommandType? nullable9 = nullable7;
      return (Task<IEnumerable<DdocRule>>) SqlMapper.QueryAsync<DdocRule>((IDbConnection) connection, "[ddocAdmin].[sp_GetRulesForChildCollection]", (object) dynamicParameters3, transaction, nullable8, nullable9);
    }

        public Task<IEnumerable<DdocRule>> GetRulesForParentField(int fieldId)
        {
            DynamicParameters dynamicParameter = new DynamicParameters();
            object obj = fieldId;
            DbType? nullable = new DbType?(DbType.String);
            int? nullable1 = new int?(10);
            ParameterDirection? nullable2 = null;
            byte? nullable3 = null;
            byte? nullable4 = nullable3;
            nullable3 = null;
            dynamicParameter.Add("@fieldId", obj, nullable, nullable2, nullable1, nullable4, nullable3);
            SqlConnection connection = base.Connection;
            IDbTransaction transaction = base.Transaction;
            CommandType? nullable5 = new CommandType?(CommandType.StoredProcedure);
            nullable1 = null;
            Task<IEnumerable<DdocRule>> task = connection.QueryAsync<DdocRule>("[ddocAdmin].[sp_GetRulesForParentField]", dynamicParameter, transaction, nullable1, nullable5);
            return task;
        }

        public Task<IEnumerable<DdocRule>> GetRulesForChildField(int fieldId)
        {
            DynamicParameters dynamicParameter = new DynamicParameters();
            object obj = fieldId;
            DbType? nullable = new DbType?(DbType.String);
            int? nullable1 = new int?(10);
            ParameterDirection? nullable2 = null;
            byte? nullable3 = null;
            byte? nullable4 = nullable3;
            nullable3 = null;
            dynamicParameter.Add("@fieldId", obj, nullable, nullable2, nullable1, nullable4, nullable3);
            SqlConnection connection = base.Connection;
            IDbTransaction transaction = base.Transaction;
            CommandType? nullable5 = new CommandType?(CommandType.StoredProcedure);
            nullable1 = null;
            Task<IEnumerable<DdocRule>> task = connection.QueryAsync<DdocRule>("[ddocAdmin].[sp_GetRulesForChildField]", dynamicParameter, transaction, nullable1, nullable5);
            return task;
        }

        public async Task<int> CreateCollectionRule(DdocRule rule)
    {
      DynamicParameters parameters = new DynamicParameters();
      DynamicParameters dynamicParameters1 = parameters;
      string parentId = rule.ParentId;
      DbType? nullable1 = new DbType?(DbType.String);
      int? nullable2 = new int?(10);
      ParameterDirection? nullable3 = new ParameterDirection?();
      int? nullable4 = nullable2;
      byte? nullable5 = new byte?();
      byte? nullable6 = new byte?();
      dynamicParameters1.Add("@parentCollectionId", (object) parentId, nullable1, nullable3, nullable4, nullable5, nullable6);
      DynamicParameters dynamicParameters2 = parameters;
      string childId = rule.ChildId;
      DbType? nullable7 = new DbType?(DbType.String);
      int? nullable8 = new int?(10);
      ParameterDirection? nullable9 = new ParameterDirection?();
      int? nullable10 = nullable8;
      byte? nullable11 = new byte?();
      byte? nullable12 = new byte?();
      dynamicParameters2.Add("@childCollectionId", (object) childId, nullable7, nullable9, nullable10, nullable11, nullable12);
      parameters.Add("@parentFieldId", (object) rule.ParentField, new DbType?(DbType.Int32), new ParameterDirection?(), new int?(), new byte?(), new byte?());
      parameters.Add("@childFieldId", (object) rule.ChildField, new DbType?(DbType.Int32), new ParameterDirection?(), new int?(), new byte?(), new byte?());
      parameters.Add("@ruleId", (object) null, new DbType?(DbType.Int32), new ParameterDirection?(ParameterDirection.Output), new int?(), new byte?(), new byte?());
      SqlConnection connection = ((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Connection;
      DynamicParameters dynamicParameters3 = parameters;
      IDbTransaction transaction = ((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Transaction;
      CommandType? nullable13 = new CommandType?(CommandType.StoredProcedure);
      int? nullable14 = new int?();
      CommandType? nullable15 = nullable13;
      int num1 = await SqlMapper.ExecuteAsync((IDbConnection) connection, "[ddocAdmin].[sp_CreateCollectionRule]", (object) dynamicParameters3, transaction, nullable14, nullable15);
      int num2 = (int) parameters.Get<int>("@ruleId");
      parameters = (DynamicParameters) null;
      return num2;
    }

        public Task<int> UpdateCollectionRule(DdocRule rule)
        {
            DynamicParameters dynamicParameter = new DynamicParameters();
            ParameterDirection? nullable = null;
            int? nullable1 = null;
            byte? nullable2 = null;
            byte? nullable3 = nullable2;
            nullable2 = null;
            dynamicParameter.Add("@ruleId", rule.Id, new DbType?(DbType.Int32), nullable, nullable1, nullable3, nullable2);
            string parentId = rule.ParentId;
            DbType? nullable4 = new DbType?(DbType.String);
            nullable1 = new int?(10);
            nullable = null;
            nullable2 = null;
            byte? nullable5 = nullable2;
            nullable2 = null;
            dynamicParameter.Add("@parentCollectionId", parentId, nullable4, nullable, nullable1, nullable5, nullable2);
            string childId = rule.ChildId;
            DbType? nullable6 = new DbType?(DbType.String);
            nullable1 = new int?(10);
            nullable = null;
            nullable2 = null;
            byte? nullable7 = nullable2;
            nullable2 = null;
            dynamicParameter.Add("@childCollectionId", childId, nullable6, nullable, nullable1, nullable7, nullable2);
            nullable = null;
            nullable1 = null;
            nullable2 = null;
            byte? nullable8 = nullable2;
            nullable2 = null;
            dynamicParameter.Add("@parentFieldId", rule.ParentField, new DbType?(DbType.Int32), nullable, nullable1, nullable8, nullable2);
            nullable = null;
            nullable1 = null;
            nullable2 = null;
            byte? nullable9 = nullable2;
            nullable2 = null;
            dynamicParameter.Add("@childFieldId", rule.ChildField, new DbType?(DbType.Int32), nullable, nullable1, nullable9, nullable2);
            SqlConnection connection = base.Connection;
            IDbTransaction transaction = base.Transaction;
            CommandType? nullable10 = new CommandType?(CommandType.StoredProcedure);
            nullable1 = null;
            Task<int> task = connection.ExecuteAsync("[ddocAdmin].[sp_UpdateCollectionRule]", dynamicParameter, transaction, nullable1, nullable10);
            return task;
        }

        public Task DeleteRule(int id)
    {
      DynamicParameters dynamicParameters1 = new DynamicParameters();
      dynamicParameters1.Add("@ruleId", (object) id, new DbType?(DbType.Int32), new ParameterDirection?(), new int?(), new byte?(), new byte?());
      SqlConnection connection = ((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Connection;
      DynamicParameters dynamicParameters2 = dynamicParameters1;
      IDbTransaction transaction = ((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Transaction;
      CommandType? nullable1 = new CommandType?(CommandType.StoredProcedure);
      int? nullable2 = new int?();
      CommandType? nullable3 = nullable1;
      return (Task) SqlMapper.ExecuteAsync((IDbConnection) connection, "[ddocAdmin].[sp_DeleteCollectionRule]", (object) dynamicParameters2, transaction, nullable2, nullable3);
    }

        public async Task<string> CreateCollection(DdocCollection collection)
        {
            ParameterDirection? nullable;
            byte? nullable1;
            DynamicParameters dynamicParameter = new DynamicParameters();
            int? warehouseId = collection.WarehouseId;
            if (warehouseId.HasValue)
            {
                nullable = null;
                warehouseId = null;
                nullable1 = null;
                byte? nullable2 = nullable1;
                nullable1 = null;
                dynamicParameter.Add("@warehouseId", collection.WarehouseId, new DbType?(DbType.Int32), nullable, warehouseId, nullable2, nullable1);
            }
            DynamicParameters dynamicParameter1 = dynamicParameter;
            string name = collection.Name;
            DbType? nullable3 = new DbType?(DbType.String);
            warehouseId = new int?(50);
            nullable = null;
            nullable1 = null;
            byte? nullable4 = nullable1;
            nullable1 = null;
            dynamicParameter1.Add("@name", name, nullable3, nullable, warehouseId, nullable4, nullable1);
            DynamicParameters dynamicParameter2 = dynamicParameter;
            string description = collection.Description;
            DbType? nullable5 = new DbType?(DbType.String);
            warehouseId = new int?(100);
            nullable = null;
            nullable1 = null;
            byte? nullable6 = nullable1;
            nullable1 = null;
            dynamicParameter2.Add("@description", description, nullable5, nullable, warehouseId, nullable6, nullable1);
            nullable = null;
            warehouseId = null;
            nullable1 = null;
            byte? nullable7 = nullable1;
            nullable1 = null;
            dynamicParameter.Add("@persistence", collection.Persistence, new DbType?(DbType.Int32), nullable, warehouseId, nullable7, nullable1);
            DynamicParameters dynamicParameter3 = dynamicParameter;
            object typeChar = collection.TypeChar;
            DbType? nullable8 = new DbType?(DbType.String);
            warehouseId = new int?(1);
            nullable = null;
            nullable1 = null;
            byte? nullable9 = nullable1;
            nullable1 = null;
            dynamicParameter3.Add("@type", typeChar, nullable8, nullable, warehouseId, nullable9, nullable1);
            nullable = null;
            warehouseId = null;
            nullable1 = null;
            byte? nullable10 = nullable1;
            nullable1 = null;
            dynamicParameter.Add("@securityGroupId", collection.SecurityGroupId, new DbType?(DbType.Int32), nullable, warehouseId, nullable10, nullable1);
            DynamicParameters dynamicParameter4 = dynamicParameter;
            DbType? nullable11 = new DbType?(DbType.String);
            warehouseId = new int?(10);
            nullable1 = null;
            byte? nullable12 = nullable1;
            nullable1 = null;
            dynamicParameter4.Add("@collectionId", null, nullable11, new ParameterDirection?(ParameterDirection.Output), warehouseId, nullable12, nullable1);
            SqlConnection connection = base.Connection;
            DynamicParameters dynamicParameter5 = dynamicParameter;
            IDbTransaction transaction = base.Transaction;
            CommandType? nullable13 = new CommandType?(CommandType.StoredProcedure);
            warehouseId = null;
            await connection.ExecuteAsync("[ddocAdmin].[sp_CreateCollection]", dynamicParameter5, transaction, warehouseId, nullable13);
            string str = dynamicParameter.Get<string>("@collectionId");
            dynamicParameter = null;
            return str;
        }

        public Task<int> UpdateCollection(DdocCollection collection)
        {
            DynamicParameters dynamicParameter = new DynamicParameters();
            string id = collection.Id;
            DbType? nullable = new DbType?(DbType.String);
            int? nullable1 = new int?(10);
            ParameterDirection? nullable2 = null;
            byte? nullable3 = null;
            byte? nullable4 = nullable3;
            nullable3 = null;
            dynamicParameter.Add("@collectionId", id, nullable, nullable2, nullable1, nullable4, nullable3);
            string name = collection.Name;
            DbType? nullable5 = new DbType?(DbType.String);
            nullable1 = new int?(50);
            nullable2 = null;
            nullable3 = null;
            byte? nullable6 = nullable3;
            nullable3 = null;
            dynamicParameter.Add("@name", name, nullable5, nullable2, nullable1, nullable6, nullable3);
            string description = collection.Description;
            DbType? nullable7 = new DbType?(DbType.String);
            nullable1 = new int?(100);
            nullable2 = null;
            nullable3 = null;
            byte? nullable8 = nullable3;
            nullable3 = null;
            dynamicParameter.Add("@description", description, nullable7, nullable2, nullable1, nullable8, nullable3);
            nullable2 = null;
            nullable1 = null;
            nullable3 = null;
            byte? nullable9 = nullable3;
            nullable3 = null;
            dynamicParameter.Add("@persistence", collection.Persistence, new DbType?(DbType.Int32), nullable2, nullable1, nullable9, nullable3);
            object typeChar = collection.TypeChar;
            DbType? nullable10 = new DbType?(DbType.String);
            nullable1 = new int?(1);
            nullable2 = null;
            nullable3 = null;
            byte? nullable11 = nullable3;
            nullable3 = null;
            dynamicParameter.Add("@type", typeChar, nullable10, nullable2, nullable1, nullable11, nullable3);
            nullable2 = null;
            nullable1 = null;
            nullable3 = null;
            byte? nullable12 = nullable3;
            nullable3 = null;
            dynamicParameter.Add("@securityGroupId", collection.SecurityGroupId, new DbType?(DbType.Int32), nullable2, nullable1, nullable12, nullable3);
            SqlConnection connection = base.Connection;
            IDbTransaction transaction = base.Transaction;
            CommandType? nullable13 = new CommandType?(CommandType.StoredProcedure);
            nullable1 = null;
            Task<int> task = connection.ExecuteAsync("[ddocAdmin].[sp_UpdateCollection]", dynamicParameter, transaction, nullable1, nullable13);
            return task;
        }

        public Task<int> UpdateCollectionFilenameTemplate(
      string collectionId,
      string filenameTemplate)
    {
      DynamicParameters dynamicParameters1 = new DynamicParameters();
      DynamicParameters dynamicParameters2 = dynamicParameters1;
      string str1 = collectionId;
      DbType? nullable1 = new DbType?(DbType.String);
      int? nullable2 = new int?(10);
      ParameterDirection? nullable3 = new ParameterDirection?();
      int? nullable4 = nullable2;
      byte? nullable5 = new byte?();
      byte? nullable6 = new byte?();
      dynamicParameters2.Add("@collectionId", (object) str1, nullable1, nullable3, nullable4, nullable5, nullable6);
      DynamicParameters dynamicParameters3 = dynamicParameters1;
      string str2 = filenameTemplate;
      DbType? nullable7 = new DbType?(DbType.String);
      nullable2 = new int?((int) byte.MaxValue);
      ParameterDirection? nullable8 = new ParameterDirection?();
      int? nullable9 = nullable2;
      byte? nullable10 = new byte?();
      byte? nullable11 = new byte?();
      dynamicParameters3.Add("@filenameTemplate", (object) str2, nullable7, nullable8, nullable9, nullable10, nullable11);
      SqlConnection connection = ((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Connection;
      DynamicParameters dynamicParameters4 = dynamicParameters1;
      IDbTransaction transaction = ((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Transaction;
      CommandType? nullable12 = new CommandType?(CommandType.StoredProcedure);
      int? nullable13 = new int?();
      CommandType? nullable14 = nullable12;
      return SqlMapper.ExecuteAsync((IDbConnection) connection, "[ddocAdmin].[sp_UpdateCollectionFilenameTemplate]", (object) dynamicParameters4, transaction, nullable13, nullable14);
    }

    public Task DeleteCollection(string collectionId)
    {
      DynamicParameters dynamicParameters1 = new DynamicParameters();
      DynamicParameters dynamicParameters2 = dynamicParameters1;
      string str = collectionId;
      DbType? nullable1 = new DbType?(DbType.String);
      int? nullable2 = new int?(10);
      ParameterDirection? nullable3 = new ParameterDirection?();
      int? nullable4 = nullable2;
      byte? nullable5 = new byte?();
      byte? nullable6 = new byte?();
      dynamicParameters2.Add("@collectionId", (object) str, nullable1, nullable3, nullable4, nullable5, nullable6);
      SqlConnection connection = ((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Connection;
      DynamicParameters dynamicParameters3 = dynamicParameters1;
      IDbTransaction transaction = ((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Transaction;
      CommandType? nullable7 = new CommandType?(CommandType.StoredProcedure);
      int? nullable8 = new int?();
      CommandType? nullable9 = nullable7;
      return (Task) SqlMapper.ExecuteAsync((IDbConnection) connection, "[ddocAdmin].[sp_DeleteCollection]", (object) dynamicParameters3, transaction, nullable8, nullable9);
    }

    public async Task<int> AddCollectionField(string collectionId, DdocField field)
    {
      DynamicParameters parameters = new DynamicParameters();
      DynamicParameters dynamicParameters1 = parameters;
      string str = collectionId;
      DbType? nullable1 = new DbType?(DbType.String);
      int? nullable2 = new int?(10);
      ParameterDirection? nullable3 = new ParameterDirection?();
      int? nullable4 = nullable2;
      byte? nullable5 = new byte?();
      byte? nullable6 = new byte?();
      dynamicParameters1.Add("@collectionId", (object) str, nullable1, nullable3, nullable4, nullable5, nullable6);
      DynamicParameters dynamicParameters2 = parameters;
      string name = field.Name;
      DbType? nullable7 = new DbType?(DbType.String);
      int? nullable8 = new int?(50);
      ParameterDirection? nullable9 = new ParameterDirection?();
      int? nullable10 = nullable8;
      byte? nullable11 = new byte?();
      byte? nullable12 = new byte?();
      dynamicParameters2.Add("@name", (object) name, nullable7, nullable9, nullable10, nullable11, nullable12);
      DynamicParameters dynamicParameters3 = parameters;
      string typeString = field.TypeString;
      DbType? nullable13 = new DbType?(DbType.String);
      int? nullable14 = new int?(20);
      ParameterDirection? nullable15 = new ParameterDirection?();
      int? nullable16 = nullable14;
      byte? nullable17 = new byte?();
      byte? nullable18 = new byte?();
      dynamicParameters3.Add("@type", (object) typeString, nullable13, nullable15, nullable16, nullable17, nullable18);
      DynamicParameters dynamicParameters4 = parameters;
      string format = field.Format;
      DbType? nullable19 = new DbType?(DbType.String);
      int? nullable20 = new int?(100);
      ParameterDirection? nullable21 = new ParameterDirection?();
      int? nullable22 = nullable20;
      byte? nullable23 = new byte?();
      byte? nullable24 = new byte?();
      dynamicParameters4.Add("@format", (object) format, nullable19, nullable21, nullable22, nullable23, nullable24);
      parameters.Add("@unique", (object) field.Unique, new DbType?(DbType.Boolean), new ParameterDirection?(), new int?(), new byte?(), new byte?());
      parameters.Add("@nullable", (object) field.Nullable, new DbType?(DbType.Boolean), new ParameterDirection?(), new int?(), new byte?(), new byte?());
      parameters.Add("@hidden", (object) field.Hidden, new DbType?(DbType.Boolean), new ParameterDirection?(), new int?(), new byte?(), new byte?());
      parameters.Add("@inheritable", (object) field.Inheritable, new DbType?(DbType.Boolean), new ParameterDirection?(), new int?(), new byte?(), new byte?());
      DynamicParameters dynamicParameters5 = parameters;
      string allowedValuesString = field.AllowedValuesString;
      DbType? nullable25 = new DbType?(DbType.String);
      int? nullable26 = new int?(5000);
      ParameterDirection? nullable27 = new ParameterDirection?();
      int? nullable28 = nullable26;
      byte? nullable29 = new byte?();
      byte? nullable30 = new byte?();
      dynamicParameters5.Add("@allowedValues", (object) allowedValuesString, nullable25, nullable27, nullable28, nullable29, nullable30);
      DynamicParameters dynamicParameters6 = parameters;
      string inMask = field.InMask;
      DbType? nullable31 = new DbType?(DbType.String);
      int? nullable32 = new int?(40);
      ParameterDirection? nullable33 = new ParameterDirection?();
      int? nullable34 = nullable32;
      byte? nullable35 = new byte?();
      byte? nullable36 = new byte?();
      dynamicParameters6.Add("@inMask", (object) inMask, nullable31, nullable33, nullable34, nullable35, nullable36);
      DynamicParameters dynamicParameters7 = parameters;
      string outMask = field.OutMask;
      DbType? nullable37 = new DbType?(DbType.String);
      int? nullable38 = new int?(40);
      ParameterDirection? nullable39 = new ParameterDirection?();
      int? nullable40 = nullable38;
      byte? nullable41 = new byte?();
      byte? nullable42 = new byte?();
      dynamicParameters7.Add("@outMask", (object) outMask, nullable37, nullable39, nullable40, nullable41, nullable42);
      parameters.Add("@globalSearch", (object) field.IncludeInGlobalSearch, new DbType?(DbType.Boolean), new ParameterDirection?(), new int?(), new byte?(), new byte?());
      parameters.Add("@computed", (object) field.Computed, new DbType?(DbType.Boolean), new ParameterDirection?(), new int?(), new byte?(), new byte?());
      parameters.Add("@fieldId", (object) null, new DbType?(DbType.Int32), new ParameterDirection?(ParameterDirection.Output), new int?(), new byte?(), new byte?());
      SqlConnection connection = ((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Connection;
      DynamicParameters dynamicParameters8 = parameters;
      IDbTransaction transaction = ((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Transaction;
      CommandType? nullable43 = new CommandType?(CommandType.StoredProcedure);
      int? nullable44 = new int?();
      CommandType? nullable45 = nullable43;
      int num1 = await SqlMapper.ExecuteAsync((IDbConnection) connection, "[ddocAdmin].[sp_AddCollectionField]", (object) dynamicParameters8, transaction, nullable44, nullable45);
      int num2 = (int) parameters.Get<int>("@fieldId");
      parameters = (DynamicParameters) null;
      return num2;
    }

        public Task<int> UpdateCollectionField(DdocField field)
        {
            DynamicParameters dynamicParameter = new DynamicParameters();
            ParameterDirection? nullable = null;
            int? nullable1 = null;
            byte? nullable2 = null;
            byte? nullable3 = nullable2;
            nullable2 = null;
            dynamicParameter.Add("@fieldId", field.Id, new DbType?(DbType.Int32), nullable, nullable1, nullable3, nullable2);
            string name = field.Name;
            DbType? nullable4 = new DbType?(DbType.String);
            nullable1 = new int?(50);
            nullable = null;
            nullable2 = null;
            byte? nullable5 = nullable2;
            nullable2 = null;
            dynamicParameter.Add("@name", name, nullable4, nullable, nullable1, nullable5, nullable2);
            string typeString = field.TypeString;
            DbType? nullable6 = new DbType?(DbType.String);
            nullable1 = new int?(20);
            nullable = null;
            nullable2 = null;
            byte? nullable7 = nullable2;
            nullable2 = null;
            dynamicParameter.Add("@type", typeString, nullable6, nullable, nullable1, nullable7, nullable2);
            string format = field.Format;
            DbType? nullable8 = new DbType?(DbType.String);
            nullable1 = new int?(100);
            nullable = null;
            nullable2 = null;
            byte? nullable9 = nullable2;
            nullable2 = null;
            dynamicParameter.Add("@format", format, nullable8, nullable, nullable1, nullable9, nullable2);
            nullable = null;
            nullable1 = null;
            nullable2 = null;
            byte? nullable10 = nullable2;
            nullable2 = null;
            dynamicParameter.Add("@unique", field.Unique, new DbType?(DbType.Boolean), nullable, nullable1, nullable10, nullable2);
            nullable = null;
            nullable1 = null;
            nullable2 = null;
            byte? nullable11 = nullable2;
            nullable2 = null;
            dynamicParameter.Add("@nullable", field.Nullable, new DbType?(DbType.Boolean), nullable, nullable1, nullable11, nullable2);
            nullable = null;
            nullable1 = null;
            nullable2 = null;
            byte? nullable12 = nullable2;
            nullable2 = null;
            dynamicParameter.Add("@hidden", field.Hidden, new DbType?(DbType.Boolean), nullable, nullable1, nullable12, nullable2);
            nullable = null;
            nullable1 = null;
            nullable2 = null;
            byte? nullable13 = nullable2;
            nullable2 = null;
            dynamicParameter.Add("@inheritable", field.Inheritable, new DbType?(DbType.Boolean), nullable, nullable1, nullable13, nullable2);
            string allowedValuesString = field.AllowedValuesString;
            DbType? nullable14 = new DbType?(DbType.String);
            nullable1 = new int?(5000);
            nullable = null;
            nullable2 = null;
            byte? nullable15 = nullable2;
            nullable2 = null;
            dynamicParameter.Add("@allowedValues", allowedValuesString, nullable14, nullable, nullable1, nullable15, nullable2);
            string inMask = field.InMask;
            DbType? nullable16 = new DbType?(DbType.String);
            nullable1 = new int?(40);
            nullable = null;
            nullable2 = null;
            byte? nullable17 = nullable2;
            nullable2 = null;
            dynamicParameter.Add("@inMask", inMask, nullable16, nullable, nullable1, nullable17, nullable2);
            string outMask = field.OutMask;
            DbType? nullable18 = new DbType?(DbType.String);
            nullable1 = new int?(40);
            nullable = null;
            nullable2 = null;
            byte? nullable19 = nullable2;
            nullable2 = null;
            dynamicParameter.Add("@outMask", outMask, nullable18, nullable, nullable1, nullable19, nullable2);
            nullable = null;
            nullable1 = null;
            nullable2 = null;
            byte? nullable20 = nullable2;
            nullable2 = null;
            dynamicParameter.Add("@globalSearch", field.IncludeInGlobalSearch, new DbType?(DbType.Boolean), nullable, nullable1, nullable20, nullable2);
            nullable = null;
            nullable1 = null;
            nullable2 = null;
            byte? nullable21 = nullable2;
            nullable2 = null;
            dynamicParameter.Add("@computed", field.Computed, new DbType?(DbType.Boolean), nullable, nullable1, nullable21, nullable2);
            SqlConnection connection = base.Connection;
            IDbTransaction transaction = base.Transaction;
            CommandType? nullable22 = new CommandType?(CommandType.StoredProcedure);
            nullable1 = null;
            Task<int> task = connection.ExecuteAsync("[ddocAdmin].[sp_UpdateCollectionField]", dynamicParameter, transaction, nullable1, nullable22);
            return task;
        }

        public Task MoveCollectionField(int fieldId, bool direction)
    {
      DynamicParameters dynamicParameters1 = new DynamicParameters();
      dynamicParameters1.Add("@fieldId", (object) fieldId, new DbType?(DbType.Int32), new ParameterDirection?(), new int?(), new byte?(), new byte?());
      dynamicParameters1.Add("@direction", (object) direction, new DbType?(DbType.Boolean), new ParameterDirection?(), new int?(), new byte?(), new byte?());
      SqlConnection connection = ((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Connection;
      DynamicParameters dynamicParameters2 = dynamicParameters1;
      IDbTransaction transaction = ((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Transaction;
      CommandType? nullable1 = new CommandType?(CommandType.StoredProcedure);
      int? nullable2 = new int?();
      CommandType? nullable3 = nullable1;
      return (Task) SqlMapper.ExecuteAsync((IDbConnection) connection, "[ddocAdmin].[sp_MoveCollectionField]", (object) dynamicParameters2, transaction, nullable2, nullable3);
    }

    public Task DeleteCollectionField(int fieldId)
    {
      DynamicParameters dynamicParameters1 = new DynamicParameters();
      dynamicParameters1.Add("@fieldId", (object) fieldId, new DbType?(DbType.Int32), new ParameterDirection?(), new int?(), new byte?(), new byte?());
      SqlConnection connection = ((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Connection;
      DynamicParameters dynamicParameters2 = dynamicParameters1;
      IDbTransaction transaction = ((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Transaction;
      CommandType? nullable1 = new CommandType?(CommandType.StoredProcedure);
      int? nullable2 = new int?();
      CommandType? nullable3 = nullable1;
      return (Task) SqlMapper.ExecuteAsync((IDbConnection) connection, "[ddocAdmin].[sp_DeleteCollectionField]", (object) dynamicParameters2, transaction, nullable2, nullable3);
    }

    public Task FinalizeCollections()
    {
      SqlConnection connection = ((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Connection;
      IDbTransaction transaction = ((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Transaction;
      CommandType? nullable1 = new CommandType?(CommandType.StoredProcedure);
      int? nullable2 = new int?();
      CommandType? nullable3 = nullable1;
      return (Task) SqlMapper.ExecuteAsync((IDbConnection) connection, "[ddocAdmin].[sp_FinalizeCollections]", (object) null, transaction, nullable2, nullable3);
    }

    public async Task<bool> GetCollectionLockStatus()
    {
      DynamicParameters parameters = new DynamicParameters();
      parameters.Add("@lockStatus", (object) null, new DbType?(DbType.Boolean), new ParameterDirection?(ParameterDirection.Output), new int?(), new byte?(), new byte?());
      SqlConnection connection = ((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Connection;
      DynamicParameters dynamicParameters = parameters;
      IDbTransaction transaction = ((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Transaction;
      CommandType? nullable1 = new CommandType?(CommandType.StoredProcedure);
      int? nullable2 = new int?();
      CommandType? nullable3 = nullable1;
      int num = await SqlMapper.ExecuteAsync((IDbConnection) connection, "[ddocAdmin].[sp_GetCollectionLockStatus]", (object) dynamicParameters, transaction, nullable2, nullable3);
      bool flag = (bool) parameters.Get<bool>("@lockStatus");
      parameters = (DynamicParameters) null;
      return flag;
    }

        public async Task<bool> CollectionHasData(string collectionId)
        {
            Query query = base.db.Query(String.Concat("G_CAMPOS_", collectionId));
            String[] strArrays = new String[] { "GID" };
            return await QueryAggregateExtensionsAsync.CountAsync<int>(query, strArrays) > 0;
        }

        public Task CreateCollectionField(int fieldId)
    {
      DynamicParameters dynamicParameters1 = new DynamicParameters();
      dynamicParameters1.Add("@fieldId", (object) fieldId, new DbType?(DbType.Int32), new ParameterDirection?(), new int?(), new byte?(), new byte?());
      SqlConnection connection = ((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Connection;
      DynamicParameters dynamicParameters2 = dynamicParameters1;
      IDbTransaction transaction = ((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Transaction;
      CommandType? nullable1 = new CommandType?(CommandType.StoredProcedure);
      int? nullable2 = new int?();
      CommandType? nullable3 = nullable1;
      return (Task) SqlMapper.ExecuteAsync((IDbConnection) connection, "[ddocAdmin].[sp_CreateCollectionField]", (object) dynamicParameters2, transaction, nullable2, nullable3);
    }

    public Task AlterCollectionField(DdocField field)
    {
      DynamicParameters dynamicParameters1 = new DynamicParameters();
      dynamicParameters1.Add("@fieldId", (object) field.Id, new DbType?(DbType.Int32), new ParameterDirection?(), new int?(), new byte?(), new byte?());
      SqlConnection connection = ((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Connection;
      DynamicParameters dynamicParameters2 = dynamicParameters1;
      IDbTransaction transaction = ((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Transaction;
      CommandType? nullable1 = new CommandType?(CommandType.StoredProcedure);
      int? nullable2 = new int?();
      CommandType? nullable3 = nullable1;
      return (Task) SqlMapper.ExecuteAsync((IDbConnection) connection, "[ddocAdmin].[sp_AlterCollectionField]", (object) dynamicParameters2, transaction, nullable2, nullable3);
    }

    public Task DropCollectionField(int fieldId)
    {
      DynamicParameters dynamicParameters1 = new DynamicParameters();
      dynamicParameters1.Add("@fieldId", (object) fieldId, new DbType?(DbType.Int32), new ParameterDirection?(), new int?(), new byte?(), new byte?());
      SqlConnection connection = ((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Connection;
      DynamicParameters dynamicParameters2 = dynamicParameters1;
      IDbTransaction transaction = ((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Transaction;
      CommandType? nullable1 = new CommandType?(CommandType.StoredProcedure);
      int? nullable2 = new int?();
      CommandType? nullable3 = nullable1;
      return (Task) SqlMapper.ExecuteAsync((IDbConnection) connection, "[ddocAdmin].[sp_DropCollectionField]", (object) dynamicParameters2, transaction, nullable2, nullable3);
    }

    public Task CreateCollectionFieldsTable(string collectionId)
    {
      DynamicParameters dynamicParameters1 = new DynamicParameters();
      DynamicParameters dynamicParameters2 = dynamicParameters1;
      string str = collectionId;
      DbType? nullable1 = new DbType?(DbType.String);
      int? nullable2 = new int?(10);
      ParameterDirection? nullable3 = new ParameterDirection?();
      int? nullable4 = nullable2;
      byte? nullable5 = new byte?();
      byte? nullable6 = new byte?();
      dynamicParameters2.Add("@collectionId", (object) str, nullable1, nullable3, nullable4, nullable5, nullable6);
      SqlConnection connection = ((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Connection;
      DynamicParameters dynamicParameters3 = dynamicParameters1;
      IDbTransaction transaction = ((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Transaction;
      CommandType? nullable7 = new CommandType?(CommandType.StoredProcedure);
      int? nullable8 = new int?();
      CommandType? nullable9 = nullable7;
      return (Task) SqlMapper.ExecuteAsync((IDbConnection) connection, "[ddocAdmin].[sp_CreateFieldsTable]", (object) dynamicParameters3, transaction, nullable8, nullable9);
    }

    //[SpecialName]
    //IDbTransaction ICommonDAL.get_Transaction() => ((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Transaction;

    //[SpecialName]
    //void ICommonDAL.set_Transaction(IDbTransaction value) => ((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Transaction = value;
  }
}
