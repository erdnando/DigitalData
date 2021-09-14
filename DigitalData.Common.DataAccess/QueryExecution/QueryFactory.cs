// Decompiled with JetBrains decompiler
// Type: DigitalData.Common.DataAccess.QueryExecution.QueryFactory
// Assembly: DigitalData.Common.DataAccess, Version=6.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 143C0880-64BB-461C-BA76-CECC458DBFC6
// Assembly location: D:\proyectos\ddocx\DigitalData.DDoc.UI.Webx\DigitalData.DDoc.UI.Web\lib\DigitalData.Common.DataAccess.dll

using SqlKata;
using SqlKata.Compilers;
using System;
using System.Data;
using System.Linq;

namespace DigitalData.Common.DataAccess.QueryExecution
{
  public class QueryFactory
  {
    public Action<SqlResult> Logger = (Action<SqlResult>) (result => { });

    public QueryFactory()
    {
    }

    public QueryFactory(IDbConnection connection, Compiler compiler, IDbTransaction transaction = null)
    {
      this.Connection = connection;
      this.Compiler = compiler;
      this.Transaction = transaction;
    }

    public Compiler Compiler { get; set; }

    public IDbConnection Connection { get; set; }

    public IDbTransaction Transaction { get; set; }

    public int QueryTimeout { get; set; } = 30;

    public SqlKata.Query Query() => (SqlKata.Query) new XQuery(this.Connection, this.Compiler, this.Transaction)
    {
      Logger = this.Logger
    };

    public SqlKata.Query Query(string table) => this.Query().From(table);

    public SqlKata.Query FromQuery(SqlKata.Query query)
    {
      XQuery xquery1 = new XQuery(this.Connection, this.Compiler, this.Transaction);
      xquery1.Clauses = query.Clauses.Select<AbstractClause, AbstractClause>((Func<AbstractClause, AbstractClause>) (x => x.Clone())).ToList<AbstractClause>();
      xquery1.QueryAlias = query.QueryAlias;
      xquery1.IsDistinct = query.IsDistinct;
      xquery1.Method = query.Method;
      XQuery xquery2 = xquery1;
      xquery2.SetEngineScope(query.EngineScope);
      xquery2.Logger = this.Logger;
      return (SqlKata.Query) xquery2;
    }
  }
}
