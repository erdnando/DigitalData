// Decompiled with JetBrains decompiler
// Type: DigitalData.Common.DataAccess.QueryExecution.PaginationResult`1
// Assembly: DigitalData.Common.DataAccess, Version=6.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 143C0880-64BB-461C-BA76-CECC458DBFC6
// Assembly location: D:\proyectos\ddocx\DigitalData.DDoc.UI.Webx\DigitalData.DDoc.UI.Web\lib\DigitalData.Common.DataAccess.dll

using SqlKata;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DigitalData.Common.DataAccess.QueryExecution
{
  public class PaginationResult<T>
  {
    public bool Buffered { get; set; }

    public int? CommandTimeout { get; set; }

    public long Count { get; set; }

    public PaginationIterator<T> Each => new PaginationIterator<T>()
    {
      FirstPage = this
    };

    public bool HasNext => this.Page < this.TotalPages;

    public bool HasPrevious => this.Page > 1;

    public bool IsFirst => this.Page == 1;

    public bool IsLast => this.Page == this.TotalPages;

    public IEnumerable<T> List { get; set; }

    public int Page { get; set; }

    public int PerPage { get; set; }

    public Query Query { get; set; }

    public int TotalPages => this.PerPage < 1 ? 0 : (int) Math.Ceiling((double) ((float) this.Count / (float) this.PerPage));

    public Query NextQuery() => this.Query.ForPage(this.Page + 1, this.PerPage);

    public PaginationResult<T> Next() => this.Query.Paginate<T>(this.Page + 1, this.PerPage, this.Buffered, this.CommandTimeout);

    public async Task<PaginationResult<T>> NextAsync()
    {
      PaginationResult<T> paginationResult = await this.Query.PaginateAsync<T>(this.Page + 1, this.PerPage, this.CommandTimeout);
      return paginationResult;
    }

    public Query PreviousQuery() => this.Query.ForPage(this.Page - 1, this.PerPage);

    public PaginationResult<T> Previous() => this.Query.Paginate<T>(this.Page - 1, this.PerPage, this.Buffered, this.CommandTimeout);

    public async Task<PaginationResult<T>> PreviousAsync()
    {
      PaginationResult<T> paginationResult = await this.Query.PaginateAsync<T>(this.Page - 1, this.PerPage, this.CommandTimeout);
      return paginationResult;
    }
  }
}
