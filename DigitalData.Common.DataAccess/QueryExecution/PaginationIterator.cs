// Decompiled with JetBrains decompiler
// Type: DigitalData.Common.DataAccess.QueryExecution.PaginationIterator`1
// Assembly: DigitalData.Common.DataAccess, Version=6.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 143C0880-64BB-461C-BA76-CECC458DBFC6
// Assembly location: D:\proyectos\ddocx\DigitalData.DDoc.UI.Webx\DigitalData.DDoc.UI.Web\lib\DigitalData.Common.DataAccess.dll

using System.Collections;
using System.Collections.Generic;

namespace DigitalData.Common.DataAccess.QueryExecution
{
  public class PaginationIterator<T> : IEnumerable<PaginationResult<T>>, IEnumerable
  {
    public PaginationResult<T> FirstPage { get; set; }

    public PaginationResult<T> CurrentPage { get; set; }

    public IEnumerator<PaginationResult<T>> GetEnumerator()
    {
      this.CurrentPage = this.FirstPage;
      yield return this.CurrentPage;
      while (this.CurrentPage.HasNext)
      {
        this.CurrentPage = this.CurrentPage.Next();
        yield return this.CurrentPage;
      }
    }

    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();
  }
}
