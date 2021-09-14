// Decompiled with JetBrains decompiler
// Type: DigitalData.Common.DataAccess.QueryExecution.QueryExtensions
// Assembly: DigitalData.Common.DataAccess, Version=6.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 143C0880-64BB-461C-BA76-CECC458DBFC6
// Assembly location: D:\proyectos\ddocx\DigitalData.DDoc.UI.Webx\DigitalData.DDoc.UI.Web\lib\DigitalData.Common.DataAccess.dll

using Dapper;
using DigitalData.Common.Entities;
using SqlKata;
using SqlKata.Compilers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace DigitalData.Common.DataAccess.QueryExecution
{
  public static class QueryExtensions
  {
    public static Query AsUpdate(
      this Query query,
      object data,
      IEnumerable<string> outputFields = null,
      string outputTable = null)
    {
      Dictionary<string, object> dictionary = new Dictionary<string, object>();
      foreach (PropertyInfo runtimeProperty in data.GetType().GetRuntimeProperties())
        dictionary.Add(runtimeProperty.Name, runtimeProperty.GetValue(data));
      return QueryExtensions.AsUpdate(query, (IReadOnlyDictionary<string, object>) dictionary, outputFields, outputTable);
    }

    public static Query AsUpdate(
      this Query query,
      IEnumerable<string> columns,
      IEnumerable<object> values,
      IEnumerable<string> outputFields = null,
      string outputTable = null)
    {
      if (columns == null || columns.Count<string>() == 0 || (values != null ? values.Count<object>() : 0) == 0)
        throw new InvalidOperationException("Columns and Values cannot be null or empty");
      if (columns.Count<string>() != values.Count<object>())
        throw new InvalidOperationException("Columns count should be equal to Values count");
      query.Method = "update";
      query.ClearComponent("update").AddComponent("update", (AbstractClause) new InsertClause()
      {
        Columns = columns.ToList<string>(),
        Values = values.ToList<object>()
      });
      query.ClearComponent("output").AddComponent("output", (AbstractClause) new OutputClause()
      {
        Outputs = (outputFields != null ? outputFields.ToList<string>() : (List<string>) null),
        OutputTable = outputTable
      });
      return query;
    }

    public static Query AsUpdate(
      this Query query,
      IReadOnlyDictionary<string, object> data,
      IEnumerable<string> outputFields = null,
      string outputTable = null)
    {
      if (data == null || data.Count == 0)
        throw new InvalidOperationException("Values dictionary cannot be null or empty");
      query.Method = "update";
      query.ClearComponent("update").AddComponent("update", (AbstractClause) new InsertClause()
      {
        Columns = data.Keys.ToList<string>(),
        Values = data.Values.ToList<object>()
      });
      query.ClearComponent("output").AddComponent("output", (AbstractClause) new OutputClause()
      {
        Outputs = (outputFields != null ? outputFields.ToList<string>() : (List<string>) null),
        OutputTable = outputTable
      });
      return query;
    }

    public static Query AsDelete(
      this Query query,
      IEnumerable<string> outputFields = null,
      string outputTable = null)
    {
      query.Method = "delete";
      query.ClearComponent("output").AddComponent("output", (AbstractClause) new OutputClause()
      {
        Outputs = (outputFields != null ? outputFields.ToList<string>() : (List<string>) null),
        OutputTable = outputTable
      });
      return query;
    }

    public static SqlResult Compile<TCompiler>(this Query query) where TCompiler : Compiler, new() => new TCompiler().Compile(query);

    public static IEnumerable<T> Get<T>(
      this Query query,
      bool buffered = true,
      int? commandTimeout = null)
    {
      return QueryHelper.CreateQueryFactory(query).Get<T>(query, buffered, commandTimeout);
    }

    public static IEnumerable<IDictionary<string, object>> GetDictionary(
      this Query query,
      bool buffered = true,
      int? commandTimeout = null)
    {
      return QueryHelper.CreateQueryFactory(query).GetDictionary(query, buffered, commandTimeout);
    }

    public static IEnumerable<object> Get(
      this Query query,
      bool buffered = true,
      int? commandTimeout = null)
    {
      return query.Get<object>(buffered, commandTimeout);
    }

    public static DbStream<T> GetAsDbStream<T>(this Query query, int? commandTimeout = null) => QueryHelper.CreateQueryFactory(query).GetAsDbStream<T>(query, commandTimeout);

    public static DbStream<object> GetAsDbStream(this Query query, int? commandTimeout = null) => query.GetAsDbStream<object>(commandTimeout);

    public static T SingleOrDefault<T>(this Query query, int? commandTimeout = null) => QueryHelper.CreateQueryFactory(query).SingleOrDefault<T>(query, commandTimeout);

    public static object SingleOrDefault(this Query query, int? commandTimeout = null) => query.SingleOrDefault<object>(commandTimeout);

    public static T Single<T>(this Query query, int? commandTimeout = null) => QueryHelper.CreateQueryFactory(query).Single<T>(query, commandTimeout);

    public static object Single(this Query query, int? commandTimeout = null) => query.Single<object>(commandTimeout);

    public static T FirstOrDefault<T>(this Query query, int? commandTimeout = null) => QueryHelper.CreateQueryFactory(query).FirstOrDefault<T>(query, commandTimeout);

    public static object FirstOrDefault(this Query query, int? commandTimeout = null) => query.FirstOrDefault<object>(commandTimeout);

    public static T First<T>(this Query query, int? commandTimeout = null) => QueryHelper.CreateQueryFactory(query).First<T>(query, commandTimeout);

    public static object First(this Query query, int? commandTimeout = null) => query.First<object>(commandTimeout);

    public static bool Exists(this Query query, int? commandTimeout = null) => QueryHelper.CreateQueryFactory(query).Exists(query, commandTimeout);

    public static PaginationResult<T> Paginate<T>(
      this Query query,
      int page,
      int perPage = 25,
      bool buffered = true,
      int? commandTimeout = null)
    {
      return QueryHelper.CreateQueryFactory(query).Paginate<T>(query, page, perPage, buffered, commandTimeout);
    }

    public static PaginationResult<object> Paginate(
      this Query query,
      int page,
      int perPage = 25,
      bool buffered = true,
      int? commandTimeout = null)
    {
      return query.Paginate<object>(page, perPage, buffered, commandTimeout);
    }

    public static void Chunk<T>(
      this Query query,
      int chunkSize,
      Func<IEnumerable<T>, int, bool> func,
      bool buffered = true,
      int? commandTimeout = null)
    {
      QueryHelper.CreateQueryFactory(query).Chunk<T>(query, chunkSize, func, buffered, commandTimeout);
    }

    public static void Chunk(
      this Query query,
      int chunkSize,
      Func<IEnumerable<object>, int, bool> func,
      bool buffered = true,
      int? commandTimeout = null)
    {
      query.Chunk<object>(chunkSize, func, buffered, commandTimeout);
    }

    public static void Chunk<T>(
      this Query query,
      int chunkSize,
      Action<IEnumerable<T>, int> action,
      bool buffered = true,
      int? commandTimeout = null)
    {
      QueryHelper.CreateQueryFactory(query).Chunk<T>(query, chunkSize, action, buffered, commandTimeout);
    }

    public static void Chunk(
      this Query query,
      int chunkSize,
      Action<IEnumerable<object>, int> action,
      bool buffered = true,
      int? commandTimeout = null)
    {
      query.Chunk<object>(chunkSize, action, buffered, commandTimeout);
    }

    public static IEnumerable<T> Chunk<T>(
      this Query query,
      int chunkSize,
      bool buffered = true,
      int? commandTimeout = null)
    {
      return QueryHelper.CreateQueryFactory(query).Chunk<T>(query, chunkSize, buffered, commandTimeout);
    }

    public static IEnumerable<object> Chunk(
      this Query query,
      int chunkSize,
      bool buffered = true,
      int? commandTimeout = null)
    {
      return query.Chunk<object>(chunkSize, buffered, commandTimeout);
    }

    public static int Insert(
      this Query query,
      IReadOnlyDictionary<string, object> values,
      int? commandTimeout = null)
    {
      XQuery xquery = QueryHelper.CastToXQuery(query, nameof (Insert));
      SqlResult sqlResult = xquery.Compiler.Compile(query.AsInsert((IEnumerable<KeyValuePair<string, object>>) values, false));
      xquery.Logger(sqlResult);
      return SqlMapper.Execute(xquery.Connection, sqlResult.Sql, (object) sqlResult.NamedBindings, xquery.Transaction, commandTimeout, new CommandType?());
    }

    public static int Insert(
      this Query query,
      IEnumerable<string> columns,
      IEnumerable<IEnumerable<object>> valuesCollection,
      int? commandTimeout = null)
    {
      XQuery xquery = QueryHelper.CastToXQuery(query, nameof (Insert));
      SqlResult sqlResult = xquery.Compiler.Compile(query.AsInsert(columns, valuesCollection));
      xquery.Logger(sqlResult);
      return SqlMapper.Execute(xquery.Connection, sqlResult.Sql, (object) sqlResult.NamedBindings, xquery.Transaction, commandTimeout, new CommandType?());
    }

    public static int Insert(
      this Query query,
      IEnumerable<string> columns,
      Query fromQuery,
      int? commandTimeout = null)
    {
      XQuery xquery = QueryHelper.CastToXQuery(query, nameof (Insert));
      SqlResult sqlResult = xquery.Compiler.Compile(query.AsInsert(columns, fromQuery));
      xquery.Logger(sqlResult);
      return SqlMapper.Execute(xquery.Connection, sqlResult.Sql, (object) sqlResult.NamedBindings, xquery.Transaction, commandTimeout, new CommandType?());
    }

    public static int Insert(this Query query, object data, int? commandTimeout = null)
    {
      XQuery xquery = QueryHelper.CastToXQuery(query, nameof (Insert));
      SqlResult sqlResult = xquery.Compiler.Compile(query.AsInsert(data));
      xquery.Logger(sqlResult);
      return SqlMapper.Execute(xquery.Connection, sqlResult.Sql, (object) sqlResult.NamedBindings, xquery.Transaction, commandTimeout, new CommandType?());
    }

    public static T InsertGetId<T>(this Query query, object data, int? commandTimeout = null)
    {
      XQuery xquery = QueryHelper.CastToXQuery(query, nameof (InsertGetId));
      SqlResult sqlResult = xquery.Compiler.Compile(query.AsInsert(data, true));
      xquery.Logger(sqlResult);
      return ((InsertGetIdRow<T>) SqlMapper.QueryFirst<InsertGetIdRow<T>>(xquery.Connection, sqlResult.Sql, (object) sqlResult.NamedBindings, xquery.Transaction, commandTimeout, new CommandType?())).Id;
    }

    public static T InsertGetId<T>(
      this Query query,
      IReadOnlyDictionary<string, object> values,
      int? commandTimeout = null)
    {
      XQuery xquery = QueryHelper.CastToXQuery(query, nameof (InsertGetId));
      SqlResult sqlResult = xquery.Compiler.Compile(query.AsInsert((IEnumerable<KeyValuePair<string, object>>) values, true));
      xquery.Logger(sqlResult);
      return ((InsertGetIdRow<T>) SqlMapper.QueryFirst<InsertGetIdRow<T>>(xquery.Connection, sqlResult.Sql, (object) sqlResult.NamedBindings, xquery.Transaction, commandTimeout, new CommandType?())).Id;
    }

    public static int Update(
      this Query query,
      IReadOnlyDictionary<string, object> values,
      int? commandTimeout = null)
    {
      XQuery xquery = QueryHelper.CastToXQuery(query, nameof (Update));
      SqlResult sqlResult = xquery.Compiler.Compile(query.AsUpdate((IEnumerable<KeyValuePair<string, object>>) values));
      xquery.Logger(sqlResult);
      return SqlMapper.Execute(xquery.Connection, sqlResult.Sql, (object) sqlResult.NamedBindings, xquery.Transaction, commandTimeout, new CommandType?());
    }

    public static IEnumerable<object> UpdateWithOutput(
      this Query query,
      IReadOnlyDictionary<string, object> values,
      IEnumerable<string> outputFields = null,
      string outputTable = null,
      bool buffered = true,
      int? commandTimeout = null)
    {
      XQuery xquery = QueryHelper.CastToXQuery(query, "Update");
      SqlResult sqlResult = xquery.Compiler.Compile(QueryExtensions.AsUpdate(query, values, outputFields, outputTable));
      xquery.Logger(sqlResult);
      return SqlMapper.Query(xquery.Connection, sqlResult.Sql, (object) sqlResult.NamedBindings, xquery.Transaction, buffered, commandTimeout, new CommandType?());
    }

    public static IEnumerable<T> UpdateWithOutput<T>(
      this Query query,
      IReadOnlyDictionary<string, object> values,
      IEnumerable<string> outputFields = null,
      string outputTable = null,
      bool buffered = true,
      int? commandTimeout = null)
    {
      XQuery xquery = QueryHelper.CastToXQuery(query, "Update");
      SqlResult sqlResult = xquery.Compiler.Compile(QueryExtensions.AsUpdate(query, values, outputFields, outputTable));
      xquery.Logger(sqlResult);
      return SqlMapper.Query<T>(xquery.Connection, sqlResult.Sql, (object) sqlResult.NamedBindings, xquery.Transaction, buffered, commandTimeout, new CommandType?());
    }

    public static int Update(this Query query, object data, int? commandTimeout = null)
    {
      XQuery xquery = QueryHelper.CastToXQuery(query, nameof (Update));
      SqlResult sqlResult = xquery.Compiler.Compile(query.AsUpdate(data));
      xquery.Logger(sqlResult);
      return SqlMapper.Execute(xquery.Connection, sqlResult.Sql, (object) sqlResult.NamedBindings, xquery.Transaction, commandTimeout, new CommandType?());
    }

    public static IEnumerable<object> UpdateWithOutput(
      this Query query,
      object data,
      IEnumerable<string> outputFields,
      string outputTable = null,
      bool buffered = true,
      int? commandTimeout = null)
    {
      XQuery xquery = QueryHelper.CastToXQuery(query, "Update");
      SqlResult sqlResult = xquery.Compiler.Compile(query.AsUpdate(data, outputFields, outputTable));
      xquery.Logger(sqlResult);
      return SqlMapper.Query(xquery.Connection, sqlResult.Sql, (object) sqlResult.NamedBindings, xquery.Transaction, buffered, commandTimeout, new CommandType?());
    }

    public static IEnumerable<T> UpdateWithOutput<T>(
      this Query query,
      object data,
      IEnumerable<string> outputFields,
      string outputTable = null,
      bool buffered = true,
      int? commandTimeout = null)
    {
      XQuery xquery = QueryHelper.CastToXQuery(query, "Update");
      SqlResult sqlResult = xquery.Compiler.Compile(query.AsUpdate(data, outputFields, outputTable));
      xquery.Logger(sqlResult);
      return SqlMapper.Query<T>(xquery.Connection, sqlResult.Sql, (object) sqlResult.NamedBindings, xquery.Transaction, buffered, commandTimeout, new CommandType?());
    }

    public static DbStream<T> UpdateWithStreamedOutput<T>(
      this Query query,
      IReadOnlyDictionary<string, object> values,
      IEnumerable<string> outputFields = null,
      string outputTable = null,
      int? commandTimeout = null)
    {
      XQuery xquery = QueryHelper.CastToXQuery(query, "Update");
      SqlResult sqlResult = xquery.Compiler.Compile(QueryExtensions.AsUpdate(query, values, outputFields, outputTable));
      xquery.Logger(sqlResult);
      return new DbStream<T>(SqlMapper.ExecuteReader(xquery.Connection, sqlResult.Sql, (object) sqlResult.NamedBindings, xquery.Transaction, commandTimeout, new CommandType?()));
    }

    public static DbStream<object> UpdateWithStreamedOutput(
      this Query query,
      IReadOnlyDictionary<string, object> values,
      IEnumerable<string> outputFields = null,
      string outputTable = null,
      int? commandTimeout = null)
    {
      return QueryExtensions.UpdateWithStreamedOutput<object>(query, values, outputFields, outputTable, commandTimeout);
    }

    public static DbStream<T> UpdateWithStreamedOutput<T>(
      this Query query,
      object data,
      IEnumerable<string> outputFields,
      string outputTable = null,
      int? commandTimeout = null)
    {
      XQuery xquery = QueryHelper.CastToXQuery(query, "Update");
      SqlResult sqlResult = xquery.Compiler.Compile(query.AsUpdate(data, outputFields, outputTable));
      xquery.Logger(sqlResult);
      return new DbStream<T>(SqlMapper.ExecuteReader(xquery.Connection, sqlResult.Sql, (object) sqlResult.NamedBindings, xquery.Transaction, commandTimeout, new CommandType?()));
    }

    public static DbStream<object> UpdateWithStreamedOutput(
      this Query query,
      object data,
      IEnumerable<string> outputFields,
      string outputTable = null,
      int? commandTimeout = null)
    {
      return query.UpdateWithStreamedOutput<object>(data, outputFields, outputTable, commandTimeout);
    }

    public static int Delete(this Query query, int? commandTimeout = null)
    {
      XQuery xquery = QueryHelper.CastToXQuery(query, nameof (Delete));
      SqlResult sqlResult = xquery.Compiler.Compile(query.AsDelete());
      xquery.Logger(sqlResult);
      return SqlMapper.Execute(xquery.Connection, sqlResult.Sql, (object) sqlResult.NamedBindings, xquery.Transaction, commandTimeout, new CommandType?());
    }

    public static IEnumerable<object> DeleteWithOutput(
      this Query query,
      IEnumerable<string> outputFields = null,
      string outputTable = null,
      bool buffered = true,
      int? commandTimeout = null)
    {
      XQuery xquery = QueryHelper.CastToXQuery(query, "Delete");
      SqlResult sqlResult = xquery.Compiler.Compile(query.AsDelete(outputFields, outputTable));
      xquery.Logger(sqlResult);
      return SqlMapper.Query(xquery.Connection, sqlResult.Sql, (object) sqlResult.NamedBindings, xquery.Transaction, buffered, commandTimeout, new CommandType?());
    }

    public static IEnumerable<T> DeleteWithOutput<T>(
      this Query query,
      IEnumerable<string> outputFields = null,
      string outputTable = null,
      bool buffered = true,
      int? commandTimeout = null)
    {
      XQuery xquery = QueryHelper.CastToXQuery(query, "Delete");
      SqlResult sqlResult = xquery.Compiler.Compile(query.AsDelete(outputFields, outputTable));
      xquery.Logger(sqlResult);
      return SqlMapper.Query<T>(xquery.Connection, sqlResult.Sql, (object) sqlResult.NamedBindings, xquery.Transaction, buffered, commandTimeout, new CommandType?());
    }

    public static DbStream<T> DeleteWithStreamedOutput<T>(
      this Query query,
      IEnumerable<string> outputFields = null,
      string outputTable = null,
      int? commandTimeout = null)
    {
      XQuery xquery = QueryHelper.CastToXQuery(query, "Delete");
      SqlResult sqlResult = xquery.Compiler.Compile(query.AsDelete(outputFields, outputTable));
      xquery.Logger(sqlResult);
      return new DbStream<T>(SqlMapper.ExecuteReader(xquery.Connection, sqlResult.Sql, (object) sqlResult.NamedBindings, xquery.Transaction, commandTimeout, new CommandType?()));
    }

    public static DbStream<object> DeleteWithStreamedOutput(
      this Query query,
      IEnumerable<string> outputFields = null,
      string outputTable = null,
      int? commandTimeout = null)
    {
      return query.DeleteWithStreamedOutput<object>(outputFields, outputTable, commandTimeout);
    }

    public static Query Where(this Query query, string column, Comparison op, bool value)
    {
      switch (op)
      {
        case Comparison.Equals:
          return query.Where(column, (object) value);
        case Comparison.IsNull:
          return query.WhereNull(column);
        case Comparison.NotNull:
          return query.WhereNotNull(column);
        default:
          throw new ArgumentOutOfRangeException(nameof (op), (object) op, (string) null);
      }
    }

    public static Query OrWhere(this Query query, string column, Comparison op, bool value)
    {
      switch (op)
      {
        case Comparison.Equals:
          return query.OrWhere(column, (object) value);
        case Comparison.IsNull:
          return query.OrWhereNull(column);
        case Comparison.NotNull:
          return query.OrWhereNotNull(column);
        default:
          throw new ArgumentOutOfRangeException(nameof (op), (object) op, (string) null);
      }
    }

    public static Query Where(this Query query, string column, Comparison op, string value)
    {
      switch (op)
      {
        case Comparison.Equals:
          return query.Where(column, (object) value);
        case Comparison.NotEquals:
          return query.Where(column, "!=", (object) value);
        case Comparison.Like:
          return query.WhereLike(column, (object) value);
        case Comparison.NotLike:
          return query.WhereNotLike(column, (object) value);
        case Comparison.Contains:
          return query.WhereContains(column, (object) value);
        case Comparison.NotContains:
          return query.WhereNotContains(column, (object) value);
        case Comparison.StartsWith:
          return query.WhereStarts(column, (object) value);
        case Comparison.NotStartsWith:
          return query.WhereNotStarts(column, (object) value);
        case Comparison.EndsWith:
          return query.WhereEnds(column, (object) value);
        case Comparison.NotEndsWith:
          return query.WhereNotEnds(column, (object) value);
        case Comparison.IsNull:
          return query.WhereNull(column);
        case Comparison.NotNull:
          return query.WhereNotNull(column);
        default:
          throw new ArgumentOutOfRangeException(nameof (op), (object) op, (string) null);
      }
    }

    public static Query OrWhere(this Query query, string column, Comparison op, string value)
    {
      switch (op)
      {
        case Comparison.Equals:
          return query.OrWhere(column, (object) value);
        case Comparison.NotEquals:
          return query.OrWhere(column, "!=", (object) value);
        case Comparison.Like:
          return query.OrWhereLike(column, (object) value);
        case Comparison.NotLike:
          return query.OrWhereNotLike(column, (object) value);
        case Comparison.Contains:
          return query.OrWhereContains(column, (object) value);
        case Comparison.NotContains:
          return query.OrWhereNotContains(column, (object) value);
        case Comparison.StartsWith:
          return query.OrWhereStarts(column, (object) value);
        case Comparison.NotStartsWith:
          return query.OrWhereNotStarts(column, (object) value);
        case Comparison.EndsWith:
          return query.OrWhereEnds(column, (object) value);
        case Comparison.NotEndsWith:
          return query.OrWhereNotEnds(column, (object) value);
        case Comparison.IsNull:
          return query.OrWhereNull(column);
        case Comparison.NotNull:
          return query.OrWhereNotNull(column);
        default:
          throw new ArgumentOutOfRangeException(nameof (op), (object) op, (string) null);
      }
    }

    public static Query Where<T>(
      this Query query,
      string column,
      Comparison op,
      IEnumerable<T> value)
      where T : struct
    {
      switch (op)
      {
        case Comparison.In:
          return query.WhereIn<T>(column, value);
        case Comparison.NotIn:
          return query.WhereNotIn<T>(column, value);
        case Comparison.Between:
          return query.WhereBetween<T>(column, value.ElementAt<T>(0), value.ElementAt<T>(1));
        case Comparison.NotBetween:
          return query.WhereBetween<T>(column, value.ElementAt<T>(0), value.ElementAt<T>(1));
        default:
          throw new ArgumentOutOfRangeException(nameof (op), (object) op, (string) null);
      }
    }

    public static Query OrWhere<T>(
      this Query query,
      string column,
      Comparison op,
      IEnumerable<T> value)
      where T : struct
    {
      switch (op)
      {
        case Comparison.In:
          return query.OrWhereIn<T>(column, value);
        case Comparison.NotIn:
          return query.OrWhereNotIn<T>(column, value);
        case Comparison.Between:
          return query.OrWhereBetween<T>(column, value.ElementAt<T>(0), value.ElementAt<T>(1));
        case Comparison.NotBetween:
          return query.OrWhereBetween<T>(column, value.ElementAt<T>(0), value.ElementAt<T>(1));
        default:
          throw new ArgumentOutOfRangeException(nameof (op), (object) op, (string) null);
      }
    }

    public static Query Where<T>(this Query query, string column, Comparison op, T value)
    {
      switch (op)
      {
        case Comparison.Equals:
          return query.Where(column, (object) value);
        case Comparison.NotEquals:
          return query.Where(column, "!=", (object) value);
        case Comparison.GreaterThan:
          return query.Where(column, ">", (object) value);
        case Comparison.GreaterOrEquals:
          return query.Where(column, ">=", (object) value);
        case Comparison.LessThan:
          return query.Where(column, "<", (object) value);
        case Comparison.LessOrEquals:
          return query.Where(column, "<=", (object) value);
        case Comparison.IsNull:
          return query.WhereNull(column);
        case Comparison.NotNull:
          return query.WhereNotNull(column);
        default:
          throw new ArgumentOutOfRangeException(nameof (op), (object) op, (string) null);
      }
    }

    public static Query OrWhere<T>(this Query query, string column, Comparison op, T value)
    {
      switch (op)
      {
        case Comparison.Equals:
          return query.OrWhere(column, (object) value);
        case Comparison.NotEquals:
          return query.OrWhere(column, "!=", (object) value);
        case Comparison.GreaterThan:
          return query.OrWhere(column, ">", (object) value);
        case Comparison.GreaterOrEquals:
          return query.OrWhere(column, ">=", (object) value);
        case Comparison.LessThan:
          return query.OrWhere(column, "<", (object) value);
        case Comparison.LessOrEquals:
          return query.OrWhere(column, "<=", (object) value);
        case Comparison.IsNull:
          return query.OrWhereNull(column);
        case Comparison.NotNull:
          return query.OrWhereNotNull(column);
        default:
          throw new ArgumentOutOfRangeException(nameof (op), (object) op, (string) null);
      }
    }
  }
}
