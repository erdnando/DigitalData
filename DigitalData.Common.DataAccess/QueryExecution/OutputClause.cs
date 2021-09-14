// Decompiled with JetBrains decompiler
// Type: DigitalData.Common.DataAccess.QueryExecution.OutputClause
// Assembly: DigitalData.Common.DataAccess, Version=6.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 143C0880-64BB-461C-BA76-CECC458DBFC6
// Assembly location: D:\proyectos\ddocx\DigitalData.DDoc.UI.Webx\DigitalData.DDoc.UI.Web\lib\DigitalData.Common.DataAccess.dll

using SqlKata;
using System.Collections.Generic;

namespace DigitalData.Common.DataAccess.QueryExecution
{
  public class OutputClause : AbstractClause
  {
    public List<string> Outputs { get; set; }

    public string OutputTable { get; set; }

    public override AbstractClause Clone() => (AbstractClause) new OutputClause()
    {
      Outputs = this.Outputs,
      OutputTable = this.OutputTable
    };
  }
}
