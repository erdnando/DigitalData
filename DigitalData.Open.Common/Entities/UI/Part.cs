// Decompiled with JetBrains decompiler
// Type: DigitalData.DDoc.Common.Entities.UI.Part
// Assembly: DigitalData.DDoc.Common, Version=4.210.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 37D7F3E1-2CA5-48B0-AB22-FE81D67009C0
// Assembly location: C:\Users\herna\Downloads\ss\BradesCard_DEV\bin\DigitalData.DDoc.Common.dll

using System;

namespace DigitalData.Open.Common.Entities.UI
{
  public class Part
  {
    public Decimal Amount { get; set; }

    public DigitalData.Open.Common.Entities.UI.CustomsInformation[] CustomsInformation { get; set; }

    public string Description { get; set; }

    public string IdentificationNumber { get; set; }

    public Decimal Quantity { get; set; }

    public string Unit { get; set; }

    public Decimal UnitValue { get; set; }
  }
}
