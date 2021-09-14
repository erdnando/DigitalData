// Decompiled with JetBrains decompiler
// Type: DigitalData.Common.DataAccess.QueryExecution.QueryExtensionsAsync
// Assembly: DigitalData.Common.DataAccess, Version=6.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 143C0880-64BB-461C-BA76-CECC458DBFC6
// Assembly location: D:\proyectos\ddocx\DigitalData.DDoc.UI.Webx\DigitalData.DDoc.UI.Web\lib\DigitalData.Common.DataAccess.dll

using Dapper;
using Microsoft.CSharp.RuntimeBinder;
using SqlKata;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace DigitalData.Common.DataAccess.QueryExecution
{
  public static class QueryExtensionsAsync
  {
    public static async Task<IEnumerable<T>> GetAsync<T>(
      this Query query,
      int? commandTimeout = null)
    {
      XQuery xQuery = QueryHelper.CastToXQuery(query, nameof (GetAsync));
      SqlResult compiled = xQuery.Compiler.Compile(query);
      xQuery.Logger(compiled);
      IEnumerable<T> objs = await (Task<IEnumerable<T>>) SqlMapper.QueryAsync<T>(xQuery.Connection, compiled.Sql, (object) compiled.NamedBindings, xQuery.Transaction, commandTimeout, new CommandType?());
      xQuery = (XQuery) null;
      compiled = (SqlResult) null;
      return objs;
    }

    public static async Task<IEnumerable<object>> GetAsync(
      this Query query,
      int? commandTimeout = null)
    {
      XQuery xQuery = QueryHelper.CastToXQuery(query, nameof (GetAsync));
      SqlResult compiled = xQuery.Compiler.Compile(query);
      xQuery.Logger(compiled);
      IEnumerable<object> objects = await (Task<IEnumerable<object>>) SqlMapper.QueryAsync<object>(xQuery.Connection, compiled.Sql, (object) compiled.NamedBindings, xQuery.Transaction, commandTimeout, new CommandType?());
      xQuery = (XQuery) null;
      compiled = (SqlResult) null;
      return objects;
    }

    [AsyncIteratorStateMachine(typeof (QueryExtensionsAsync.\u003CGetAsyncStream\u003Ed__2<>))]
    public static IAsyncEnumerable<T> GetAsyncStream<T>(
      this Query query,
      int? commandTimeout = null)
    {
      // ISSUE: object of a compiler-generated type is created
      return (IAsyncEnumerable<T>) new QueryExtensionsAsync.\u003CGetAsyncStream\u003Ed__2<T>(-2)
      {
        \u003C\u003E3__query = query,
        \u003C\u003E3__commandTimeout = commandTimeout
      };
    }

    public static async Task<DbStream<T>> GetAsDbStreamAsync<T>(
      this Query query,
      int? commandTimeout = null)
    {
      XQuery xQuery = QueryHelper.CastToXQuery(query, nameof (GetAsDbStreamAsync));
      SqlResult compiled = xQuery.Compiler.Compile(query);
      xQuery.Logger(compiled);
      IDataReader reader = await SqlMapper.ExecuteReaderAsync(xQuery.Connection, compiled.Sql, (object) compiled.NamedBindings, xQuery.Transaction, commandTimeout, new CommandType?());
      DbStream<T> dbStream = new DbStream<T>(reader);
      xQuery = (XQuery) null;
      compiled = (SqlResult) null;
      reader = (IDataReader) null;
      return dbStream;
    }

    public static async Task<DbStream<object>> GetAsDbStreamAsync(
      this Query query,
      int? commandTimeout = null)
    {
      DbStream<object> asDbStreamAsync = await query.GetAsDbStreamAsync<object>(commandTimeout);
      return asDbStreamAsync;
    }

    public static async Task<T> SingleOrDefaultAsync<T>(this Query query, int? commandTimeout = null)
    {
      XQuery xQuery = QueryHelper.CastToXQuery(query, nameof (SingleOrDefaultAsync));
      SqlResult compiled = xQuery.Compiler.Compile(query.Limit(1));
      xQuery.Logger(compiled);
      T obj = await (Task<T>) SqlMapper.QuerySingleOrDefaultAsync<T>(xQuery.Connection, compiled.Sql, (object) compiled.NamedBindings, xQuery.Transaction, commandTimeout, new CommandType?());
      xQuery = (XQuery) null;
      compiled = (SqlResult) null;
      return obj;
    }

    public static async Task<object> SingleOrDefaultAsync(this Query query, int? commandTimeout = null)
    {
      object obj = await query.SingleOrDefaultAsync<object>(commandTimeout);
      return obj;
    }

    public static async Task<T> SingleAsync<T>(this Query query, int? commandTimeout = null)
    {
      XQuery xQuery = QueryHelper.CastToXQuery(query, nameof (SingleAsync));
      SqlResult compiled = xQuery.Compiler.Compile(query.Limit(1));
      xQuery.Logger(compiled);
      T obj = await (Task<T>) SqlMapper.QuerySingleAsync<T>(xQuery.Connection, compiled.Sql, (object) compiled.NamedBindings, xQuery.Transaction, commandTimeout, new CommandType?());
      xQuery = (XQuery) null;
      compiled = (SqlResult) null;
      return obj;
    }

    public static async Task<object> SingleAsync(this Query query, int? commandTimeout = null)
    {
      object obj = await query.SingleAsync<object>(commandTimeout);
      return obj;
    }

    public static async Task<T> FirstOrDefaultAsync<T>(this Query query, int? commandTimeout = null)
    {
      XQuery xQuery = QueryHelper.CastToXQuery(query, nameof (FirstOrDefaultAsync));
      SqlResult compiled = xQuery.Compiler.Compile(query.Limit(1));
      xQuery.Logger(compiled);
      T obj = await (Task<T>) SqlMapper.QueryFirstOrDefaultAsync<T>(xQuery.Connection, compiled.Sql, (object) compiled.NamedBindings, xQuery.Transaction, commandTimeout, new CommandType?());
      xQuery = (XQuery) null;
      compiled = (SqlResult) null;
      return obj;
    }

    public static async Task<object> FirstOrDefaultAsync(this Query query, int? commandTimeout = null)
    {
      object obj = await query.FirstOrDefaultAsync<object>(commandTimeout);
      return obj;
    }

    public static async Task<T> FirstAsync<T>(this Query query, int? commandTimeout = null)
    {
      XQuery xQuery = QueryHelper.CastToXQuery(query, nameof (FirstAsync));
      SqlResult compiled = xQuery.Compiler.Compile(query.Limit(1));
      xQuery.Logger(compiled);
      T obj = await (Task<T>) SqlMapper.QueryFirstAsync<T>(xQuery.Connection, compiled.Sql, (object) compiled.NamedBindings, xQuery.Transaction, commandTimeout, new CommandType?());
      xQuery = (XQuery) null;
      compiled = (SqlResult) null;
      return obj;
    }

    public static async Task<object> FirstAsync(this Query query, int? commandTimeout = null)
    {
      object obj = await query.FirstAsync<object>(commandTimeout);
      return obj;
    }

    public static async Task<bool> ExistsAsync(this Query query, int? commandTimeout = null)
    {
      XQuery xQuery = QueryHelper.CastToXQuery(query, nameof (ExistsAsync));
      SqlResult compiled = xQuery.Compiler.Compile(query.Limit(1));
      xQuery.Logger(compiled);
      object record = await SqlMapper.QueryFirstOrDefaultAsync(xQuery.Connection, compiled.Sql, (object) compiled.NamedBindings, xQuery.Transaction, commandTimeout, new CommandType?());
      // ISSUE: reference to a compiler-generated field
      if (QueryExtensionsAsync.\u003C\u003Eo__13.\u003C\u003Ep__1 == null)
      {
        // ISSUE: reference to a compiler-generated field
        QueryExtensionsAsync.\u003C\u003Eo__13.\u003C\u003Ep__1 = CallSite<Func<CallSite, object, bool>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (bool), typeof (QueryExtensionsAsync)));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, bool> target = QueryExtensionsAsync.\u003C\u003Eo__13.\u003C\u003Ep__1.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, bool>> p1 = QueryExtensionsAsync.\u003C\u003Eo__13.\u003C\u003Ep__1;
      // ISSUE: reference to a compiler-generated field
      if (QueryExtensionsAsync.\u003C\u003Eo__13.\u003C\u003Ep__0 == null)
      {
        // ISSUE: reference to a compiler-generated field
        QueryExtensionsAsync.\u003C\u003Eo__13.\u003C\u003Ep__0 = CallSite<Func<CallSite, object, object, object>>.Create(Binder.BinaryOperation(CSharpBinderFlags.None, ExpressionType.NotEqual, typeof (QueryExtensionsAsync), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.Constant, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj = QueryExtensionsAsync.\u003C\u003Eo__13.\u003C\u003Ep__0.Target((CallSite) QueryExtensionsAsync.\u003C\u003Eo__13.\u003C\u003Ep__0, record, (object) null);
      bool flag = target((CallSite) p1, obj);
      xQuery = (XQuery) null;
      compiled = (SqlResult) null;
      record = (object) null;
      return flag;
    }

    public static async Task<PaginationResult<T>> PaginateAsync<T>(
      this Query query,
      int page,
      int perPage = 25,
      int? commandTimeout = null)
    {
      if (page < 1)
        throw new ArgumentException("Page param should be greater than or equal to 1", nameof (page));
      if (perPage < 1)
        throw new ArgumentException("PerPage param should be greater than or equal to 1", nameof (perPage));
      long count = await query.Clone().CountAsync<long>();
      IEnumerable<T> list;
      if (count > 0L)
        list = await query.Clone().ForPage(page, perPage).GetAsync<T>(commandTimeout);
      else
        list = Enumerable.Empty<T>();
      PaginationResult<T> paginationResult = new PaginationResult<T>()
      {
        Query = query.Clone(),
        Page = page,
        PerPage = perPage,
        Count = count,
        List = list
      };
      list = (IEnumerable<T>) null;
      return paginationResult;
    }

    public static async Task<PaginationResult<object>> PaginateAsync(
      this Query query,
      int page,
      int perPage = 25,
      int? commandTimeout = null)
    {
      PaginationResult<object> paginationResult = await query.PaginateAsync<object>(page, perPage, commandTimeout);
      return paginationResult;
    }

    public static async Task ChunkAsync<T>(
      this Query query,
      int chunkSize,
      Func<IEnumerable<T>, int, bool> func,
      int? commandTimeout = null)
    {
      PaginationResult<T> result = await query.PaginateAsync<T>(1, chunkSize, commandTimeout);
      if (!func(result.List, 1))
      {
        result = (PaginationResult<T>) null;
      }
      else
      {
        while (result.HasNext)
        {
          result = result.Next();
          if (!func(result.List, result.Page))
          {
            result = (PaginationResult<T>) null;
            return;
          }
        }
        result = (PaginationResult<T>) null;
      }
    }

    public static async Task ChunkAsync(
      this Query query,
      int chunkSize,
      Func<IEnumerable<object>, int, bool> func,
      int? commandTimeout = null)
    {
      await query.ChunkAsync<object>(chunkSize, func, commandTimeout);
    }

    public static async Task ChunkAsync<T>(
      this Query query,
      int chunkSize,
      Action<IEnumerable<T>, int> action,
      int? commandTimeout = null)
    {
      PaginationResult<T> result = await query.PaginateAsync<T>(1, chunkSize, commandTimeout);
      action(result.List, 1);
      while (result.HasNext)
      {
        result = result.Next();
        action(result.List, result.Page);
      }
      result = (PaginationResult<T>) null;
    }

    public static async Task ChunkAsync(
      this Query query,
      int chunkSize,
      Action<IEnumerable<object>, int> action,
      int? commandTimeout = null)
    {
      await query.ChunkAsync<object>(chunkSize, action, commandTimeout);
    }

    [AsyncIteratorStateMachine(typeof (QueryExtensionsAsync.\u003CChunkAsync\u003Ed__20<>))]
    public static IAsyncEnumerable<T> ChunkAsync<T>(
      this Query query,
      int chunkSize,
      int? commandTimeout = null)
    {
      // ISSUE: object of a compiler-generated type is created
      return (IAsyncEnumerable<T>) new QueryExtensionsAsync.\u003CChunkAsync\u003Ed__20<T>(-2)
      {
        \u003C\u003E3__query = query,
        \u003C\u003E3__chunkSize = chunkSize,
        \u003C\u003E3__commandTimeout = commandTimeout
      };
    }

    [AsyncIteratorStateMachine(typeof (QueryExtensionsAsync.\u003CChunkAsync\u003Ed__21))]
    public static IAsyncEnumerable<object> ChunkAsync(
      this Query query,
      int chunkSize,
      int? commandTimeout = null)
    {
      // ISSUE: object of a compiler-generated type is created
      return (IAsyncEnumerable<object>) new QueryExtensionsAsync.\u003CChunkAsync\u003Ed__21(-2)
      {
        \u003C\u003E3__query = query,
        \u003C\u003E3__chunkSize = chunkSize,
        \u003C\u003E3__commandTimeout = commandTimeout
      };
    }

    public static async Task<int> InsertAsync(
      this Query query,
      IReadOnlyDictionary<string, object> values,
      int? commandTimeout = null)
    {
      XQuery xQuery = QueryHelper.CastToXQuery(query, nameof (InsertAsync));
      SqlResult compiled = xQuery.Compiler.Compile(query.AsInsert((IEnumerable<KeyValuePair<string, object>>) values, false));
      xQuery.Logger(compiled);
      int num = await SqlMapper.ExecuteAsync(xQuery.Connection, compiled.Sql, (object) compiled.NamedBindings, xQuery.Transaction, commandTimeout, new CommandType?());
      xQuery = (XQuery) null;
      compiled = (SqlResult) null;
      return num;
    }

    public static async Task<int> InsertAsync(
      this Query query,
      object data,
      int? commandTimeout = null)
    {
      XQuery xQuery = QueryHelper.CastToXQuery(query, nameof (InsertAsync));
      SqlResult compiled = xQuery.Compiler.Compile(query.AsInsert(data));
      xQuery.Logger(compiled);
      int num = await SqlMapper.ExecuteAsync(xQuery.Connection, compiled.Sql, (object) compiled.NamedBindings, xQuery.Transaction, commandTimeout, new CommandType?());
      xQuery = (XQuery) null;
      compiled = (SqlResult) null;
      return num;
    }

    public static async Task<T> InsertGetIdAsync<T>(
      this Query query,
      object data,
      int? commandTimeout = null)
    {
      XQuery xQuery = QueryHelper.CastToXQuery(query, nameof (InsertGetIdAsync));
      SqlResult compiled = xQuery.Compiler.Compile(query.AsInsert(data, true));
      xQuery.Logger(compiled);
      InsertGetIdRow<T> row = await (Task<InsertGetIdRow<T>>) SqlMapper.QueryFirstAsync<InsertGetIdRow<T>>(xQuery.Connection, compiled.Sql, (object) compiled.NamedBindings, xQuery.Transaction, commandTimeout, new CommandType?());
      T id = row.Id;
      xQuery = (XQuery) null;
      compiled = (SqlResult) null;
      row = (InsertGetIdRow<T>) null;
      return id;
    }

    public static async Task<T> InsertGetIdAsync<T>(
      this Query query,
      IReadOnlyDictionary<string, object> values,
      int? commandTimeout = null)
    {
      XQuery xQuery = QueryHelper.CastToXQuery(query, nameof (InsertGetIdAsync));
      SqlResult compiled = xQuery.Compiler.Compile(query.AsInsert((IEnumerable<KeyValuePair<string, object>>) values, true));
      xQuery.Logger(compiled);
      InsertGetIdRow<T> row = await (Task<InsertGetIdRow<T>>) SqlMapper.QueryFirstAsync<InsertGetIdRow<T>>(xQuery.Connection, compiled.Sql, (object) compiled.NamedBindings, xQuery.Transaction, commandTimeout, new CommandType?());
      T id = row.Id;
      xQuery = (XQuery) null;
      compiled = (SqlResult) null;
      row = (InsertGetIdRow<T>) null;
      return id;
    }

    public static async Task<int> InsertAsync(
      this Query query,
      IEnumerable<string> columns,
      Query fromQuery,
      int? commandTimeout = null)
    {
      XQuery xQuery = QueryHelper.CastToXQuery(query, nameof (InsertAsync));
      SqlResult compiled = xQuery.Compiler.Compile(query.AsInsert(columns, fromQuery));
      xQuery.Logger(compiled);
      int num = await SqlMapper.ExecuteAsync(xQuery.Connection, compiled.Sql, (object) compiled.NamedBindings, xQuery.Transaction, commandTimeout, new CommandType?());
      xQuery = (XQuery) null;
      compiled = (SqlResult) null;
      return num;
    }

    public static async Task<int> UpdateAsync(
      this Query query,
      IReadOnlyDictionary<string, object> values,
      int? commandTimeout = null)
    {
      XQuery xQuery = QueryHelper.CastToXQuery(query, nameof (UpdateAsync));
      SqlResult compiled = xQuery.Compiler.Compile(query.AsUpdate((IEnumerable<KeyValuePair<string, object>>) values));
      xQuery.Logger(compiled);
      int num = await SqlMapper.ExecuteAsync(xQuery.Connection, compiled.Sql, (object) compiled.NamedBindings, xQuery.Transaction, commandTimeout, new CommandType?());
      xQuery = (XQuery) null;
      compiled = (SqlResult) null;
      return num;
    }

    public static async Task<IEnumerable<object>> UpdateWithOutputAsync(
      this Query query,
      IReadOnlyDictionary<string, object> values,
      IEnumerable<string> outputFields = null,
      string outputTable = null,
      int? commandTimeout = null)
    {
      XQuery xQuery = QueryHelper.CastToXQuery(query, "UpdateAsync");
      SqlResult compiled = xQuery.Compiler.Compile(QueryExtensions.AsUpdate(query, values, outputFields, outputTable));
      xQuery.Logger(compiled);
      IEnumerable<object> objects = await SqlMapper.QueryAsync(xQuery.Connection, compiled.Sql, (object) compiled.NamedBindings, xQuery.Transaction, commandTimeout, new CommandType?());
      xQuery = (XQuery) null;
      compiled = (SqlResult) null;
      return objects;
    }

    public static async Task<IEnumerable<T>> UpdateWithOutputAsync<T>(
      this Query query,
      IReadOnlyDictionary<string, object> values,
      IEnumerable<string> outputFields = null,
      string outputTable = null,
      int? commandTimeout = null)
    {
      XQuery xQuery = QueryHelper.CastToXQuery(query, "UpdateAsync");
      SqlResult compiled = xQuery.Compiler.Compile(QueryExtensions.AsUpdate(query, values, outputFields, outputTable));
      xQuery.Logger(compiled);
      IEnumerable<T> objs = await (Task<IEnumerable<T>>) SqlMapper.QueryAsync<T>(xQuery.Connection, compiled.Sql, (object) compiled.NamedBindings, xQuery.Transaction, commandTimeout, new CommandType?());
      xQuery = (XQuery) null;
      compiled = (SqlResult) null;
      return objs;
    }

    public static async Task<int> UpdateAsync(
      this Query query,
      object data,
      int? commandTimeout = null)
    {
      XQuery xQuery = QueryHelper.CastToXQuery(query, nameof (UpdateAsync));
      SqlResult compiled = xQuery.Compiler.Compile(query.AsUpdate(data));
      xQuery.Logger(compiled);
      int num = await SqlMapper.ExecuteAsync(xQuery.Connection, compiled.Sql, (object) compiled.NamedBindings, xQuery.Transaction, commandTimeout, new CommandType?());
      xQuery = (XQuery) null;
      compiled = (SqlResult) null;
      return num;
    }

    public static async Task<IEnumerable<object>> UpdateWithOutputAsync(
      this Query query,
      object data,
      IEnumerable<string> outputFields = null,
      string outputTable = null,
      int? commandTimeout = null)
    {
      XQuery xQuery = QueryHelper.CastToXQuery(query, "UpdateAsync");
      SqlResult compiled = xQuery.Compiler.Compile(query.AsUpdate(data, outputFields, outputTable));
      xQuery.Logger(compiled);
      IEnumerable<object> objects = await SqlMapper.QueryAsync(xQuery.Connection, compiled.Sql, (object) compiled.NamedBindings, xQuery.Transaction, commandTimeout, new CommandType?());
      xQuery = (XQuery) null;
      compiled = (SqlResult) null;
      return objects;
    }

    public static async Task<IEnumerable<T>> UpdateWithOutputAsync<T>(
      this Query query,
      object data,
      IEnumerable<string> outputFields = null,
      string outputTable = null,
      int? commandTimeout = null)
    {
      XQuery xQuery = QueryHelper.CastToXQuery(query, "UpdateAsync");
      SqlResult compiled = xQuery.Compiler.Compile(query.AsUpdate(data, outputFields, outputTable));
      xQuery.Logger(compiled);
      IEnumerable<T> objs = await (Task<IEnumerable<T>>) SqlMapper.QueryAsync<T>(xQuery.Connection, compiled.Sql, (object) compiled.NamedBindings, xQuery.Transaction, commandTimeout, new CommandType?());
      xQuery = (XQuery) null;
      compiled = (SqlResult) null;
      return objs;
    }

    public static async Task<DbStream<T>> UpdateWithStreamedOutputAsync<T>(
      this Query query,
      IReadOnlyDictionary<string, object> values,
      IEnumerable<string> outputFields = null,
      string outputTable = null,
      int? commandTimeout = null)
    {
      XQuery xQuery = QueryHelper.CastToXQuery(query, "Update");
      SqlResult compiled = xQuery.Compiler.Compile(QueryExtensions.AsUpdate(query, values, outputFields, outputTable));
      xQuery.Logger(compiled);
      IDataReader reader = await SqlMapper.ExecuteReaderAsync(xQuery.Connection, compiled.Sql, (object) compiled.NamedBindings, xQuery.Transaction, commandTimeout, new CommandType?());
      DbStream<T> dbStream = new DbStream<T>(reader);
      xQuery = (XQuery) null;
      compiled = (SqlResult) null;
      reader = (IDataReader) null;
      return dbStream;
    }

    public static async Task<DbStream<object>> UpdateWithStreamedOutputAsync(
      this Query query,
      IReadOnlyDictionary<string, object> values,
      IEnumerable<string> outputFields = null,
      string outputTable = null,
      int? commandTimeout = null)
    {
      XQuery xQuery = QueryHelper.CastToXQuery(query, "Update");
      SqlResult compiled = xQuery.Compiler.Compile(QueryExtensions.AsUpdate(query, values, outputFields, outputTable));
      xQuery.Logger(compiled);
      IDataReader reader = await SqlMapper.ExecuteReaderAsync(xQuery.Connection, compiled.Sql, (object) compiled.NamedBindings, xQuery.Transaction, commandTimeout, new CommandType?());
      DbStream<object> dbStream = new DbStream<object>(reader);
      xQuery = (XQuery) null;
      compiled = (SqlResult) null;
      reader = (IDataReader) null;
      return dbStream;
    }

    public static async Task<DbStream<T>> UpdateWithStreamedOutpuAsync<T>(
      this Query query,
      object data,
      IEnumerable<string> outputFields,
      string outputTable = null,
      int? commandTimeout = null)
    {
      XQuery xQuery = QueryHelper.CastToXQuery(query, "Update");
      SqlResult compiled = xQuery.Compiler.Compile(query.AsUpdate(data, outputFields, outputTable));
      xQuery.Logger(compiled);
      IDataReader reader = await SqlMapper.ExecuteReaderAsync(xQuery.Connection, compiled.Sql, (object) compiled.NamedBindings, xQuery.Transaction, commandTimeout, new CommandType?());
      DbStream<T> dbStream = new DbStream<T>(reader);
      xQuery = (XQuery) null;
      compiled = (SqlResult) null;
      reader = (IDataReader) null;
      return dbStream;
    }

    public static async Task<DbStream<object>> UpdateWithStreamedOutputAsync(
      this Query query,
      object data,
      IEnumerable<string> outputFields,
      string outputTable = null,
      int? commandTimeout = null)
    {
      XQuery xQuery = QueryHelper.CastToXQuery(query, "Update");
      SqlResult compiled = xQuery.Compiler.Compile(query.AsUpdate(data, outputFields, outputTable));
      xQuery.Logger(compiled);
      IDataReader reader = await SqlMapper.ExecuteReaderAsync(xQuery.Connection, compiled.Sql, (object) compiled.NamedBindings, xQuery.Transaction, commandTimeout, new CommandType?());
      DbStream<object> dbStream = new DbStream<object>(reader);
      xQuery = (XQuery) null;
      compiled = (SqlResult) null;
      reader = (IDataReader) null;
      return dbStream;
    }

    public static async Task<int> DeleteAsync(this Query query, int? commandTimeout = null)
    {
      XQuery xQuery = QueryHelper.CastToXQuery(query, nameof (DeleteAsync));
      SqlResult compiled = xQuery.Compiler.Compile(query.AsDelete());
      xQuery.Logger(compiled);
      int num = await SqlMapper.ExecuteAsync(xQuery.Connection, compiled.Sql, (object) compiled.NamedBindings, xQuery.Transaction, commandTimeout, new CommandType?());
      xQuery = (XQuery) null;
      compiled = (SqlResult) null;
      return num;
    }

    public static async Task<IEnumerable<object>> DeleteWithOutputAsync(
      this Query query,
      IEnumerable<string> outputFields = null,
      string outputTable = null,
      int? commandTimeout = null)
    {
      XQuery xQuery = QueryHelper.CastToXQuery(query, "DeleteAsync");
      SqlResult compiled = xQuery.Compiler.Compile(query.AsDelete(outputFields, outputTable));
      xQuery.Logger(compiled);
      IEnumerable<object> objects = await SqlMapper.QueryAsync(xQuery.Connection, compiled.Sql, (object) compiled.NamedBindings, xQuery.Transaction, commandTimeout, new CommandType?());
      xQuery = (XQuery) null;
      compiled = (SqlResult) null;
      return objects;
    }

    public static async Task<IEnumerable<T>> DeleteWithOutputAsync<T>(
      this Query query,
      IEnumerable<string> outputFields = null,
      string outputTable = null,
      int? commandTimeout = null)
    {
      XQuery xQuery = QueryHelper.CastToXQuery(query, "DeleteAsync");
      SqlResult compiled = xQuery.Compiler.Compile(query.AsDelete(outputFields, outputTable));
      xQuery.Logger(compiled);
      IEnumerable<T> objs = await (Task<IEnumerable<T>>) SqlMapper.QueryAsync<T>(xQuery.Connection, compiled.Sql, (object) compiled.NamedBindings, xQuery.Transaction, commandTimeout, new CommandType?());
      xQuery = (XQuery) null;
      compiled = (SqlResult) null;
      return objs;
    }

    public static async Task<DbStream<T>> DeleteWithStreamedOutputAsync<T>(
      this Query query,
      IEnumerable<string> outputFields = null,
      string outputTable = null,
      int? commandTimeout = null)
    {
      XQuery xQuery = QueryHelper.CastToXQuery(query, "Delete");
      SqlResult compiled = xQuery.Compiler.Compile(query.AsDelete(outputFields, outputTable));
      xQuery.Logger(compiled);
      IDataReader reader = await SqlMapper.ExecuteReaderAsync(xQuery.Connection, compiled.Sql, (object) compiled.NamedBindings, xQuery.Transaction, commandTimeout, new CommandType?());
      DbStream<T> dbStream = new DbStream<T>(reader);
      xQuery = (XQuery) null;
      compiled = (SqlResult) null;
      reader = (IDataReader) null;
      return dbStream;
    }

    public static async Task<DbStream<object>> DeleteWithStreamedOutputAsync(
      this Query query,
      IEnumerable<string> outputFields = null,
      string outputTable = null,
      int? commandTimeout = null)
    {
      XQuery xQuery = QueryHelper.CastToXQuery(query, "Delete");
      SqlResult compiled = xQuery.Compiler.Compile(query.AsDelete(outputFields, outputTable));
      xQuery.Logger(compiled);
      IDataReader reader = await SqlMapper.ExecuteReaderAsync(xQuery.Connection, compiled.Sql, (object) compiled.NamedBindings, xQuery.Transaction, commandTimeout, new CommandType?());
      DbStream<object> dbStream = new DbStream<object>(reader);
      xQuery = (XQuery) null;
      compiled = (SqlResult) null;
      reader = (IDataReader) null;
      return dbStream;
    }
  }
}
