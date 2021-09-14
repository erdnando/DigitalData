// Decompiled with JetBrains decompiler
// Type: DigitalData.Common.DataAccess.QueryExecution.QueryAggregateExtensionsAsync
// Assembly: DigitalData.Common.DataAccess, Version=6.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 143C0880-64BB-461C-BA76-CECC458DBFC6
// Assembly location: D:\proyectos\ddocx\DigitalData.DDoc.UI.Webx\DigitalData.DDoc.UI.Web\lib\DigitalData.Common.DataAccess.dll

using Dapper;
using SqlKata;
using System.Data;
using System.Threading.Tasks;

namespace DigitalData.Common.DataAccess.QueryExecution
{
  public static class QueryAggregateExtensionsAsync
  {
    public static async Task<T> AggregateAsync<T>(
      this Query query,
      string aggregateOperation,
      params string[] columns)
    {
      XQuery xQuery = QueryHelper.CastToXQuery(query, nameof (AggregateAsync));
      SqlResult result = xQuery.Compiler.Compile(query.AsAggregate(aggregateOperation, columns));
      T obj = await (Task<T>) SqlMapper.ExecuteScalarAsync<T>(xQuery.Connection, result.Sql, (object) result.NamedBindings, xQuery.Transaction, new int?(), new CommandType?());
      T scalar = obj;
      obj = default (T);
      T obj1 = scalar;
      xQuery = (XQuery) null;
      result = (SqlResult) null;
      scalar = default (T);
      return obj1;
    }

    public static async Task<T> CountAsync<T>(this Query query, params string[] columns)
    {
      XQuery xQuery = QueryHelper.CastToXQuery(query, nameof (CountAsync));
      SqlResult result = xQuery.Compiler.Compile(query.AsCount(columns));
      T obj = await (Task<T>) SqlMapper.ExecuteScalarAsync<T>(xQuery.Connection, result.Sql, (object) result.NamedBindings, xQuery.Transaction, new int?(), new CommandType?());
      T scalar = obj;
      obj = default (T);
      T obj1 = scalar;
      xQuery = (XQuery) null;
      result = (SqlResult) null;
      scalar = default (T);
      return obj1;
    }

    public static async Task<T> AverageAsync<T>(this Query query, string column)
    {
      XQuery xQuery = QueryHelper.CastToXQuery(query, nameof (AverageAsync));
      T obj = await query.AggregateAsync<T>("avg", column);
      xQuery = (XQuery) null;
      return obj;
    }

    public static async Task<T> SumAsync<T>(this Query query, string column)
    {
      XQuery xQuery = QueryHelper.CastToXQuery(query, nameof (SumAsync));
      T obj = await query.AggregateAsync<T>("sum", column);
      xQuery = (XQuery) null;
      return obj;
    }

    public static async Task<T> MinAsync<T>(this Query query, string column)
    {
      XQuery xQuery = QueryHelper.CastToXQuery(query, nameof (MinAsync));
      T obj = await query.AggregateAsync<T>("min", column);
      xQuery = (XQuery) null;
      return obj;
    }

    public static async Task<T> MaxAsync<T>(this Query query, string column)
    {
      XQuery xQuery = QueryHelper.CastToXQuery(query, nameof (MaxAsync));
      T obj = await query.AggregateAsync<T>("max", column);
      xQuery = (XQuery) null;
      return obj;
    }
  }
}
