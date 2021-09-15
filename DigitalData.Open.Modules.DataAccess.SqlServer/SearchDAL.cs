// Decompiled with JetBrains decompiler
// Type: DigitalData.DDoc.Modules.DataAccess.SearchDAL
// Assembly: DigitalData.DDoc.Modules.DataAccess.SqlServer, Version=4.210.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EB5ECC0D-3654-4980-9D29-200BC39CB926
// Assembly location: D:\proyectos\ddocx\DigitalData.DDoc.UI.Webx\DigitalData.DDoc.UI.Web\bin\DigitalData.DDoc.Modules.DataAccess.SqlServer.dll

using Dapper;
using DigitalData.Common.DataAccess;
using DigitalData.Common.DataAccess.QueryExecution;
using DigitalData.Common.DataAccess.SqlServer;
using DigitalData.Common.Entities;
using DigitalData.Open.Common;
using DigitalData.Open.Common.Api.DataAccess;
using DigitalData.Open.Common.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace DigitalData.Open.Modules.DataAccess
{
  public class SearchDAL : SqlServerDAL, ISearchDAL, ICommonDAL
  {
    public SearchDAL(SqlConnection connection, SqlTransaction transaction = null)
      : base(connection, transaction)
    {
    }

        public Task<IEnumerable<DdocChildEntity>> GlobalSearch(string searchText, CollectionType collectionType, string securityGroupsCsv)
        {
            string str = (collectionType == CollectionType.D ? "DOCUMENTOS" : "FOLDERS");
            Task<IEnumerable<DdocChildEntity>> async = QueryExtensionsAsync.GetAsync<DdocChildEntity>(base.db.Query(String.Concat("G_", str, " as I")).WhereLike("I.DATAKEY", String.Concat("%", searchText, "%"), false, null).WhereIn<string>("I.SEGURIDAD", securityGroupsCsv.Split(new Char[] { ',' })).Select(new String[] { "I.GID as Id", "I.GID_COLECCION as CollectionId" }), new int?(3600));
            return async;
        }

        public Task<IEnumerable<DdocChildEntity>> GlobalSearch2(string searchText, CollectionType collectionType, string securityGroupsCsv, int maxHits)
        {
            string str = (collectionType == CollectionType.D ? "DOCUMENTOS" : "FOLDERS");
            Task<IEnumerable<DdocChildEntity>> async = QueryExtensionsAsync.GetAsync<DdocChildEntity>(base.db.Query(String.Concat("G_", str)).WhereLike("DATAKEY", String.Concat("%", searchText, "%"), false, null).WhereIn<string>("SEGURIDAD", securityGroupsCsv.Split(new Char[] { ',' })).Select(new String[] { "GID as Id", "GID_COLECCION as CollectionId", "NOMBRE as Name", "FECHA_CREACION as CreationDate", "SEGURIDAD as SecurityGroupId", "FECHA_BORRADO as DeletionDate", "FECHA_ACTUALIZACION as ModificationDate" }).OrderByDesc(new String[] { "FECHA_CREACION" }).Limit(maxHits), new int?(3600));
            return async;
        }

        public async Task<ValueTuple<long, IEnumerable<DdocChildEntity>>> AllFieldsSearch(string searchText, string collectionId, CollectionType collectionType, List<DdocField> collectionField, string securityGroupsCsv, int page, int pageSize, string sortBy, int sortDirection)
        {
            DynamicParameters dynamicParameter = new DynamicParameters();
            DynamicParameters dynamicParameter1 = dynamicParameter;
            string str = searchText;
            DbType? nullable = new DbType?(DbType.String);
            int? nullable1 = new int?(500);
            ParameterDirection? nullable2 = null;
            byte? nullable3 = null;
            byte? nullable4 = nullable3;
            nullable3 = null;
            dynamicParameter1.Add("queryText", str, nullable, nullable2, nullable1, nullable4, nullable3);
            DynamicParameters dynamicParameter2 = dynamicParameter;
            string str1 = collectionId;
            DbType? nullable5 = new DbType?(DbType.String);
            nullable1 = new int?(10);
            nullable2 = null;
            nullable3 = null;
            byte? nullable6 = nullable3;
            nullable3 = null;
            dynamicParameter2.Add("collectionId", str1, nullable5, nullable2, nullable1, nullable6, nullable3);
            nullable2 = null;
            nullable1 = null;
            nullable3 = null;
            byte? nullable7 = nullable3;
            nullable3 = null;
            dynamicParameter.Add("collectionType", collectionType, new DbType?(DbType.Int32), nullable2, nullable1, nullable7, nullable3);
            nullable2 = null;
            nullable1 = null;
            nullable3 = null;
            byte? nullable8 = nullable3;
            nullable3 = null;
            dynamicParameter.Add("page", page, new DbType?(DbType.Int32), nullable2, nullable1, nullable8, nullable3);
            DynamicParameters dynamicParameter3 = dynamicParameter;
            object obj = pageSize;
            DbType? nullable9 = new DbType?(DbType.String);
            nullable1 = new int?(10);
            nullable2 = null;
            nullable3 = null;
            byte? nullable10 = nullable3;
            nullable3 = null;
            dynamicParameter3.Add("pageSize", obj, nullable9, nullable2, nullable1, nullable10, nullable3);
            nullable1 = null;
            nullable3 = null;
            byte? nullable11 = nullable3;
            nullable3 = null;
            dynamicParameter.Add("totalRecords", null, new DbType?(DbType.Int32), new ParameterDirection?(ParameterDirection.Output), nullable1, nullable11, nullable3);
            DynamicParameters dynamicParameter4 = dynamicParameter;
            string str2 = sortBy;
            DbType? nullable12 = new DbType?(DbType.String);
            nullable1 = new int?(10);
            nullable2 = null;
            nullable3 = null;
            byte? nullable13 = nullable3;
            nullable3 = null;
            dynamicParameter4.Add("sortedBy", str2, nullable12, nullable2, nullable1, nullable13, nullable3);
            DynamicParameters dynamicParameter5 = dynamicParameter;
            object obj1 = sortDirection;
            DbType? nullable14 = new DbType?(DbType.Int32);
            nullable1 = new int?(10);
            nullable2 = null;
            nullable3 = null;
            byte? nullable15 = nullable3;
            nullable3 = null;
            dynamicParameter5.Add("sortDir", obj1, nullable14, nullable2, nullable1, nullable15, nullable3);
            DynamicParameters dynamicParameter6 = dynamicParameter;
            string str3 = securityGroupsCsv.Replace(",", "','");
            DbType? nullable16 = new DbType?(DbType.String);
            nullable1 = new int?(8000);
            nullable2 = null;
            nullable3 = null;
            byte? nullable17 = nullable3;
            nullable3 = null;
            dynamicParameter6.Add("securityGroupsCsv", str3, nullable16, nullable2, nullable1, nullable17, nullable3);
            SqlConnection connection = base.Connection;
            DynamicParameters dynamicParameter7 = dynamicParameter;
            IDbTransaction transaction = base.Transaction;
            CommandType? nullable18 = new CommandType?(CommandType.StoredProcedure);
            nullable1 = null;
            IEnumerable<object> objs = await connection.QueryAsync("[ddoc].[sp_AllFieldsSearch]", dynamicParameter7, transaction, nullable1, nullable18);
            IEnumerable<IReadOnlyDictionary<string, object>> readOnlyDictionaries = objs.Cast<IReadOnlyDictionary<string, object>>();
            objs = null;
            List<DdocChildEntity> list = (
                from d in readOnlyDictionaries
                select new DdocChildEntity()
                {
                    Id = d["GID"].ToString(),
                    Data = (
                        from f in collectionField
                        select new DdocField()
                        {
                            Id = f.Id,
                            Name = f.Name,
                            Value = d.GetValue(f.Id, f.Type, f.OutMask),
                            Type = f.Type,
                            Format = f.Format,
                            Nullable = f.Nullable,
                            Hidden = f.Hidden,
                            AllowedValuesString = f.AllowedValuesString,
                            InMask = f.InMask,
                            OutMask = f.OutMask,
                            IncludeInGlobalSearch = f.IncludeInGlobalSearch
                        }).ToList<DdocField>()
                }).ToList<DdocChildEntity>();
            int num = dynamicParameter.Get<int>("totalRecords");
            ValueTuple<long, IEnumerable<DdocChildEntity>> valueTuple = new ValueTuple<long, IEnumerable<DdocChildEntity>>((long)num, list);
            dynamicParameter = null;
            readOnlyDictionaries = null;
            list = null;
            return valueTuple;
        }

        public Task<IEnumerable<string>> SearchFieldValues(string collectionId, int fieldId, Comparison op, string textQuery)
        {
            return QueryExtensionsAsync.GetAsync<string>(base.db.Query(String.Concat("G_CAMPOS_", collectionId)).Where(String.Format("C{0}", fieldId), op, textQuery).Select(new String[] { String.Format("C{0}", fieldId) }), null);
        }

        public Task<IEnumerable<dynamic>> SearchCollectionValues(string collectionId, int fieldId, Comparison op, string textQuery)
        {
            int? nullable = null;
            return QueryExtensionsAsync.GetAsync(base.db.Query(String.Concat("G_CAMPOS_", collectionId)).Where(String.Format("C{0}", fieldId), op, textQuery), nullable);
        }

        //[SpecialName]
        //IDbTransaction ICommonDAL.get_Transaction() => ((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Transaction;

        //[SpecialName]
        //void ICommonDAL.set_Transaction(IDbTransaction value) => ((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Transaction = value;
    }
}
