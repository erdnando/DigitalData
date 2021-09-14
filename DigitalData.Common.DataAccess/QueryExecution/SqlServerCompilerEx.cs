// Decompiled with JetBrains decompiler
// Type: DigitalData.Common.DataAccess.QueryExecution.SqlServerCompilerEx
// Assembly: DigitalData.Common.DataAccess, Version=6.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 143C0880-64BB-461C-BA76-CECC458DBFC6
// Assembly location: D:\proyectos\ddocx\DigitalData.DDoc.UI.Webx\DigitalData.DDoc.UI.Web\lib\DigitalData.Common.DataAccess.dll


using SqlKata;
using SqlKata.Compilers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DigitalData.Common.DataAccess.QueryExecution
{
  public class SqlServerCompilerEx : SqlServerCompiler
  {
    protected override SqlResult CompileRaw(Query query)
    {
      SqlResult sqlResult1;
      if (query.Method == "insert")
        sqlResult1 = this.CompileInsertQuery(query);
      else if (query.Method == "update")
        sqlResult1 = this.CompileUpdateQuery(query);
      else if (query.Method == "delete")
      {
        sqlResult1 = this.CompileDeleteQuery(query);
      }
      else
      {
        if (query.Method == "aggregate")
          query.ClearComponent("limit").ClearComponent("order").ClearComponent("group");
        sqlResult1 = this.CompileSelectQuery(query);
      }
      if (query.HasComponent("cte", this.EngineCode))
      {
        List<AbstractFrom> abstractFromList = new CteFinder(query, this.EngineCode).Find();
        StringBuilder stringBuilder = new StringBuilder("WITH ");
        List<object> objectList = new List<object>();
        foreach (AbstractFrom cte in abstractFromList)
        {
          SqlResult sqlResult2 = this.CompileCte(cte);
          objectList.AddRange((IEnumerable<object>) sqlResult2.Bindings);
          stringBuilder.Append(sqlResult2.RawSql.Trim());
          stringBuilder.Append(",\n");
        }
        stringBuilder.Length -= 2;
        stringBuilder.Append('\n');
        stringBuilder.Append(sqlResult1.RawSql);
        sqlResult1.Bindings.InsertRange(0, (IEnumerable<object>) objectList);
        sqlResult1.RawSql = stringBuilder.ToString();
      }
      sqlResult1.RawSql = Helper.ExpandParameters(sqlResult1.RawSql, "?", sqlResult1.Bindings.ToArray());
      return sqlResult1;
    }

    private new SqlResult CompileUpdateQuery(Query query)
    {
      SqlResult ctx = new SqlResult()
      {
        Query = query
      };
      if (!ctx.Query.HasComponent("from", this.EngineCode))
        throw new InvalidOperationException("No table set to update");
      AbstractFrom oneComponent1 = ctx.Query.GetOneComponent<AbstractFrom>("from", this.EngineCode);
      if (!(oneComponent1 is FromClause))
        throw new InvalidOperationException("Invalid table expression");
      InsertClause oneComponent2 = ctx.Query.GetOneComponent<InsertClause>("update", this.EngineCode);
      List<string> stringList = new List<string>();
      foreach (string column in oneComponent2.Columns)
        stringList.Add(this.Wrap(column) + " = ?");
      ctx.Bindings.AddRange((IEnumerable<object>) oneComponent2.Values);
      string str1 = this.CompileOutput(ctx);
      string str2 = this.CompileWheres(ctx);
      if (!string.IsNullOrEmpty(str2))
        str2 = " " + str2;
      ctx.RawSql = "UPDATE " + this.CompileTableExpression(ctx, oneComponent1) + " SET " + string.Join(", ", (IEnumerable<string>) stringList) + str1 + str2;
      return ctx;
    }

    private new SqlResult CompileDeleteQuery(Query query)
    {
      SqlResult ctx = new SqlResult()
      {
        Query = query
      };
      if (!ctx.Query.HasComponent("from", this.EngineCode))
        throw new InvalidOperationException("No table set to delete");
      AbstractFrom oneComponent = ctx.Query.GetOneComponent<AbstractFrom>("from", this.EngineCode);
      if (!(oneComponent is FromClause))
        throw new InvalidOperationException("Invalid table expression");
      string str1 = this.CompileOutput(ctx);
      string str2 = this.CompileWheres(ctx);
      if (!string.IsNullOrEmpty(str2))
        str2 = " " + str2;
      ctx.RawSql = "DELETE FROM " + this.CompileTableExpression(ctx, oneComponent) + str1 + str2;
      return ctx;
    }

    private string CompileOutput(SqlResult ctx)
    {
      if (!ctx.Query.HasComponent("from", this.EngineCode) || !ctx.Query.HasComponent("output", this.EngineCode))
        return (string) null;
      OutputClause oneComponent = ctx.Query.GetOneComponent<OutputClause>("output", this.EngineCode);
      List<string> source = new List<string>();
      if (oneComponent.Outputs != null)
      {
        foreach (string output in oneComponent.Outputs)
          source.Add(this.Wrap(output) ?? "");
      }
      string str = (source.Any<string>() ? " " + string.Join(",", (IEnumerable<string>) source) + " " : " deleted.* ") + (!string.IsNullOrEmpty(oneComponent.OutputTable) ? " INTO " + oneComponent.OutputTable + " " : string.Empty);
      return string.IsNullOrEmpty(str) ? (string) null : " OUTPUT " + str;
    }
  }
}
