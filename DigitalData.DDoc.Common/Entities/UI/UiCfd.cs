// Decompiled with JetBrains decompiler
// Type: DigitalData.DDoc.Common.Entities.UI.UiCfd
// Assembly: DigitalData.DDoc.Common, Version=4.210.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 37D7F3E1-2CA5-48B0-AB22-FE81D67009C0
// Assembly location: C:\Users\herna\Downloads\ss\BradesCard_DEV\bin\DigitalData.DDoc.Common.dll

using System;

namespace DigitalData.DDoc.Common.Entities.UI
{
  public class UiCfd
  {
    public Addenda Addenda { get; set; }

    public string Certificate { get; set; }

    public string CertificateNumber { get; set; }

    public Complement Complement { get; set; }

    public Concept[] Concepts { get; set; }

    public string Currency { get; set; }

    public DateTime Date { get; set; }

    public Decimal Discount { get; set; }

    public string DiscountReason { get; set; }

    public string ExchangeRate { get; set; }

    public string Folio { get; set; }

    public Issuer Issuer { get; set; }

    public string IssuingPlace { get; set; }

    public string OriginalFiscalFolio { get; set; }

    public Decimal OriginalFiscalFolioAmount { get; set; }

    public DateTime OriginalFiscalFolioDate { get; set; }

    public string OriginalFiscalFolioSeries { get; set; }

    public string PaymentAccountNumber { get; set; }

    public string PaymentConditions { get; set; }

    public string PaymentForm { get; set; }

    public string PaymentMethod { get; set; }

    public Receiver Receiver { get; set; }

    public string Seal { get; set; }

    public string Series { get; set; }

    public Decimal SubTotal { get; set; }

    public Taxes Taxes { get; set; }

    public Decimal Total { get; set; }

    public CFDType Type { get; set; }

    public string Version { get; set; } = "3.2";
  }
}
