// Decompiled with JetBrains decompiler
// Type: DigitalData.Common.DataAccess.QueryExecution.QueryFactoryExtensions
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
  public static class QueryFactoryExtensions
  {
    private static SqlResult compile(this QueryFactory db, Query query)
    {
      SqlResult sqlResult = db.Compiler.Compile(query);
      db.Logger(sqlResult);
      return sqlResult;
    }

    public static IEnumerable<T> Get<T>(
      this QueryFactory db,
      Query query,
      bool buffered = true,
      int? commandTimeout = null)
    {
      SqlResult sqlResult = db.compile(query);
      return SqlMapper.Query<T>(db.Connection, sqlResult.Sql, (object) sqlResult.NamedBindings, db.Transaction, buffered, commandTimeout, new CommandType?());
    }

    public static IEnumerable<IDictionary<string, object>> GetDictionary(
      this QueryFactory db,
      Query query,
      bool buffered = true,
      int? commandTimeout = null)
    {
      SqlResult sqlResult = db.compile(query);
      return SqlMapper.Query(db.Connection, sqlResult.Sql, (object) sqlResult.NamedBindings, db.Transaction, buffered, commandTimeout, new CommandType?()) as IEnumerable<IDictionary<string, object>>;
    }

    public static IEnumerable<object> Get(
      this QueryFactory db,
      Query query,
      bool buffered = true,
      int? commandTimeout = null)
    {
      return db.Get<object>(query, buffered, commandTimeout);
    }

    public static DbStream<T> GetAsDbStream<T>(
      this QueryFactory db,
      Query query,
      int? commandTimeout = null)
    {
      SqlResult sqlResult = db.compile(query);
      return new DbStream<T>(SqlMapper.ExecuteReader(db.Connection, sqlResult.Sql, (object) sqlResult.NamedBindings, db.Transaction, commandTimeout, new CommandType?()));
    }

    public static DbStream<object> GetAsDbStream(
      this QueryFactory db,
      Query query,
      int? commandTimeout = null)
    {
      return db.GetAsDbStream<object>(query, commandTimeout);
    }

    public static T Single<T>(this QueryFactory db, Query query, int? commandTimeout = null)
    {
      SqlResult sqlResult = db.compile(query);
      return SqlMapper.QuerySingle<T>(db.Connection, sqlResult.Sql, (object) sqlResult.NamedBindings, db.Transaction, commandTimeout, new CommandType?());
    }

    public static object Single(this QueryFactory db, Query query, int? commandTimeout = null) => db.Single<object>(query, commandTimeout);

    public static T SingleOrDefault<T>(this QueryFactory db, Query query, int? commandTimeout = null)
    {
      SqlResult sqlResult = db.compile(query);
      return SqlMapper.QuerySingleOrDefault<T>(db.Connection, sqlResult.Sql, (object) sqlResult.NamedBindings, db.Transaction, commandTimeout, new CommandType?());
    }

    public static object SingleOrDefault(this QueryFactory db, Query query, int? commandTimeout = null) => db.SingleOrDefault<object>(query, commandTimeout);

    public static T First<T>(this QueryFactory db, Query query, int? commandTimeout = null)
    {
      SqlResult sqlResult = db.compile(query.Limit(1));
      return SqlMapper.QueryFirst<T>(db.Connection, sqlResult.Sql, (object) sqlResult.NamedBindings, db.Transaction, commandTimeout, new CommandType?());
    }

    public static object First(this QueryFactory db, Query query, int? commandTimeout = null) => db.First<object>(query, commandTimeout);

    public static T FirstOrDefault<T>(this QueryFactory db, Query query, int? commandTimeout = null)
    {
      SqlResult sqlResult = db.compile(query.Limit(1));
      return SqlMapper.QueryFirstOrDefault<T>(db.Connection, sqlResult.Sql, (object) sqlResult.NamedBindings, db.Transaction, commandTimeout, new CommandType?());
    }

    public static object FirstOrDefault(this QueryFactory db, Query query, int? commandTimeout = null) => db.FirstOrDefault<object>(query, commandTimeout);

        public static bool Exists(this QueryFactory db, Query query, int? commandTimeout = null)
        {
            SqlResult sqlResult = db.compile(query);
            CommandType? nullable = null;
            bool flag = (Boolean)((dynamic)db.Connection.QueryFirstOrDefault(sqlResult.Sql, sqlResult.NamedBindings, db.Transaction, commandTimeout, nullable) != (dynamic)null);
            return flag;
        }

        public static int Execute(
      this QueryFactory db,
      Query query,
      IDbTransaction transaction = null,
      CommandType? commandType = null)
    {
      SqlResult sqlResult = db.compile(query);
      return SqlMapper.Execute(db.Connection, sqlResult.Sql, (object) sqlResult.NamedBindings, transaction, new int?(db.QueryTimeout), commandType);
    }

    public static T ExecuteScalar<T>(
      this QueryFactory db,
      Query query,
      IDbTransaction transaction = null,
      CommandType? commandType = null)
    {
      SqlResult sqlResult = db.compile(query.Limit(1));
      return SqlMapper.ExecuteScalar<T>(db.Connection, sqlResult.Sql, (object) sqlResult.NamedBindings, transaction, new int?(db.QueryTimeout), commandType);
    }

    public static SqlMapper.GridReader GetMultiple<T>(
      this QueryFactory db,
      Query[] queries,
      IDbTransaction transaction = null,
      CommandType? commandType = null)
    {
      SqlResult sqlResult = db.Compiler.Compile((IEnumerable<Query>) queries);
      return SqlMapper.QueryMultiple(db.Connection, sqlResult.Sql, (object) sqlResult.NamedBindings, transaction, new int?(db.QueryTimeout), commandType);
    }

    public static IEnumerable<IEnumerable<T>> Get<T>(
      this QueryFactory db,
      Query[] queries,
      IDbTransaction transaction = null,
      CommandType? commandType = null)
    {
      SqlMapper.GridReader multi = db.GetMultiple<T>(queries, transaction, commandType);
      using (multi)
      {
        for (int i = 0; i < ((IEnumerable<Query>) queries).Count<Query>(); ++i)
          yield return (IEnumerable<T>) multi.Read<T>(true);
      }
    }

    public static T Aggregate<T>(
      this QueryFactory db,
      Query query,
      string aggregateOperation,
      params string[] columns)
    {
      return db.ExecuteScalar<T>(query.AsAggregate(aggregateOperation, columns), db.Transaction);
    }

    public static T Count<T>(this QueryFactory db, Query query, params string[] columns) => db.ExecuteScalar<T>(query.AsCount(columns), db.Transaction);

    public static T Average<T>(this QueryFactory db, Query query, string column) => db.Aggregate<T>(query, "avg", column);

    public static T Sum<T>(this QueryFactory db, Query query, string column) => db.Aggregate<T>(query, "sum", column);

    public static T Min<T>(this QueryFactory db, Query query, string column) => db.Aggregate<T>(query, "min", column);

    public static T Max<T>(this QueryFactory db, Query query, string column) => db.Aggregate<T>(query, "max", column);

    public static PaginationResult<T> Paginate<T>(
      this QueryFactory db,
      Query query,
      int page,
      int perPage = 25,
      bool buffered = false,
      int? commandTimeout = null)
    {
      if (page < 1)
        throw new ArgumentException("Page param should be greater than or equal to 1", nameof (page));
      if (perPage < 1)
        throw new ArgumentException("PerPage param should be greater than or equal to 1", nameof (perPage));
      long num = query.Clone().Count<long>();
      IEnumerable<T> objs = num <= 0L ? Enumerable.Empty<T>() : query.Clone().ForPage(page, perPage).Get<T>(buffered, commandTimeout);
      return new PaginationResult<T>()
      {
        Query = query,
        Page = page,
        PerPage = perPage,
        Count = num,
        List = objs
      };
    }

    public static void Chunk<T>(
      this QueryFactory db,
      Query query,
      int chunkSize,
      Func<IEnumerable<T>, int, bool> func,
      bool buffered = false,
      int? commandTimeout = null)
    {
      PaginationResult<T> paginationResult = db.Paginate<T>(query, 1, chunkSize, buffered, commandTimeout);
      if (!func(paginationResult.List, 1))
        return;
      while (paginationResult.HasNext)
      {
        paginationResult = paginationResult.Next();
        if (!func(paginationResult.List, paginationResult.Page))
          break;
      }
    }

    public static void Chunk<T>(
      this QueryFactory db,
      Query query,
      int chunkSize,
      Action<IEnumerable<T>, int> action,
      bool buffered = false,
      int? commandTimeout = null)
    {
      PaginationResult<T> paginationResult = db.Paginate<T>(query, 1, chunkSize, buffered, commandTimeout);
      action(paginationResult.List, 1);
      while (paginationResult.HasNext)
      {
        paginationResult = paginationResult.Next();
        action(paginationResult.List, paginationResult.Page);
      }
    }

    public static IEnumerable<T> Chunk<T>(
      this QueryFactory db,
      Query query,
      int chunkSize,
      bool buffered = false,
      int? commandTimeout = null)
    {
      PaginationResult<T> results = db.Paginate<T>(query, 1, chunkSize, buffered, commandTimeout);
      foreach (T obj in results.List)
      {
        T result = obj;
        yield return result;
        result = default (T);
      }
      while (results.HasNext)
      {
        results = results.Next();
        foreach (T obj in results.List)
        {
          T result = obj;
          yield return result;
          result = default (T);
        }
      }
    }

    public static IEnumerable<T> Select<T>(
      this QueryFactory db,
      string sql,
      object param = null,
      bool buffered = true,
      int? commandTimeout = null)
    {
      return SqlMapper.Query<T>(db.Connection, sql, param, db.Transaction, buffered, commandTimeout, new CommandType?());
    }

    public static IEnumerable<object> Select(
      this QueryFactory db,
      string sql,
      object param = null,
      bool buffered = true,
      int? commandTimeout = null)
    {
      return db.Select<object>(sql, param, buffered, commandTimeout);
    }

    public static DbStream<T> StreamSelect<T>(
      this QueryFactory db,
      string sql,
      object param = null,
      int? commandTimeout = null)
    {
      return new DbStream<T>(SqlMapper.ExecuteReader(db.Connection, sql, param, db.Transaction, commandTimeout, new CommandType?()));
    }

    public static DbStream<object> StreamSelect(
      this QueryFactory db,
      string sql,
      object param = null,
      int? commandTimeout = null)
    {
      return db.StreamSelect<object>(sql, param, commandTimeout);
    }

    public static int Statement(
      this QueryFactory db,
      string sql,
      object param = null,
      int? commandTimeout = null)
    {
      return SqlMapper.Execute(db.Connection, sql, param, db.Transaction, commandTimeout, new CommandType?());
    }

    public static int ExecuteSp(
      this QueryFactory db,
      string sql,
      DynamicParameters param = null,
      int? commandTimeout = null)
    {
      return SqlMapper.Execute(db.Connection, sql, (object) param, db.Transaction, commandTimeout, new CommandType?(CommandType.StoredProcedure));
    }

    public static IEnumerable<T> GetWithSp<T>(
      this QueryFactory db,
      string sql,
      DynamicParameters param = null,
      bool buffered = true,
      int? commandTimeout = null)
    {
      return SqlMapper.Query<T>(db.Connection, sql, (object) param, db.Transaction, buffered, commandTimeout, new CommandType?(CommandType.StoredProcedure));
    }

    public static async Task<IEnumerable<T>> SelectAsync<T>(
      this QueryFactory db,
      string sql,
      object param = null,
      int? commandTimeout = null)
    {
      IEnumerable<T> objs = await (Task<IEnumerable<T>>) SqlMapper.QueryAsync<T>(db.Connection, sql, param, db.Transaction, commandTimeout, new CommandType?());
      return objs;
    }

    public static async Task<IEnumerable<object>> SelectAsync(
      this QueryFactory db,
      string sql,
      object param = null,
      int? commandTimeout = null)
    {
      IEnumerable<object> objects = await db.SelectAsync<object>(sql, param, commandTimeout);
      return objects;
    }

    public static async Task<DbStream<T>> StreamSelectAsync<T>(
      this QueryFactory db,
      string sql,
      object param = null,
      int? commandTimeout = null)
    {
      IDataReader reader = await SqlMapper.ExecuteReaderAsync(db.Connection, sql, param, db.Transaction, commandTimeout, new CommandType?());
      DbStream<T> dbStream = new DbStream<T>(reader);
      reader = (IDataReader) null;
      return dbStream;
    }

    public static async Task<DbStream<object>> StreamSelectAsync(
      this QueryFactory db,
      string sql,
      object param = null,
      int? commandTimeout = null)
    {
      DbStream<object> dbStream = await db.StreamSelectAsync<object>(sql, param, commandTimeout);
      return dbStream;
    }

    public static async Task<int> StatementAsync(
      this QueryFactory db,
      string sql,
      object param = null,
      int? commandTimeout = null)
    {
      int num = await SqlMapper.ExecuteAsync(db.Connection, sql, param, db.Transaction, commandTimeout, new CommandType?());
      return num;
    }

    public static Task<int> ExecuteSpAsync(
      this QueryFactory db,
      string sql,
      DynamicParameters param = null,
      int? commandTimeout = null)
    {
      return SqlMapper.ExecuteAsync(db.Connection, sql, (object) param, db.Transaction, commandTimeout, new CommandType?(CommandType.StoredProcedure));
    }

    public static Task<IEnumerable<T>> GetWithSpAsync<T>(
      this QueryFactory db,
      string sql,
      DynamicParameters param = null,
      int? commandTimeout = null)
    {
      return SqlMapper.QueryAsync<T>(db.Connection, sql, (object) param, db.Transaction, commandTimeout, new CommandType?(CommandType.StoredProcedure));
    }

   // [AsyncIteratorStateMachine(typeof (QueryFactoryExtensions.\u003CGetStreamWithSpAsync\u003Ed__43<>))]
    //public static IAsyncEnumerable<T> GetStreamWithSpAsync<T>(
    //  this QueryFactory db,
    //  string sql,
    //  DynamicParameters param = null,
    //  int? commandTimeout = null)
    //{
    //  // ISSUE: object of a compiler-generated type is created
    //  return (IAsyncEnumerable<T>) new QueryFactoryExtensions.\u003CGetStreamWithSpAsync\u003Ed__43<T>(-2)
    //  {
    //    \u003C\u003E3__db = db,
    //    \u003C\u003E3__sql = sql,
    //    \u003C\u003E3__param = param,
    //    \u003C\u003E3__commandTimeout = commandTimeout
    //  };
    //}
  }
}
