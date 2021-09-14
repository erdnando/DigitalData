// Decompiled with JetBrains decompiler
// Type: DigitalData.Common.DataAccess.QueryExecution.QueryHelper
// Assembly: DigitalData.Common.DataAccess, Version=6.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 143C0880-64BB-461C-BA76-CECC458DBFC6
// Assembly location: D:\proyectos\ddocx\DigitalData.DDoc.UI.Webx\DigitalData.DDoc.UI.Web\lib\DigitalData.Common.DataAccess.dll

using SqlKata;
using System;

namespace DigitalData.Common.DataAccess.QueryExecution
{
  public static class QueryHelper
  {
    public static XQuery CastToXQuery(Query query, string method = null)
    {
      if (query is XQuery xquery)
        return xquery;
      if (method == null)
        throw new InvalidOperationException("Execution methods can only be used with `XQuery` instances, consider using the `QueryFactory.Query()` to create executable queries, check https://sqlkata.com/docs/execution/setup#xquery-class for more info");
      throw new InvalidOperationException("The method $" + method + " can only be used with `XQuery` instances, consider using the `QueryFactory.Query()` to create executable queries, check https://sqlkata.com/docs/execution/setup#xquery-class for more info");
    }

    public static QueryFactory CreateQueryFactory(XQuery xQuery) => new QueryFactory(xQuery.Connection, xQuery.Compiler, xQuery.Transaction)
    {
      Logger = xQuery.Logger
    };

    public static QueryFactory CreateQueryFactory(Query query)
    {
      XQuery xquery = QueryHelper.CastToXQuery(query);
      return new QueryFactory(xquery.Connection, xquery.Compiler, xquery.Transaction)
      {
        Logger = xquery.Logger
      };
    }
  }
}
