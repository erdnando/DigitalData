// Decompiled with JetBrains decompiler
// Type: DigitalData.Common.DataAccess.QueryExecution.XQuery
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
  public class XQuery : Query
  {
    public Action<SqlResult> Logger = (Action<SqlResult>) (result => { });

    public XQuery(IDbConnection connection, Compiler compiler, IDbTransaction transaction = null)
    {
      this.Connection = connection;
      this.Compiler = compiler;
      this.Transaction = transaction;
    }

    public Compiler Compiler { get; set; }

    public IDbConnection Connection { get; set; }

    public IDbTransaction Transaction { get; set; }

    public override Query Clone()
    {
      XQuery xquery1 = new XQuery(this.Connection, this.Compiler, this.Transaction);
      xquery1.Clauses = this.Clauses.Select<AbstractClause, AbstractClause>((Func<AbstractClause, AbstractClause>) (x => x.Clone())).ToList<AbstractClause>();
      xquery1.Logger = this.Logger;
      xquery1.QueryAlias = this.QueryAlias;
      xquery1.IsDistinct = this.IsDistinct;
      xquery1.Method = this.Method;
      XQuery xquery2 = xquery1;
      xquery2.SetEngineScope(this.EngineScope);
      return (Query) xquery2;
    }
  }
}
