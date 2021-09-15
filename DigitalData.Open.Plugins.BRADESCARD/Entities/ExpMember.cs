// Decompiled with JetBrains decompiler
// Type: DigitalData.Ddoc.Plugins.ExpMember
// Assembly: DigitalData.Ddoc.Plugins.BRADESCARD, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 76226255-9B51-4DC8-9719-6E02C225B847
// Assembly location: D:\proyectos\ddocx\DigitalData.DDoc.UI.Webx\DigitalData.DDoc.UI.Web\bin\DigitalData.Ddoc.Plugins.BRADESCARD.dll

using System.Collections.Generic;

namespace DigitalData.Ddoc.Plugins
{
  public class ExpMember
  {
    public string CollectionId { get; set; }

    public string CollectionName { get; set; }

    public List<ExpDocument> Documents { get; set; } = new List<ExpDocument>();
  }
}
