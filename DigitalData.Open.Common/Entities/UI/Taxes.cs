// Decompiled with JetBrains decompiler
// Type: DigitalData.DDoc.Common.Entities.UI.Taxes
// Assembly: DigitalData.DDoc.Common, Version=4.210.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 37D7F3E1-2CA5-48B0-AB22-FE81D67009C0
// Assembly location: C:\Users\herna\Downloads\ss\BradesCard_DEV\bin\DigitalData.DDoc.Common.dll

using System;

namespace DigitalData.Open.Common.Entities.UI
{
  public class Taxes
  {
    public WithheldTaxes[] Retentions { get; set; }

    public Decimal TotalTransferredTaxes { get; set; }

    public Decimal TotalWithheldTaxes { get; set; }

    public TransferredTaxes[] Transfers { get; set; }
  }
}
