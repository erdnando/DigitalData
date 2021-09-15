// Decompiled with JetBrains decompiler
// Type: DigitalData.DDoc.Modules.DataAccess.ViewerDAL
// Assembly: DigitalData.DDoc.Modules.DataAccess.SqlServer, Version=4.210.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EB5ECC0D-3654-4980-9D29-200BC39CB926
// Assembly location: D:\proyectos\ddocx\DigitalData.DDoc.UI.Webx\DigitalData.DDoc.UI.Web\bin\DigitalData.DDoc.Modules.DataAccess.SqlServer.dll

using DigitalData.Common.DataAccess;
using DigitalData.Common.DataAccess.QueryExecution;
using DigitalData.Common.DataAccess.SqlServer;
using DigitalData.Open.Common.Api.DataAccess;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace DigitalData.Open.Modules.DataAccess
{
  public class ViewerDAL : SqlServerDAL, IViewerDAL, ICommonDAL
  {
    public ViewerDAL(SqlConnection connection, SqlTransaction transaction = null)
      : base(connection, transaction)
    {
    }

    public Task<string> GetPageType(string pageId) => (Task<string>) QueryExtensionsAsync.SingleAsync<string>(base.db.Query("G_PAGINAS").Where("GID", (object) pageId).Select("EXTENSION"), new int?());

    public Task<string> GetWarehousePath(string pageId) => (Task<string>) QueryExtensionsAsync.SingleAsync<string>(base.db.Query("G_PATH as P").Join("G_PAGINAS as F", "F.PATH_ID", "P.PATH_ID").Where("F.GID", (object) pageId).Select("P.PATH_RAIZ"), new int?());

    //[SpecialName]
    //IDbTransaction ICommonDAL.get_Transaction() => ((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Transaction;

    //[SpecialName]
    //void ICommonDAL.set_Transaction(IDbTransaction value) => ((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Transaction = value;
  }
}
