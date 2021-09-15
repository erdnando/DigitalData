// Decompiled with JetBrains decompiler
// Type: DigitalData.DDoc.Common.Entities.UI.Issuer
// Assembly: DigitalData.DDoc.Common, Version=4.210.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 37D7F3E1-2CA5-48B0-AB22-FE81D67009C0
// Assembly location: C:\Users\herna\Downloads\ss\BradesCard_DEV\bin\DigitalData.DDoc.Common.dll

namespace DigitalData.Open.Common.Entities.UI
{
  public class Issuer
  {
    public FiscalLocation FiscalLocation { get; set; }

    public IssuerFiscalRegime[] FiscalRegimes { get; set; }

    public Location IssuingPlace { get; set; }

    public string Name { get; set; }

    public string Rfc { get; set; }
  }
}
