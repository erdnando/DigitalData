// Decompiled with JetBrains decompiler
// Type: DigitalData.DDoc.Modules.DataAccess.DocumentDAL
// Assembly: DigitalData.DDoc.Modules.DataAccess.SqlServer, Version=4.210.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EB5ECC0D-3654-4980-9D29-200BC39CB926
// Assembly location: D:\proyectos\ddocx\DigitalData.DDoc.UI.Webx\DigitalData.DDoc.UI.Web\bin\DigitalData.DDoc.Modules.DataAccess.SqlServer.dll

using Dapper;
using DigitalData.Common.DataAccess;
using DigitalData.Common.DataAccess.QueryExecution;
using DigitalData.Common.DataAccess.SqlServer;
using DigitalData.DDoc.Common;
using DigitalData.DDoc.Common.Api;
using DigitalData.DDoc.Common.Api.DataAccess;
using DigitalData.DDoc.Common.Entities;
using Microsoft.CSharp.RuntimeBinder;
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
  public class DocumentDAL : SqlServerDAL, IDocumentDAL, ICommonDAL
  {
        public DocumentDAL(SqlConnection connection, SqlTransaction transaction = null) : base(connection, transaction)
        {
        }

        public Task<string> GetDocumentId(string pageId)
        {
            return QueryExtensionsAsync.SingleOrDefaultAsync<string>(base.db.Query("G_PAGINAS").Where("GID", pageId).Select(new String[] { "GID_DOCUMENTO" }), null);
        }
        public Task<string> GetCollectionId(string documentId)
        {
            return QueryExtensionsAsync.SingleOrDefaultAsync<string>(base.db.Query("G_DOCUMENTOS").Where("GID", documentId).Select(new String[] { "GID_COLECCION" }), null);
        }
        public Task<DdocDocument> GetDocument(string documentId)
    {
      DynamicParameters dynamicParameters1 = new DynamicParameters();
      DynamicParameters dynamicParameters2 = dynamicParameters1;
      string str = documentId;
      DbType? nullable1 = new DbType?(DbType.String);
      int? nullable2 = new int?(10);
      ParameterDirection? nullable3 = new ParameterDirection?();
      int? nullable4 = nullable2;
      byte? nullable5 = new byte?();
      byte? nullable6 = new byte?();
      dynamicParameters2.Add("@documentId", (object) str, nullable1, nullable3, nullable4, nullable5, nullable6);
      SqlConnection connection = ((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Connection;
      DynamicParameters dynamicParameters3 = dynamicParameters1;
      IDbTransaction transaction = ((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Transaction;
      CommandType? nullable7 = new CommandType?(CommandType.StoredProcedure);
      int? nullable8 = new int?();
      CommandType? nullable9 = nullable7;
      return (Task<DdocDocument>) SqlMapper.QuerySingleOrDefaultAsync<DdocDocument>((IDbConnection) connection, "[ddoc].[sp_GetDocument]", (object) dynamicParameters3, transaction, nullable8, nullable9);
    }

        public async Task<int> CountDocuments(string collectionId, IEnumerable<int> securityGroups)
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
            await connection.ExecuteAsync("[ddoc].[sp_CountDocuments]", dynamicParameter2, transaction, nullable1, nullable8);
            int num = dynamicParameter.Get<int>("total");
            dynamicParameter = null;
            return num;
        }

        public Task<IEnumerable<DdocDocument>> GetDocuments(string collectionId, IEnumerable<int> securityGroups, int page, int pageSize, string sortBy, int sortDirection)
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
            Task<IEnumerable<DdocDocument>> task = connection.QueryAsync<DdocDocument>("ddoc.sp_GetDocuments", dynamicParameter, transaction, nullable1, nullable11);
            return task;
        }

        // [return: TupleElementNames(new string[] { "totalRecords", "results" })]
        public async Task<ValueTuple<int, IEnumerable<DdocDocument>>> SearchDocuments(DdocSearchParameters searchParameters, List<DdocField> fields, int page, int pageSize)
        {
            String str;
            String str1;
            Query query = base.db.Query(String.Concat("G_CAMPOS_", searchParameters.CollectionId, " as F")).Join("G_DOCUMENTOS as D", "D.GID", "F.GID", "=", "inner join");
            string securityGroupsCsv = searchParameters.SecurityGroupsCsv;
            Char[] chrArray = new Char[] { ',' };
            Query query1 = query.WhereIn<string>("SEGURIDAD", securityGroupsCsv.Split(chrArray)).BuildWhere(searchParameters.ClauseGroups);
            int num = await QueryAggregateExtensionsAsync.CountAsync<int>(query1, Array.Empty<string>());
            int num1 = num;
            List<AbstractClause> clauses = query1.Clauses;// .get_Clauses();
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
            Query query6 = base.db.Query("cte_pagedDocuments").With("cte_pagedDocuments", query5).WhereBetween<int>("rownum", (page - 1) * pageSize + 1, page * pageSize);
            IEnumerable<object> async = await QueryExtensionsAsync.GetAsync(query6, null);
            IEnumerable<IReadOnlyDictionary<string, object>> readOnlyDictionaries = async.Cast<IReadOnlyDictionary<string, object>>();
            async = null;
            List<DdocDocument> list = (
                from d in readOnlyDictionaries
                select new DdocDocument()
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
                }).ToList<DdocDocument>();
            ValueTuple<int, IEnumerable<DdocDocument>> valueTuple = new ValueTuple<int, IEnumerable<DdocDocument>>(num1, list);
            query1 = null;
            strs = null;
            query2 = null;
            query5 = null;
            query6 = null;
            readOnlyDictionaries = null;
            list = null;
            return valueTuple;
        }

        public async Task<IEnumerable<DdocDocument>> PrintSearchDocuments(DdocSearchParameters searchParameters, List<DdocField> fields)
        {
            string str;
            string str1;
            Query query = base.db.Query(String.Concat("G_CAMPOS_", searchParameters.CollectionId, " as F")).Join("G_DOCUMENTOS as D", "D.GID", "F.GID", "=", "inner join").BuildWhere(searchParameters.ClauseGroups);
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
            IEnumerable<DdocDocument> ddocDocument =
                from d in readOnlyDictionaries
                select new DdocDocument()
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
            IEnumerable<DdocDocument> ddocDocuments = ddocDocument;
            query1 = null;
            strs = null;
            readOnlyDictionaries = null;
            ddocDocument = null;
            return ddocDocuments;
        }

        public async Task<IEnumerable<DdocDocument>> DocumentTextSearch(string collectionId, List<DdocField> fields, string searchText)
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
            IEnumerable<DdocDocument> ddocDocument =
                from d in readOnlyDictionaries
                select new DdocDocument()
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
                };
            IEnumerable<DdocDocument> ddocDocuments = ddocDocument;
            query = null;
            strs = null;
            readOnlyDictionaries = null;
            ddocDocument = null;
            return ddocDocuments;
        }

        public async Task<IEnumerable<DdocField>> GetDocumentData(string documentId, string collectionId, List<DdocField> fields)
        {
            Query query = base.db.Query(String.Concat("G_CAMPOS_", collectionId)).Where("GID", documentId);
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

        public async Task<string> CreatePage(
      string documentId,
      string extension,
      int sequence,
      int? pathId = null,
      bool replaced = false)
    {
      DynamicParameters parameters = new DynamicParameters();
      DynamicParameters dynamicParameters1 = parameters;
      string str1 = documentId;
      DbType? nullable1 = new DbType?(DbType.String);
      int? nullable2 = new int?(10);
      ParameterDirection? nullable3 = new ParameterDirection?();
      int? nullable4 = nullable2;
      byte? nullable5 = new byte?();
      byte? nullable6 = new byte?();
      dynamicParameters1.Add(nameof (documentId), (object) str1, nullable1, nullable3, nullable4, nullable5, nullable6);
      if (pathId.HasValue)
        parameters.Add(nameof (pathId), (object) pathId, new DbType?(DbType.Int32), new ParameterDirection?(), new int?(), new byte?(), new byte?());
      DynamicParameters dynamicParameters2 = parameters;
      string str2 = extension;
      DbType? nullable7 = new DbType?(DbType.String);
      int? nullable8 = new int?(4);
      ParameterDirection? nullable9 = new ParameterDirection?();
      int? nullable10 = nullable8;
      byte? nullable11 = new byte?();
      byte? nullable12 = new byte?();
      dynamicParameters2.Add(nameof (extension), (object) str2, nullable7, nullable9, nullable10, nullable11, nullable12);
      if ((uint) sequence > 0U)
        parameters.Add(nameof (sequence), (object) sequence, new DbType?(DbType.Int32), new ParameterDirection?(), new int?(), new byte?(), new byte?());
      if (replaced)
        parameters.Add(nameof (replaced), (object) true, new DbType?(DbType.Boolean), new ParameterDirection?(), new int?(), new byte?(), new byte?());
      DynamicParameters dynamicParameters3 = parameters;
      DbType? nullable13 = new DbType?(DbType.String);
      int? nullable14 = new int?(10);
      ParameterDirection? nullable15 = new ParameterDirection?(ParameterDirection.Output);
      int? nullable16 = nullable14;
      byte? nullable17 = new byte?();
      byte? nullable18 = new byte?();
      dynamicParameters3.Add("pageId", (object) null, nullable13, nullable15, nullable16, nullable17, nullable18);
      SqlConnection connection = ((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Connection;
      DynamicParameters dynamicParameters4 = parameters;
      IDbTransaction transaction = ((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Transaction;
      CommandType? nullable19 = new CommandType?(CommandType.StoredProcedure);
      int? nullable20 = new int?();
      CommandType? nullable21 = nullable19;
      string str3 = await (Task<string>) SqlMapper.QuerySingleOrDefaultAsync<string>((IDbConnection) connection, "[ddoc].[sp_CreatePage]", (object) dynamicParameters4, transaction, nullable20, nullable21);
      string str4 = (string) parameters.Get<string>("pageId");
      parameters = (DynamicParameters) null;
      return str4;
    }

    public Task<IEnumerable<DdocPage>> GetPages(string documentId)
    {
      DynamicParameters dynamicParameters1 = new DynamicParameters();
      DynamicParameters dynamicParameters2 = dynamicParameters1;
      string str = documentId;
      DbType? nullable1 = new DbType?(DbType.String);
      int? nullable2 = new int?(10);
      ParameterDirection? nullable3 = new ParameterDirection?();
      int? nullable4 = nullable2;
      byte? nullable5 = new byte?();
      byte? nullable6 = new byte?();
      dynamicParameters2.Add("@documentId", (object) str, nullable1, nullable3, nullable4, nullable5, nullable6);
      SqlConnection connection = ((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Connection;
      DynamicParameters dynamicParameters3 = dynamicParameters1;
      IDbTransaction transaction = ((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Transaction;
      CommandType? nullable7 = new CommandType?(CommandType.StoredProcedure);
      int? nullable8 = new int?();
      CommandType? nullable9 = nullable7;
      return (Task<IEnumerable<DdocPage>>) SqlMapper.QueryAsync<DdocPage>((IDbConnection) connection, "[ddoc].[sp_GetPages]", (object) dynamicParameters3, transaction, nullable8, nullable9);
    }

    public async Task<string> RegisterNewDocument(string collectionId, int securityGroupId)
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
      parameters.Add("@creationDate", (object) DateTime.Now, new DbType?(DbType.DateTime), new ParameterDirection?(), new int?(), new byte?(), new byte?());
      parameters.Add("@security", (object) securityGroupId, new DbType?(DbType.Int32), new ParameterDirection?(), new int?(), new byte?(), new byte?());
      DynamicParameters dynamicParameters2 = parameters;
      DbType? nullable7 = new DbType?(DbType.String);
      int? nullable8 = new int?(10);
      ParameterDirection? nullable9 = new ParameterDirection?(ParameterDirection.Output);
      int? nullable10 = nullable8;
      byte? nullable11 = new byte?();
      byte? nullable12 = new byte?();
      dynamicParameters2.Add("@documentId", (object) null, nullable7, nullable9, nullable10, nullable11, nullable12);
      SqlConnection connection = ((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Connection;
      DynamicParameters dynamicParameters3 = parameters;
      IDbTransaction transaction = ((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Transaction;
      CommandType? nullable13 = new CommandType?(CommandType.StoredProcedure);
      int? nullable14 = new int?();
      CommandType? nullable15 = nullable13;
      int num = await SqlMapper.ExecuteAsync((IDbConnection) connection, "[ddoc].[sp_RegisterNewDocument]", (object) dynamicParameters3, transaction, nullable14, nullable15);
      string str2 = (string) parameters.Get<string>("@documentId");
      parameters = (DynamicParameters) null;
      return str2;
    }

        public Task CommitDocument(string documentId, string name, string dataKey)
        {
            return QueryExtensionsAsync.UpdateAsync(base.db.Query("G_DOCUMENTOS").Where("GID", documentId), (IReadOnlyDictionary<string, object>)(new Dictionary<string, object>()
            {
                { "NOMBRE", name },
                { "DATAKEY", dataKey },
                { "COMMITED", true }
            }), null);
        }



        public Task UpdateCreationDate(string documentId, DateTime creationDate)
        {
            return QueryExtensionsAsync.UpdateAsync(base.db.Query("G_DOCUMENTOS").Where("GID", documentId), (IReadOnlyDictionary<string, object>)(new Dictionary<string, object>()
            {
                { "FECHA_CREACION", creationDate }
            }), null);
        }

        public async Task<string> CreateDocument(DdocDocument document)
    {
      DynamicParameters parameters = new DynamicParameters();
      DynamicParameters dynamicParameters1 = parameters;
      string collectionId = document.CollectionId;
      DbType? nullable1 = new DbType?(DbType.String);
      int? nullable2 = new int?(10);
      ParameterDirection? nullable3 = new ParameterDirection?();
      int? nullable4 = nullable2;
      byte? nullable5 = new byte?();
      byte? nullable6 = new byte?();
      dynamicParameters1.Add("@collectionId", (object) collectionId, nullable1, nullable3, nullable4, nullable5, nullable6);
      DynamicParameters dynamicParameters2 = parameters;
      string name = document.Name;
      DbType? nullable7 = new DbType?(DbType.String);
      int? nullable8 = new int?(100);
      ParameterDirection? nullable9 = new ParameterDirection?();
      int? nullable10 = nullable8;
      byte? nullable11 = new byte?();
      byte? nullable12 = new byte?();
      dynamicParameters2.Add("@name", (object) name, nullable7, nullable9, nullable10, nullable11, nullable12);
      DynamicParameters dynamicParameters3 = parameters;
      string dataKey = document.DataKey;
      DbType? nullable13 = new DbType?(DbType.String);
      int? nullable14 = new int?(5000);
      ParameterDirection? nullable15 = new ParameterDirection?();
      int? nullable16 = nullable14;
      byte? nullable17 = new byte?();
      byte? nullable18 = new byte?();
      dynamicParameters3.Add("@dataKey", (object) dataKey, nullable13, nullable15, nullable16, nullable17, nullable18);
      if (document.CreationDate.HasValue)
        parameters.Add("@creationDate", (object) document.CreationDate, new DbType?(DbType.DateTime), new ParameterDirection?(), new int?(), new byte?(), new byte?());
      parameters.Add("@security", (object) document.SecurityGroupId, new DbType?(DbType.Int32), new ParameterDirection?(), new int?(), new byte?(), new byte?());
      DynamicParameters dynamicParameters4 = parameters;
      DbType? nullable19 = new DbType?(DbType.String);
      int? nullable20 = new int?(10);
      ParameterDirection? nullable21 = new ParameterDirection?(ParameterDirection.Output);
      int? nullable22 = nullable20;
      byte? nullable23 = new byte?();
      byte? nullable24 = new byte?();
      dynamicParameters4.Add("@documentId", (object) null, nullable19, nullable21, nullable22, nullable23, nullable24);
      SqlConnection connection = ((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Connection;
      DynamicParameters dynamicParameters5 = parameters;
      IDbTransaction transaction = ((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Transaction;
      CommandType? nullable25 = new CommandType?(CommandType.StoredProcedure);
      int? nullable26 = new int?();
      CommandType? nullable27 = nullable25;
      int num = await SqlMapper.ExecuteAsync((IDbConnection) connection, "[ddoc].[sp_CreateDocument]", (object) dynamicParameters5, transaction, nullable26, nullable27);
      string str = (string) parameters.Get<string>("@documentId");
      parameters = (DynamicParameters) null;
      return str;
    }

        public Task CreateDocumentData(string documentId, string collectionId, IEnumerable<DdocField> documentData)
        {
            Query query = base.db.Query(String.Concat("G_CAMPOS_", collectionId));
            Dictionary<string, object> strs = new Dictionary<string, object>()
            {
                { "GID", documentId }
            };
            foreach (DdocField ddocField in
                from f in documentData
                where !f.Computed
                select f)
            {
                strs.Add(String.Format("C{0}", ddocField.Id), (dynamic)ddocField.GetValue());
            }
            return QueryExtensionsAsync.InsertAsync(query, (IReadOnlyDictionary<string, object>)strs, null);
        }

        public Task UpdateDocumentCollection(string documentId, string collectionId)
        {
            return QueryExtensionsAsync.UpdateAsync(base.db.Query("G_DOCUMENTOS").Where("GID", documentId), (IReadOnlyDictionary<string, object>)(new Dictionary<string, object>()
            {
                { "GID_COLECCION", collectionId },
                { "FECHA_ACTUALIZACION", DateTime.Now }
            }), null);
        }

        public Task UpdateDocumentDataKey(string documentId, string dataKey)
        {
            return QueryExtensionsAsync.UpdateAsync(base.db.Query("G_DOCUMENTOS").Where("GID", documentId), (IReadOnlyDictionary<string, object>)(new Dictionary<string, object>()
            {
                { "DATAKEY", dataKey }
            }), null);
        }

        public async Task UpdateDocumentData(string documentId, string collectionId, IEnumerable<DdocField> documentData, bool updateDate = true)
        {
            Query query = base.db.Query(String.Concat("G_CAMPOS_", collectionId)).Where("GID", documentId);
            Dictionary<string, object> strs = new Dictionary<string, object>();
            IEnumerable<DdocField> ddocFields = documentData;
            foreach (DdocField ddocField in
                from f in ddocFields
                where !f.Computed
                select f)
            {
                strs.Add(String.Format("C{0}", ddocField.Id), (dynamic)ddocField.GetValue());
            }
            int? nullable = null;
            await QueryExtensionsAsync.UpdateAsync(query, (IReadOnlyDictionary<string, object>)strs, nullable);
            if (updateDate)
            {
                Query query1 = base.db.Query("G_DOCUMENTOS").Where("GID", documentId);
                Dictionary<string, object> strs1 = new Dictionary<string, object>()
                {
                    { "FECHA_ACTUALIZACION", DateTime.Now }
                };
                nullable = null;
                await QueryExtensionsAsync.UpdateAsync(query1, (IReadOnlyDictionary<string, object>)strs1, nullable);
            }
            query = null;
            strs = null;
        }

        public Task UpdateDocumentSecurity(string documentId, int securityGroupId)
        {
            return QueryExtensionsAsync.UpdateAsync(base.db.Query("G_DOCUMENTOS").Where("GID", documentId), (IReadOnlyDictionary<string, object>)(new Dictionary<string, object>()
            {
                { "SEGURIDAD", securityGroupId },
                { "FECHA_ACTUALIZACION", DateTime.Now }
            }), null);
        }

        public Task UpdatePageSequence(string pageId, int sequence)
    {
      DynamicParameters dynamicParameters1 = new DynamicParameters();
      DynamicParameters dynamicParameters2 = dynamicParameters1;
      string str = pageId;
      DbType? nullable1 = new DbType?(DbType.String);
      int? nullable2 = new int?(10);
      ParameterDirection? nullable3 = new ParameterDirection?();
      int? nullable4 = nullable2;
      byte? nullable5 = new byte?();
      byte? nullable6 = new byte?();
      dynamicParameters2.Add("@pageId", (object) str, nullable1, nullable3, nullable4, nullable5, nullable6);
      dynamicParameters1.Add("@sequence", (object) sequence, new DbType?(DbType.Int32), new ParameterDirection?(), new int?(), new byte?(), new byte?());
      SqlConnection connection = ((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Connection;
      DynamicParameters dynamicParameters3 = dynamicParameters1;
      IDbTransaction transaction = ((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Transaction;
      CommandType? nullable7 = new CommandType?(CommandType.StoredProcedure);
      int? nullable8 = new int?();
      CommandType? nullable9 = nullable7;
      return (Task) SqlMapper.ExecuteAsync((IDbConnection) connection, "[ddoc].[sp_UpdatePageOrder]", (object) dynamicParameters3, transaction, nullable8, nullable9);
    }

        public Task UpdatePageWarehousePathId(string pageId, int pathId)
        {
            return QueryExtensionsAsync.UpdateAsync(base.db.Query("G_PAGINAS").Where("GID", pageId), (IReadOnlyDictionary<string, object>)(new Dictionary<string, object>()
            {
                { "PATH_ID", pathId }
            }), null);
        }

        public Task<bool> DocumentExists(string documentId)
        {
            int? nullable = null;
            return QueryExtensionsAsync.ExistsAsync(base.db.Query("G_DOCUMENTOS").Where("GID", documentId), nullable);
        }
        public Task<bool> PageExists(string pageId)
        {
            int? nullable = null;
            return QueryExtensionsAsync.ExistsAsync(base.db.Query("G_PAGINAS").Where("GID", pageId), nullable);
        }
        public Task<bool> DocumentExists(DdocDocument document, List<int> includedFields = null, List<int> excludedFields = null)
        {
            Query query = base.db.Query("G_DOCUMENTOS as D").Join(String.Concat("G_CAMPOS_", document.CollectionId, " as F"), "F.GID", "D.GID", "=", "inner join");
            if (includedFields != null)
            {
                document.Data = document.Data.Except<DdocField>(
                    from f in document.Data
                    where !includedFields.Contains(f.Id)
                    select f).ToList<DdocField>();
            }
            else if (excludedFields != null)
            {
                document.Data = document.Data.Except<DdocField>(
                    from f in document.Data
                    where excludedFields.Contains(f.Id)
                    select f).ToList<DdocField>();
            }
            foreach (DdocField datum in document.Data)
            {
                query.Where(String.Format("C{0}", datum.Id), (dynamic)datum.GetValue());
            }
            return QueryExtensionsAsync.ExistsAsync(query, null);
        }

        public Task<bool> PathExists(int pathId)
        {
            int? nullable = null;
            return QueryExtensionsAsync.ExistsAsync(base.db.Query("G_PATH").Where("PATH_ID", pathId), nullable);
        }
        public Task<int> DeleteDocument(string documentId, string user)
    {
      DynamicParameters dynamicParameters1 = new DynamicParameters();
      DynamicParameters dynamicParameters2 = dynamicParameters1;
      string str1 = documentId;
      DbType? nullable1 = new DbType?(DbType.String);
      int? nullable2 = new int?(10);
      ParameterDirection? nullable3 = new ParameterDirection?();
      int? nullable4 = nullable2;
      byte? nullable5 = new byte?();
      byte? nullable6 = new byte?();
      dynamicParameters2.Add("@documentId", (object) str1, nullable1, nullable3, nullable4, nullable5, nullable6);
      if (!string.IsNullOrEmpty(user))
      {
        DynamicParameters dynamicParameters3 = dynamicParameters1;
        string str2 = user;
        DbType? nullable7 = new DbType?(DbType.String);
        nullable2 = new int?(50);
        ParameterDirection? nullable8 = new ParameterDirection?();
        int? nullable9 = nullable2;
        byte? nullable10 = new byte?();
        byte? nullable11 = new byte?();
        dynamicParameters3.Add("@user", (object) str2, nullable7, nullable8, nullable9, nullable10, nullable11);
      }
      SqlConnection connection = ((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Connection;
      DynamicParameters dynamicParameters4 = dynamicParameters1;
      IDbTransaction transaction = ((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Transaction;
      CommandType? nullable12 = new CommandType?(CommandType.StoredProcedure);
      nullable2 = new int?();
      int? nullable13 = nullable2;
      CommandType? nullable14 = nullable12;
      return SqlMapper.ExecuteAsync((IDbConnection) connection, "[ddoc].[sp_DeleteDocument]", (object) dynamicParameters4, transaction, nullable13, nullable14);
    }

        public Task DeleteDocumentData(string documentId, string collectionId)
        {
            int? nullable = null;
            return QueryExtensionsAsync.DeleteAsync(base.db.Query(String.Concat("G_CAMPOS_", collectionId)).Where("GID", documentId), nullable);
        }
        public Task<int> DeletePage(string pageId, string user)
    {
      DynamicParameters dynamicParameters1 = new DynamicParameters();
      DynamicParameters dynamicParameters2 = dynamicParameters1;
      string str1 = pageId;
      DbType? nullable1 = new DbType?(DbType.String);
      int? nullable2 = new int?(10);
      ParameterDirection? nullable3 = new ParameterDirection?();
      int? nullable4 = nullable2;
      byte? nullable5 = new byte?();
      byte? nullable6 = new byte?();
      dynamicParameters2.Add("@pageId", (object) str1, nullable1, nullable3, nullable4, nullable5, nullable6);
      if (!string.IsNullOrEmpty(user))
      {
        DynamicParameters dynamicParameters3 = dynamicParameters1;
        string str2 = user;
        DbType? nullable7 = new DbType?(DbType.String);
        nullable2 = new int?(50);
        ParameterDirection? nullable8 = new ParameterDirection?();
        int? nullable9 = nullable2;
        byte? nullable10 = new byte?();
        byte? nullable11 = new byte?();
        dynamicParameters3.Add("@user", (object) str2, nullable7, nullable8, nullable9, nullable10, nullable11);
      }
      SqlConnection connection = ((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Connection;
      DynamicParameters dynamicParameters4 = dynamicParameters1;
      IDbTransaction transaction = ((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Transaction;
      CommandType? nullable12 = new CommandType?(CommandType.StoredProcedure);
      nullable2 = new int?();
      int? nullable13 = nullable2;
      CommandType? nullable14 = nullable12;
      return SqlMapper.ExecuteAsync((IDbConnection) connection, "[ddoc].[sp_DeletePage]", (object) dynamicParameters4, transaction, nullable13, nullable14);
    }

    public Task<int> GetPageSequence(string pageId)
    {
      DynamicParameters dynamicParameters1 = new DynamicParameters();
      DynamicParameters dynamicParameters2 = dynamicParameters1;
      string str = pageId;
      DbType? nullable1 = new DbType?(DbType.String);
      int? nullable2 = new int?(10);
      ParameterDirection? nullable3 = new ParameterDirection?();
      int? nullable4 = nullable2;
      byte? nullable5 = new byte?();
      byte? nullable6 = new byte?();
      dynamicParameters2.Add("@pageId", (object) str, nullable1, nullable3, nullable4, nullable5, nullable6);
      SqlConnection connection = ((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Connection;
      DynamicParameters dynamicParameters3 = dynamicParameters1;
      IDbTransaction transaction = ((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Transaction;
      CommandType? nullable7 = new CommandType?(CommandType.StoredProcedure);
      int? nullable8 = new int?();
      CommandType? nullable9 = nullable7;
      return (Task<int>) SqlMapper.QuerySingleOrDefaultAsync<int>((IDbConnection) connection, "[ddoc].[sp_GetPageOrder]", (object) dynamicParameters3, transaction, nullable8, nullable9);
    }

    public async Task<int> GetWarehousePathId(string documentId)
    {
      DynamicParameters parameters = new DynamicParameters();
      DynamicParameters dynamicParameters1 = parameters;
      string str = documentId;
      DbType? nullable1 = new DbType?(DbType.String);
      int? nullable2 = new int?(10);
      ParameterDirection? nullable3 = new ParameterDirection?();
      int? nullable4 = nullable2;
      byte? nullable5 = new byte?();
      byte? nullable6 = new byte?();
      dynamicParameters1.Add("@documentId", (object) str, nullable1, nullable3, nullable4, nullable5, nullable6);
      parameters.Add("@pathId", (object) null, new DbType?(DbType.Int32), new ParameterDirection?(ParameterDirection.Output), new int?(), new byte?(), new byte?());
      SqlConnection connection = ((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Connection;
      DynamicParameters dynamicParameters2 = parameters;
      IDbTransaction transaction = ((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Transaction;
      CommandType? nullable7 = new CommandType?(CommandType.StoredProcedure);
      int? nullable8 = new int?();
      CommandType? nullable9 = nullable7;
      int num1 = await (Task<int>) SqlMapper.QuerySingleOrDefaultAsync<int>((IDbConnection) connection, "[ddoc].[sp_GetWarehousePathId]", (object) dynamicParameters2, transaction, nullable8, nullable9);
      int num2 = (int) parameters.Get<int>("@pathId");
      parameters = (DynamicParameters) null;
      return num2;
    }

    public Task UpdatePageImageCount(string pageId, int imageCount)
    {
      DynamicParameters dynamicParameters1 = new DynamicParameters();
      DynamicParameters dynamicParameters2 = dynamicParameters1;
      string str = pageId;
      DbType? nullable1 = new DbType?(DbType.String);
      int? nullable2 = new int?(10);
      ParameterDirection? nullable3 = new ParameterDirection?();
      int? nullable4 = nullable2;
      byte? nullable5 = new byte?();
      byte? nullable6 = new byte?();
      dynamicParameters2.Add(nameof (pageId), (object) str, nullable1, nullable3, nullable4, nullable5, nullable6);
      dynamicParameters1.Add(nameof (imageCount), (object) imageCount, new DbType?(DbType.Int32), new ParameterDirection?(), new int?(), new byte?(), new byte?());
      SqlConnection connection = ((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Connection;
      DynamicParameters dynamicParameters3 = dynamicParameters1;
      IDbTransaction transaction = ((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Transaction;
      CommandType? nullable7 = new CommandType?(CommandType.StoredProcedure);
      int? nullable8 = new int?();
      CommandType? nullable9 = nullable7;
      return (Task) SqlMapper.QueryAsync((IDbConnection) connection, "[ddoc].[sp_UpdatePageImageCount]", (object) dynamicParameters3, transaction, nullable8, nullable9);
    }

        public Task<IEnumerable<DdocDocument>> GetOrphanDocuments(int timeoutMinutes)
        {
            Query query = base.db.Query("G_DOCUMENTOS").WhereFalse("COMMITED");
            DateTime now = DateTime.Now;
            return QueryExtensionsAsync.GetAsync<DdocDocument>(query.Where("FECHA_CREACION", "<", now.AddMinutes((double)(-timeoutMinutes))).Select(new String[] { "GID AS Id", "GID AS Id", "GID_COLECCION AS CollectionId", "NOMBRE AS Name", "FECHA_CREACION AS CreationDate", "SEGURIDAD AS SecurityGroupId", "PAGINAS AS PageCount", "FECHA_BORRADO AS DeletionDate", "FECHA_ACTUALIZACION AS ModificationDate", "COMMITED AS Commited" }), null);
        }

        public Task<int> GetCommitTimeout()
        {
            return QueryExtensionsAsync.SingleAsync<int>(base.db.Query("G_PARAMETROS").Where("PARAMETRO_NOMBRE", "CommitTimeoutMinutes").Select(new String[] { "PARAMETRO_VALOR" }), null);
        }
        public Task<IEnumerable<DdocDocument>> GetExpiredDocuments()
        {
            return QueryExtensionsAsync.GetAsync<DdocDocument>(base.db.Query("G_DOCUMENTOS").WhereDate("FECHA_BORRADO", "<=", DateTime.Now).Select(new String[] { "GID AS Id", "GID AS Id", "GID_COLECCION AS CollectionId", "NOMBRE AS Name", "FECHA_CREACION AS CreationDate", "SEGURIDAD AS SecurityGroupId", "PAGINAS AS PageCount", "FECHA_BORRADO AS DeletionDate", "FECHA_ACTUALIZACION AS ModificationDate", "COMMITED AS Commited" }), null);
        }

        [SpecialName]
    IDbTransaction get_Transaction() => ((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Transaction;

    [SpecialName]
    void set_Transaction(IDbTransaction value) => ((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Transaction = value;
  }
}
