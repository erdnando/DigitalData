// Decompiled with JetBrains decompiler
// Type: DigitalData.DDoc.Common.Extensions.DdocCustomAction
// Assembly: DigitalData.DDoc.Common, Version=4.210.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 37D7F3E1-2CA5-48B0-AB22-FE81D67009C0
// Assembly location: C:\Users\herna\Downloads\ss\BradesCard_DEV\bin\DigitalData.DDoc.Common.dll

using DigitalData.Open.Common.Entities;

namespace DigitalData.Open.Common.Extensions
{
  public class DdocCustomAction
  {
    public CustomActionType Type { get; set; }

    public string CollectionId { get; set; }

    public string Action { get; set; }

    public string Command { get; set; }

    public string Label { get; set; }

    public string Controller { get; set; }

    public string ImageClass { get; set; }

    public string Tooltip { get; set; }

    public string Target { get; set; }

    public string Url { get; set; }

    public CellType CellType { get; set; }
  }
}
