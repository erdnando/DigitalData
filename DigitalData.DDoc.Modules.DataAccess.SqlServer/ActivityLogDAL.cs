// Decompiled with JetBrains decompiler
// Type: DigitalData.DDoc.Modules.DataAccess.ActivityLogDAL
// Assembly: DigitalData.DDoc.Modules.DataAccess.SqlServer, Version=4.210.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EB5ECC0D-3654-4980-9D29-200BC39CB926
// Assembly location: D:\proyectos\ddocx\DigitalData.DDoc.UI.Webx\DigitalData.DDoc.UI.Web\bin\DigitalData.DDoc.Modules.DataAccess.SqlServer.dll

using DigitalData.Common.DataAccess;
using DigitalData.Common.DataAccess.QueryExecution;
using DigitalData.Common.DataAccess.SqlServer;
using DigitalData.Open.Common.Api.DataAccess;
using DigitalData.Open.Common.Entities.Helpers;
using SqlKata;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace DigitalData.Open.Modules.DataAccess
{
  public class ActivityLogDAL : SqlServerDAL, IActivityLogDAL, ICommonDAL
  {
    public ActivityLogDAL(SqlConnection connection, SqlTransaction transaction = null)
      : base(connection, transaction)
    {
    }

    public Task WriteLogEntry(DdocActionLogEntry entry)
    {
      Query query = base.db.Query("G_REGISTROACTIVIDAD");
      Dictionary<string, object> dictionary = new Dictionary<string, object>();
      dictionary.Add("Usuario_ID", (object) entry.User);
      dictionary.Add("FECHAHORA", (object) entry.EventDate);
      dictionary.Add("MODULO", (object) entry.Module);
      dictionary.Add("ACCION", (object) entry.Action);
      dictionary.Add("DETALLES", (object) entry.Details);
      int? nullable = new int?();
      return (Task) QueryExtensionsAsync.InsertAsync(query, (IReadOnlyDictionary<string, object>) dictionary, nullable);
    }

    //[SpecialName]
    //IDbTransaction ICommonDAL.get_Transaction() => ((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Transaction;

    //[SpecialName]
    //void ICommonDAL.set_Transaction(IDbTransaction value) => ((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Transaction = value;
  }
}
