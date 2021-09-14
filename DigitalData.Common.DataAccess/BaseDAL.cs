// Decompiled with JetBrains decompiler
// Type: DigitalData.Common.DataAccess.BaseDAL`2
// Assembly: DigitalData.Common.DataAccess, Version=6.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 143C0880-64BB-461C-BA76-CECC458DBFC6
// Assembly location: D:\proyectos\ddocx\DigitalData.DDoc.UI.Webx\DigitalData.DDoc.UI.Web\lib\DigitalData.Common.DataAccess.dll

using DigitalData.Common.DataAccess.QueryExecution;
using SqlKata.Compilers;
using System;
using System.Data;

namespace DigitalData.Common.DataAccess
{
  public abstract class BaseDAL<TConnectionType, TCompilerType> : IDisposable
    where TConnectionType : class, IDbConnection, new()
    where TCompilerType : Compiler, new()
  {
    protected readonly string ConnectionString;

    protected BaseDAL(string connString, bool useTransaction = false)
    {
      this.ConnectionString = connString;
      this.OpenConnection();
      if (!useTransaction)
        return;
      this.BeginTransaction();
    }

    protected BaseDAL(TConnectionType connection, IDbTransaction transaction = null)
    {
      this.OpenConnection(connection);
      if (transaction == null)
        return;
      this.BeginTransaction(transaction);
    }

    public TConnectionType Connection { get; private set; }

    protected QueryFactory db => new QueryFactory((IDbConnection) this.Connection, (Compiler) new TCompilerType(), this.Transaction);

    public IDbTransaction Transaction { get; set; }

    public void Dispose()
    {
      if ((object) this.Connection == null || this.Connection.State != ConnectionState.Open)
        return;
      this.Connection.Close();
      this.Connection.Dispose();
      this.Transaction?.Dispose();
      this.Transaction = (IDbTransaction) null;
    }

    protected void OpenConnection(TConnectionType connection = null)
    {
      if ((object) connection != null)
      {
        this.Connection = connection;
      }
      else
      {
        if (string.IsNullOrEmpty(this.ConnectionString))
          throw new InvalidOperationException("No se ha inicializado la cadena de conexion");
        TConnectionType connectionType = new TConnectionType();
        connectionType.ConnectionString = this.ConnectionString;
        this.Connection = connectionType;
        this.Connection.Open();
      }
    }

    public void BeginTransaction(IDbTransaction transaction = null)
    {
      if (transaction != null)
        this.Transaction = transaction;
      else
        this.Transaction = this.Connection.State == ConnectionState.Open ? this.Connection.BeginTransaction() : (IDbTransaction) null;
    }

    public void CommitTransaction()
    {
      this.Transaction.Commit();
      this.Transaction.Dispose();
      this.Transaction = (IDbTransaction) null;
    }

    public void RollbackTransaction()
    {
      this.Transaction.Rollback();
      this.Transaction.Dispose();
      this.Transaction = (IDbTransaction) null;
    }
  }
}
