
using DigitalData.Open.Common.Api.DataAccess;
using System;
using System.Collections.Generic;

namespace DigitalData.Open.Common.Api
{
  public interface IDdocDAL : IDisposable
  {
    string Name { get; }

    IActivityLogDAL ActivityLog { get; set; }

    IAdminDAL Admin { get; set; }

    ICollectionDAL Collections { get; set; }

    IDocumentDAL Documents { get; set; }

    IFolderDAL Folders { get; set; }

    IReportsDAL Reports { get; set; }

    ISearchDAL Search { get; set; }

    ISecurityDAL Security { get; set; }

    IViewerDAL Viewer { get; set; }

    void BeginTransaction();

    void CommitTransaction();

    IEnumerable<string> GetDatabaseTables();

    IEnumerable<string> GetTableSchema(string tableName);

    bool TableExists(string tableName);
  }
}
