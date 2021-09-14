// Decompiled with JetBrains decompiler
// Type: DigitalData.DDoc.Common.Entities.UI.UiTreeNode
// Assembly: DigitalData.DDoc.Common, Version=4.210.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 37D7F3E1-2CA5-48B0-AB22-FE81D67009C0
// Assembly location: C:\Users\herna\Downloads\ss\BradesCard_DEV\bin\DigitalData.DDoc.Common.dll

using System.Collections.Generic;

namespace DigitalData.DDoc.Common.Entities.UI
{
  public class UiTreeNode
  {
    public bool active { get; set; }

    public bool checkbox { get; set; }

    public List<UiTreeNode> children { get; set; }

    public bool expanded { get; set; }

    public string extraClasses { get; set; }

    public string collection { get; set; }

    public bool focus { get; set; }

    public bool folder { get; set; }

    public string icon { get; set; }

    public string iconTooltip { get; set; }

    public string key { get; set; }

    public bool lazy { get; set; }

    public string path { get; set; }

    public bool selected { get; set; }

    public string statusNodeType { get; set; }

    public string title { get; set; }

    public string tooltip { get; set; }

    public string type { get; set; }

    public bool unselectable { get; set; }
  }
}
