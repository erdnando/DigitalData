using Dapper;
using DigitalData.Common.DataAccess;
using DigitalData.Common.DataAccess.QueryExecution;
using DigitalData.Common.DataAccess.SqlServer;
using DigitalData.Open.Common;
using DigitalData.Open.Common.Api;
using DigitalData.Open.Common.Api.DataAccess;
using DigitalData.Open.Common.Entities;
using Microsoft.CSharp.RuntimeBinder;
using SqlKata;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace DigitalData.DDoc.Modules.DataAccess
{
    public class FolderDAL : SqlServerDAL, IFolderDAL, ICommonDAL
    {
        public FolderDAL(SqlConnection connection, SqlTransaction transaction = null) : base(connection, transaction)
        {
        }

        public async Task<int> CountFolders(string collectionId, IEnumerable<int> securityGroups)
        {
            DynamicParameters dynamicParameter = new DynamicParameters();
            DynamicParameters dynamicParameter1 = dynamicParameter;
            string str = collectionId;
            DbType? nullable = new DbType?(DbType.String);
            int? nullable1 = new int?(10);
            ParameterDirection? nullable2 = null;
            byte? nullable3 = null;
            byte? nullable4 = nullable3;
            nullable3 = null;
            dynamicParameter1.Add("collectionId", str, nullable, nullable2, nullable1, nullable4, nullable3);
            DbType? nullable5 = null;
            nullable2 = null;
            nullable1 = null;
            nullable3 = null;
            byte? nullable6 = nullable3;
            nullable3 = null;
            dynamicParameter.Add("securityGroups", securityGroups.ToTableValuedParameter<int>(), nullable5, nullable2, nullable1, nullable6, nullable3);
            nullable1 = null;
            nullable3 = null;
            byte? nullable7 = nullable3;
            nullable3 = null;
            dynamicParameter.Add("total", null, new DbType?(DbType.Int32), new ParameterDirection?(ParameterDirection.Output), nullable1, nullable7, nullable3);
            SqlConnection connection = base.Connection;
            DynamicParameters dynamicParameter2 = dynamicParameter;
            IDbTransaction transaction = base.Transaction;
            CommandType? nullable8 = new CommandType?(CommandType.StoredProcedure);
            nullable1 = null;
            await connection.ExecuteAsync("[ddoc].[sp_CountFolders]", dynamicParameter2, transaction, nullable1, nullable8);
            int num = dynamicParameter.Get<int>("total");
            dynamicParameter = null;
            return num;
        }

        public async Task<string> CreateFolder(DdocFolder folder)
        {
            DynamicParameters dynamicParameter = new DynamicParameters();
            DynamicParameters dynamicParameter1 = dynamicParameter;
            string collectionId = folder.CollectionId;
            DbType? nullable = new DbType?(DbType.String);
            int? nullable1 = new int?(10);
            ParameterDirection? nullable2 = null;
            byte? nullable3 = null;
            byte? nullable4 = nullable3;
            nullable3 = null;
            dynamicParameter1.Add("@collectionId", collectionId, nullable, nullable2, nullable1, nullable4, nullable3);
            DynamicParameters dynamicParameter2 = dynamicParameter;
            string name = folder.Name;
            DbType? nullable5 = new DbType?(DbType.String);
            nullable1 = new int?(100);
            nullable2 = null;
            nullable3 = null;
            byte? nullable6 = nullable3;
            nullable3 = null;
            dynamicParameter2.Add("@name", name, nullable5, nullable2, nullable1, nullable6, nullable3);
            DynamicParameters dynamicParameter3 = dynamicParameter;
            string dataKey = folder.DataKey;
            DbType? nullable7 = new DbType?(DbType.String);
            nullable1 = new int?(5000);
            nullable2 = null;
            nullable3 = null;
            byte? nullable8 = nullable3;
            nullable3 = null;
            dynamicParameter3.Add("@dataKey", dataKey, nullable7, nullable2, nullable1, nullable8, nullable3);
            nullable2 = null;
            nullable1 = null;
            nullable3 = null;
            byte? nullable9 = nullable3;
            nullable3 = null;
            dynamicParameter.Add("@security", folder.SecurityGroupId, new DbType?(DbType.Int32), nullable2, nullable1, nullable9, nullable3);
            DynamicParameters dynamicParameter4 = dynamicParameter;
            DbType? nullable10 = new DbType?(DbType.String);
            nullable1 = new int?(10);
            nullable3 = null;
            byte? nullable11 = nullable3;
            nullable3 = null;
            dynamicParameter4.Add("@folderId", null, nullable10, new ParameterDirection?(ParameterDirection.Output), nullable1, nullable11, nullable3);
            SqlConnection connection = base.Connection;
            DynamicParameters dynamicParameter5 = dynamicParameter;
            IDbTransaction transaction = base.Transaction;
            CommandType? nullable12 = new CommandType?(CommandType.StoredProcedure);
            nullable1 = null;
            await connection.ExecuteAsync("[ddoc].[sp_CreateFolder]", dynamicParameter5, transaction, nullable1, nullable12);
            string str = dynamicParameter.Get<string>("@folderId");
            string str1 = str;
            dynamicParameter = null;
            str = null;
            return str1;
        }

        public Task CreateFolderData(string folderId, string collectionId, IEnumerable<DdocField> folderData)
        {
            Query query = base.db.Query(String.Concat("G_CAMPOS_", collectionId));
            Dictionary<string, object> strs = new Dictionary<string, object>()
            {
                { "GID", folderId }
            };
            foreach (DdocField ddocField in
                from f in folderData
                where !f.Computed
                select f)
            {
                strs.Add(String.Format("C{0}", ddocField.Id), (dynamic)ddocField.GetValue());
            }
            return QueryExtensionsAsync.InsertAsync(query, (IReadOnlyDictionary<string, object>)strs, null);
        }

        public Task<int> DeleteFolder(string folderId, string user)
        {
            DynamicParameters dynamicParameter = new DynamicParameters();
            DbType? nullable = new DbType?(DbType.String);
            int? nullable1 = new int?(10);
            ParameterDirection? nullable2 = null;
            byte? nullable3 = null;
            byte? nullable4 = nullable3;
            nullable3 = null;
            dynamicParameter.Add("@folderId", folderId, nullable, nullable2, nullable1, nullable4, nullable3);
            DbType? nullable5 = new DbType?(DbType.String);
            nullable1 = new int?(50);
            nullable2 = null;
            nullable3 = null;
            byte? nullable6 = nullable3;
            nullable3 = null;
            dynamicParameter.Add("@user", user, nullable5, nullable2, nullable1, nullable6, nullable3);
            SqlConnection connection = base.Connection;
            IDbTransaction transaction = base.Transaction;
            CommandType? nullable7 = new CommandType?(CommandType.StoredProcedure);
            nullable1 = null;
            Task<int> task = connection.ExecuteAsync("[ddoc].[sp_DeleteFolder]", dynamicParameter, transaction, nullable1, nullable7);
            return task;
        }
        //DigitalData.DDoc.Common.Api.DataAccess.ICommonDAL.
        IDbTransaction get_Transaction()
        {
            return base.Transaction;    
        }
        //DigitalData.DDoc.Common.Api.DataAccess.ICommonDAL.
        void set_Transaction(IDbTransaction value)
        {
            base.Transaction = value;
        }

        public Task<bool> FolderExists(string folderId)
        {
            int? nullable = null;
            return QueryExtensionsAsync.ExistsAsync(base.db.Query("G_FOLDERS").Where("GID", folderId), nullable);
        }

        public Task<bool> FolderExists(DdocFolder folder, List<int> includedFields = null, List<int> excludedFields = null)
        {
            Query query = base.db.Query("G_FOLDERS as D").Join(String.Concat("G_CAMPOS_", folder.CollectionId, " as F"), "F.GID", "D.GID", "=", "inner join");
            if (includedFields != null)
            {
                folder.Data = folder.Data.Except<DdocField>(
                    from f in folder.Data
                    where !includedFields.Contains(f.Id)
                    select f).ToList<DdocField>();
            }
            else if (excludedFields != null)
            {
                folder.Data = folder.Data.Except<DdocField>(
                    from f in folder.Data
                    where excludedFields.Contains(f.Id)
                    select f).ToList<DdocField>();
            }
            foreach (DdocField datum in folder.Data)
            {
                query.Where(String.Format("C{0}", datum.Id), (dynamic)datum.GetValue());
            }
            return QueryExtensionsAsync.ExistsAsync(query, null);
        }

        public async Task<IEnumerable<DdocFolder>> FolderTextSearch(string collectionId, List<DdocField> fields, string searchText)
        {
            Query query = base.db.Query(String.Concat("G_CAMPOS_", collectionId));
            List<string> strs = new List<string>()
            {
                "GID"
            };
            foreach (DdocField field in fields)
            {
                strs.Add(String.Format("C{0}", field.Id));
                query.OrWhereLike(String.Format("C{0}", field.Id), String.Concat("%", searchText, "%"), false, null);
            }
            query.Select(strs.ToArray());
            IEnumerable<object> async = await QueryExtensionsAsync.GetAsync(query, null);
            IEnumerable<IReadOnlyDictionary<string, object>> readOnlyDictionaries = async.Cast<IReadOnlyDictionary<string, object>>();
            async = null;
            List<DdocFolder> list = (
                from d in readOnlyDictionaries
                select new DdocFolder()
                {
                    Id = d["GID"].ToString(),
                    Data = (
                        from f in fields
                        select new DdocField()
                        {
                            Id = f.Id,
                            Name = f.Name,
                            Value = d.GetValue(f.Id, f.Type, f.OutMask),
                            TypeString = f.TypeString,
                            Format = f.Format,
                            Nullable = f.Nullable,
                            Hidden = f.Hidden,
                            AllowedValuesString = f.AllowedValuesString,
                            InMask = f.InMask,
                            OutMask = f.OutMask,
                            IncludeInGlobalSearch = f.IncludeInGlobalSearch,
                            Computed = f.Computed
                        }).ToList<DdocField>()
                }).ToList<DdocFolder>();
            IEnumerable<DdocFolder> ddocFolders = list;
            query = null;
            strs = null;
            readOnlyDictionaries = null;
            list = null;
            return ddocFolders;
        }

        public Task<DdocFolder> GetFolder(string folderId)
        {
            DynamicParameters dynamicParameter = new DynamicParameters();
            DbType? nullable = new DbType?(DbType.String);
            int? nullable1 = new int?(10);
            ParameterDirection? nullable2 = null;
            byte? nullable3 = null;
            byte? nullable4 = nullable3;
            nullable3 = null;
            dynamicParameter.Add("@folderId", folderId, nullable, nullable2, nullable1, nullable4, nullable3);
            SqlConnection connection = base.Connection;
            IDbTransaction transaction = base.Transaction;
            CommandType? nullable5 = new CommandType?(CommandType.StoredProcedure);
            nullable1 = null;
            Task<DdocFolder> task = connection.QuerySingleOrDefaultAsync<DdocFolder>("[ddoc].[sp_GetFolder]", dynamicParameter, transaction, nullable1, nullable5);
            return task;
        }

        public async Task<IEnumerable<DdocChildEntity>> GetFolderContents(string childCollectionId, CollectionType childCollectionType, Dictionary<string, string> filters, List<DdocField> childFields)
        {
            string str;
            Query query = base.db.Query(String.Concat("G_CAMPOS_", childCollectionId, " as C"));
            str = (childCollectionType == CollectionType.D ? "DOCUMENTOS" : "FOLDERS");
            Query query1 = query.Join(String.Concat("G_", str, " as I"), "I.GID", "C.GID", "=", "inner join");
            foreach (KeyValuePair<string, string> filter in filters)
            {
                query1.Where(filter.Key, filter.Value);
                //filter = new KeyValuePair<string, string>();
            }
            IEnumerable<object> async = await QueryExtensionsAsync.GetAsync(query1, null);
            List<IReadOnlyDictionary<string, object>> list = async.Cast<IReadOnlyDictionary<string, object>>().ToList<IReadOnlyDictionary<string, object>>();
            async = null;
            List<DdocChildEntity> ddocChildEntities = (
                from i in list
                select new DdocChildEntity()
                {
                    Id = i["GID"].ToString(),
                    Data = (
                        from f in childFields
                        select new DdocField()
                        {
                            Id = f.Id,
                            Name = f.Name,
                            Value = i.GetValue(f.Id, f.Type, f.OutMask),
                            TypeString = f.TypeString,
                            Format = f.Format,
                            Nullable = f.Nullable,
                            Hidden = f.Hidden,
                            AllowedValuesString = f.AllowedValuesString,
                            InMask = f.InMask,
                            OutMask = f.OutMask,
                            IncludeInGlobalSearch = f.IncludeInGlobalSearch,
                            Computed = f.Computed
                        }).ToList<DdocField>()
                }).ToList<DdocChildEntity>();
            IEnumerable<DdocChildEntity> ddocChildEntities1 = ddocChildEntities;
            query1 = null;
            list = null;
            ddocChildEntities = null;
            return ddocChildEntities1;
        }

        public async Task<IEnumerable<DdocField>> GetFolderData(string folderId, string collectionId, List<DdocField> fields)
        {
            Query query = base.db.Query(String.Concat("G_CAMPOS_", collectionId)).Where("GID", folderId);
            List<string> strs = new List<string>();
            foreach (DdocField field in fields)
            {
                strs.Add(String.Format("C{0}", field.Id));
            }
            query.Select(strs.ToArray());
            dynamic obj = await QueryExtensionsAsync.SingleAsync(query, null);
            IReadOnlyDictionary<string, object> strs1 = (IReadOnlyDictionary<string, object>)obj;
            obj = null;
            List<DdocField> list = (
                from f in fields
                select new DdocField()
                {
                    Id = f.Id,
                    TypeString = f.TypeString,
                    Format = f.Format,
                    Name = f.Name,
                    Nullable = f.Nullable,
                    Hidden = f.Hidden,
                    Value = strs1.GetValue(f.Id, f.Type, f.OutMask),
                    AllowedValuesString = f.AllowedValuesString,
                    InMask = f.InMask,
                    OutMask = f.OutMask,
                    IncludeInGlobalSearch = f.IncludeInGlobalSearch,
                    Computed = f.Computed
                }).ToList<DdocField>();
            IEnumerable<DdocField> ddocFields = list;
            query = null;
            strs = null;
            list = null;
            return ddocFields;
        }

        public Task<IEnumerable<DdocFolder>> GetFolders(string collectionId, IEnumerable<int> securityGroups, int page, int pageSize, string sortBy, int sortDirection)
        {
            DynamicParameters dynamicParameter = new DynamicParameters();
            DbType? nullable = new DbType?(DbType.String);
            int? nullable1 = new int?(10);
            ParameterDirection? nullable2 = null;
            byte? nullable3 = null;
            byte? nullable4 = nullable3;
            nullable3 = null;
            dynamicParameter.Add("collectionId", collectionId, nullable, nullable2, nullable1, nullable4, nullable3);
            DbType? nullable5 = null;
            nullable2 = null;
            nullable1 = null;
            nullable3 = null;
            byte? nullable6 = nullable3;
            nullable3 = null;
            dynamicParameter.Add("securityGroups", securityGroups.ToTableValuedParameter<int>(), nullable5, nullable2, nullable1, nullable6, nullable3);
            nullable2 = null;
            nullable1 = null;
            nullable3 = null;
            byte? nullable7 = nullable3;
            nullable3 = null;
            dynamicParameter.Add("page", page, new DbType?(DbType.Int32), nullable2, nullable1, nullable7, nullable3);
            nullable2 = null;
            nullable1 = null;
            nullable3 = null;
            byte? nullable8 = nullable3;
            nullable3 = null;
            dynamicParameter.Add("pageSize", pageSize, new DbType?(DbType.Int32), nullable2, nullable1, nullable8, nullable3);
            if (!String.IsNullOrEmpty(sortBy))
            {
                nullable5 = null;
                nullable2 = null;
                nullable1 = null;
                nullable3 = null;
                byte? nullable9 = nullable3;
                nullable3 = null;
                dynamicParameter.Add("sortedBy", sortBy, nullable5, nullable2, nullable1, nullable9, nullable3);
                nullable5 = null;
                nullable2 = null;
                nullable1 = null;
                nullable3 = null;
                byte? nullable10 = nullable3;
                nullable3 = null;
                dynamicParameter.Add("sortDir", sortDirection, nullable5, nullable2, nullable1, nullable10, nullable3);
            }
            SqlConnection connection = base.Connection;
            IDbTransaction transaction = base.Transaction;
            CommandType? nullable11 = new CommandType?(CommandType.StoredProcedure);
            nullable1 = null;
            Task<IEnumerable<DdocFolder>> task = connection.QueryAsync<DdocFolder>("ddoc.sp_GetFolders", dynamicParameter, transaction, nullable1, nullable11);
            return task;
        }

        public async Task<IEnumerable<DdocFolder>> PrintSearchFolders(DdocSearchParameters searchParameters, List<DdocField> fields)
        {
            string str;
            string str1;
            Query query = base.db.Query(String.Concat("G_CAMPOS_", searchParameters.CollectionId, " as F")).Join("G_FOLDERS as D", "D.GID", "F.GID", "=", "inner join").BuildWhere(searchParameters.ClauseGroups);
            str = (String.IsNullOrEmpty(searchParameters.SortResultsBy) ? "D.GID" : searchParameters.SortResultsBy);
            str1 = (searchParameters.SortDirection == 1 ? "ASC" : "DESC");
            Query query1 = query.OrderByRaw(String.Concat(str, " ", str1), Array.Empty<object>());
            List<string> strs = new List<string>()
            {
                "F.GID"
            };
            foreach (DdocField field in fields)
            {
                strs.Add(String.Format("F.C{0}", field.Id));
            }
            query1.Select(strs.ToArray());
            IEnumerable<object> async = await QueryExtensionsAsync.GetAsync(query1, null);
            IEnumerable<IReadOnlyDictionary<string, object>> readOnlyDictionaries = async.Cast<IReadOnlyDictionary<string, object>>();
            async = null;
            IEnumerable<DdocFolder> ddocFolder =
                from d in readOnlyDictionaries
                select new DdocFolder()
                {
                    Id = d["GID"].ToString(),
                    CollectionId = searchParameters.CollectionId,
                    Data = (
                        from f in fields
                        select new DdocField()
                        {
                            Id = f.Id,
                            Name = f.Name,
                            Value = d.GetValue(f.Id, f.Type, f.OutMask),
                            TypeString = f.TypeString,
                            Format = f.Format,
                            Nullable = f.Nullable,
                            Hidden = f.Hidden,
                            AllowedValuesString = f.AllowedValuesString,
                            InMask = f.InMask,
                            OutMask = f.OutMask,
                            IncludeInGlobalSearch = f.IncludeInGlobalSearch,
                            Computed = f.Computed
                        }).ToList<DdocField>()
                };
            IEnumerable<DdocFolder> ddocFolders = ddocFolder;
            query1 = null;
            strs = null;
            readOnlyDictionaries = null;
            ddocFolder = null;
            return ddocFolders;
        }

        //[return: TupleElementNames(new string[] { "totalRecords", "results" })]
        public async Task<ValueTuple<int, IEnumerable<DdocFolder>>> SearchFolders(DdocSearchParameters searchParameters, List<DdocField> fields, int page, int pageSize)
        {
            String str;
            String str1;
            Query query = base.db.Query(String.Concat("G_CAMPOS_", searchParameters.CollectionId, " as F")).Join("G_FOLDERS as D", "D.GID", "F.GID", "=", "inner join");
            string securityGroupsCsv = searchParameters.SecurityGroupsCsv;
            Char[] chrArray = new Char[] { ',' };
            Query query1 = query.WhereIn<string>("SEGURIDAD", securityGroupsCsv.Split(chrArray)).BuildWhere(searchParameters.ClauseGroups);
            int num = await QueryAggregateExtensionsAsync.CountAsync<int>(query1, Array.Empty<string>());
            int num1 = num;
            List<AbstractClause> clauses = query1.Clauses;
            clauses.RemoveAll((AbstractClause c) => c.Component == "aggregate");
            List<string> strs = new List<string>()
            {
                "F.GID"
            };
            foreach (DdocField field in fields)
            {
                strs.Add(String.Format("F.C{0}", field.Id));
            }
            Query query2 = query1.Select(strs.ToArray());
            Query query3 = base.db.Query().From(query2, "D");
            Query query4 = query3.Select(new String[] { "D.*" });
            String[] strArrays = new String[] { "ROW_NUMBER() OVER(ORDER BY ", null, null, null, null };
            str = (String.IsNullOrEmpty(searchParameters.SortResultsBy) ? "D.GID" : searchParameters.SortResultsBy);
            strArrays[1] = str;
            strArrays[2] = " ";
            str1 = (searchParameters.SortDirection == 1 ? "ASC" : "DESC");
            strArrays[3] = str1;
            strArrays[4] = ") AS [rownum]";
            Query query5 = query4.SelectRaw(String.Concat(strArrays), Array.Empty<object>());
            Query query6 = base.db.Query("cte_pagedFolders").With("cte_pagedFolders", query5).WhereBetween<int>("rownum", (page - 1) * pageSize + 1, page * pageSize);
            IEnumerable<object> async = await QueryExtensionsAsync.GetAsync(query6, null);
            IEnumerable<IReadOnlyDictionary<string, object>> readOnlyDictionaries = async.Cast<IReadOnlyDictionary<string, object>>();
            async = null;
            List<DdocFolder> list = (
                from d in readOnlyDictionaries
                select new DdocFolder()
                {
                    Id = d["GID"].ToString(),
                    Data = (
                        from f in fields
                        select new DdocField()
                        {
                            Id = f.Id,
                            Name = f.Name,
                            Value = d.GetValue(f.Id, f.Type, f.OutMask),
                            TypeString = f.TypeString,
                            Format = f.Format,
                            Nullable = f.Nullable,
                            Hidden = f.Hidden,
                            AllowedValuesString = f.AllowedValuesString,
                            InMask = f.InMask,
                            OutMask = f.OutMask,
                            IncludeInGlobalSearch = f.IncludeInGlobalSearch,
                            Computed = f.Computed
                        }).ToList<DdocField>()
                }).ToList<DdocFolder>();
            ValueTuple<int, IEnumerable<DdocFolder>> valueTuple = new ValueTuple<int, IEnumerable<DdocFolder>>(num1, list);
            query1 = null;
            strs = null;
            query2 = null;
            query5 = null;
            query6 = null;
            readOnlyDictionaries = null;
            list = null;
            return valueTuple;
        }

        public async Task UpdateFolderData(string folderId, string collectionId, IEnumerable<DdocField> folderData)
        {
            Query query = base.db.Query(String.Concat("G_CAMPOS_", collectionId)).Where("GID", folderId);
            Dictionary<string, object> strs = new Dictionary<string, object>();
            IEnumerable<DdocField> ddocFields = folderData;
            foreach (DdocField ddocField in
                from f in ddocFields
                where !f.Computed
                select f)
            {
                strs.Add(String.Format("C{0}", ddocField.Id), (dynamic)ddocField.GetValue());
            }
            int? nullable = null;
            await QueryExtensionsAsync.UpdateAsync(query, (IReadOnlyDictionary<string, object>)strs, nullable);
            Query query1 = base.db.Query("G_FOLDERS").Where("GID", folderId);
            Dictionary<string, object> strs1 = new Dictionary<string, object>()
            {
                { "FECHA_ACTUALIZACION", DateTime.Now }
            };
            nullable = null;
            await QueryExtensionsAsync.UpdateAsync(query1, (IReadOnlyDictionary<string, object>)strs1, nullable);
            query = null;
            strs = null;
        }

        public Task UpdateFolderSecurity(string folderId, int securityGroupId)
        {
            return QueryExtensionsAsync.UpdateAsync(base.db.Query("G_FOLDERS").Where("GID", folderId), (IReadOnlyDictionary<string, object>)(new Dictionary<string, object>()
            {
                { "SEGURIDAD", securityGroupId },
                { "FECHA_ACTUALIZACION", DateTime.Now }
            }), null);
        }

        public Task UpdateFoldertDataKey(string folderId, string dataKey)
        {
            return QueryExtensionsAsync.UpdateAsync(base.db.Query("G_FOLDERS").Where("GID", folderId), (IReadOnlyDictionary<string, object>)(new Dictionary<string, object>()
            {
                { "DATAKEY", dataKey }
            }), null);
        }
    }
}