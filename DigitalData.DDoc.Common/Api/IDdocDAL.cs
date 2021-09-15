// Decompiled with JetBrains decompiler
// Type: DigitalData.DDoc.Common.Api.IDdocDAL
// Assembly: DigitalData.DDoc.Common, Version=4.210.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 37D7F3E1-2CA5-48B0-AB22-FE81D67009C0
// Assembly location: C:\Users\herna\Downloads\ss\BradesCard_DEV\bin\DigitalData.DDoc.Common.dll

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
