// Decompiled with JetBrains decompiler
// Type: DigitalData.DDoc.Modules.DataAccess.DdocDAL
// Assembly: DigitalData.DDoc.Modules.DataAccess.SqlServer, Version=4.210.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EB5ECC0D-3654-4980-9D29-200BC39CB926
// Assembly location: D:\proyectos\ddocx\DigitalData.DDoc.UI.Webx\DigitalData.DDoc.UI.Web\bin\DigitalData.DDoc.Modules.DataAccess.SqlServer.dll

using DigitalData.Common.DataAccess;
using DigitalData.Common.DataAccess.QueryExecution;
using DigitalData.Common.DataAccess.SqlServer;
using DigitalData.Common.Entities;
using DigitalData.Common.IoC;
using DigitalData.Open.Common.Api;
using DigitalData.Open.Common.Api.DataAccess;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace DigitalData.DDoc.Modules.DataAccess
{
  public class DdocDAL : SqlServerDAL, IDdocDAL, IDisposable
  {
    public DdocDAL(bool useTransaction)
      : base(IoCContainer.GetService<IOptions<DiDaSettings>>().Value.GetConnectionSettings("DDocConnection"), useTransaction)
    {
    }

    public DdocDAL(IOptions<DiDaSettings> settings)
      : base(settings.Value.GetConnectionSettings("DDocConnection"), false)
    {
      this.ActivityLog = (IActivityLogDAL) new ActivityLogDAL(((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Connection, ((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Transaction as SqlTransaction);
      this.Admin = (IAdminDAL) new AdminDAL(((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Connection, ((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Transaction as SqlTransaction);
      this.Collections = (ICollectionDAL) new CollectionDAL(((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Connection, ((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Transaction as SqlTransaction);
      this.Documents = (IDocumentDAL) new DocumentDAL(((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Connection, ((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Transaction as SqlTransaction);
      this.Folders = (IFolderDAL) new FolderDAL(((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Connection, ((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Transaction as SqlTransaction);
      this.Reports = (IReportsDAL) new ReportsDAL(((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Connection, ((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Transaction as SqlTransaction);
      this.Search = (ISearchDAL) new SearchDAL(((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Connection, ((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Transaction as SqlTransaction);
      this.Security = (ISecurityDAL) new SecurityDAL(((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Connection, ((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Transaction as SqlTransaction);
      this.Viewer = (IViewerDAL) new ViewerDAL(((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Connection, ((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Transaction as SqlTransaction);
    }

    public string Name { get; } = "SqlServer";

    public IActivityLogDAL ActivityLog { get; set; }

    public IAdminDAL Admin { get; set; }

    public ICollectionDAL Collections { get; set; }

    public IDocumentDAL Documents { get; set; }

    public IFolderDAL Folders { get; set; }

    public IReportsDAL Reports { get; set; }

    public ISearchDAL Search { get; set; }

    public ISecurityDAL Security { get; set; }

    public IViewerDAL Viewer { get; set; }

    public void BeginTransaction()
    {
      ((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).BeginTransaction((IDbTransaction) null);
      this.ActivityLog.Transaction = ((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Transaction;
      this.Admin.Transaction = ((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Transaction;
      this.Collections.Transaction = ((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Transaction;
      this.Documents.Transaction = ((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Transaction;
      this.Folders.Transaction = ((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Transaction;
      this.Reports.Transaction = ((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Transaction;
      this.Search.Transaction = ((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Transaction;
      this.Security.Transaction = ((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Transaction;
      this.Viewer.Transaction = ((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).Transaction;
    }

    void IDdocDAL.CommitTransaction() => ((BaseDAL<SqlConnection, SqlServerCompilerEx>) this).CommitTransaction();

    IEnumerable<string> IDdocDAL.GetDatabaseTables() => this.GetDatabaseTables();

    IEnumerable<string> IDdocDAL.GetTableSchema(string tableName) => this.GetTableSchema(tableName);

    bool IDdocDAL.TableExists(string tableName) => this.TableExists(tableName);
  }
}
