// Decompiled with JetBrains decompiler
// Type: DigitalData.Common.WebUtils.AspNet4.SourceList
// Assembly: DigitalData.Common.WebUtils.AspNet4, Version=6.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 74D4764F-0348-418D-8077-4ECEFD368363
// Assembly location: C:\Users\herna\Downloads\ss\BradesCard_DEV\bin\DigitalData.Common.WebUtils.AspNet4.dll

using System.Collections;
using System.Collections.Generic;

namespace DigitalData.Common.WebUtils.AspNet4
{
  public class SourceList : IEnumerable<SourceListItem>, IEnumerable
  {
    public SourceList(IEnumerable<SourceListItem> items) => this.Items = items;

    public IEnumerable<SourceListItem> Items { get; }

    public virtual IEnumerator<SourceListItem> GetEnumerator() => this.Items.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();
  }
}
