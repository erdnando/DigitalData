// Decompiled with JetBrains decompiler
// Type: DigitalData.DDoc.Common.Entities.Security.DdocLoginConfig
// Assembly: DigitalData.DDoc.Common, Version=4.210.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 37D7F3E1-2CA5-48B0-AB22-FE81D67009C0
// Assembly location: C:\Users\herna\Downloads\ss\BradesCard_DEV\bin\DigitalData.DDoc.Common.dll

namespace DigitalData.Open.Common.Entities.Security
{
  public class DdocLoginConfig
  {
    public long InactivityDays { get; set; }

    public bool LoginExpirationEnabled { get; set; }

    public long LoginExpiryDays { get; set; }

    public long LoginWarningDays { get; set; }

    public long Tries { get; set; }
  }
}
