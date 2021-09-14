// Decompiled with JetBrains decompiler
// Type: DigitalData.Common.DataAccess.QueryHelpers
// Assembly: DigitalData.Common.DataAccess, Version=6.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 143C0880-64BB-461C-BA76-CECC458DBFC6
// Assembly location: D:\proyectos\ddocx\DigitalData.DDoc.UI.Webx\DigitalData.DDoc.UI.Web\lib\DigitalData.Common.DataAccess.dll

using Dapper;
using System.Collections.Generic;
using System.Data;

namespace DigitalData.Common.DataAccess
{
  public static class QueryHelpers
  {
    public static SqlMapper.ICustomQueryParameter ToTableValuedParameter<T>(
      this IEnumerable<T> values)
    {
      DataTable dataTable = new DataTable();
      dataTable.Columns.Add("Item", typeof (T));
      foreach (T obj in values)
        dataTable.Rows.Add((object) obj);
      return SqlMapper.AsTableValuedParameter(dataTable, (string) null);
    }
  }
}
