// Decompiled with JetBrains decompiler
// Type: DigitalData.Common.DataAccess.QueryExecution.DbStream`1
// Assembly: DigitalData.Common.DataAccess, Version=6.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 143C0880-64BB-461C-BA76-CECC458DBFC6
// Assembly location: D:\proyectos\ddocx\DigitalData.DDoc.UI.Webx\DigitalData.DDoc.UI.Web\lib\DigitalData.Common.DataAccess.dll

using Dapper;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

namespace DigitalData.Common.DataAccess.QueryExecution
{
  public class DbStream<T> : IEnumerator<T>, IEnumerator, IDisposable
  {
    private readonly IDataReader reader;

    public DbStream(IDataReader reader) => this.reader = reader;

    public bool MoveNext() => this.reader.Read();

    public void Reset() => throw new NotSupportedException();

    object IEnumerator.Current => (object) this.Current;

    public T Current => ((Func<IDataReader, T>) SqlMapper.GetRowParser<T>(this.reader, typeof (T), 0, -1, false))(this.reader);

    public void Dispose() => this.reader.Dispose();
  }
}
