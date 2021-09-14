// Decompiled with JetBrains decompiler
// Type: DigitalData.Common.DataAccess.QueryExecution.QueryAggregateExtensions
// Assembly: DigitalData.Common.DataAccess, Version=6.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 143C0880-64BB-461C-BA76-CECC458DBFC6
// Assembly location: D:\proyectos\ddocx\DigitalData.DDoc.UI.Webx\DigitalData.DDoc.UI.Web\lib\DigitalData.Common.DataAccess.dll

using SqlKata;

namespace DigitalData.Common.DataAccess.QueryExecution
{
  public static class QueryAggregateExtensions
  {
    public static T Aggregate<T>(
      this Query query,
      string aggregateOperation,
      params string[] columns)
    {
      QueryFactory queryFactory = QueryHelper.CreateQueryFactory(query);
      return queryFactory.ExecuteScalar<T>(query.AsAggregate(aggregateOperation, columns), queryFactory.Transaction);
    }

    public static T Count<T>(this Query query, params string[] columns)
    {
      QueryFactory queryFactory = QueryHelper.CreateQueryFactory(query);
      return queryFactory.ExecuteScalar<T>(query.AsCount(columns), queryFactory.Transaction);
    }

    public static T Average<T>(this Query query, string column) => query.Aggregate<T>("avg", column);

    public static T Sum<T>(this Query query, string column) => query.Aggregate<T>("sum", column);

    public static T Min<T>(this Query query, string column) => query.Aggregate<T>("min", column);

    public static T Max<T>(this Query query, string column) => query.Aggregate<T>("max", column);
  }
}
