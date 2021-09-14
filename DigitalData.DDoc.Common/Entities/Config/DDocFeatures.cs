// Decompiled with JetBrains decompiler
// Type: DigitalData.DDoc.Common.Entities.Config.DdocFeatures
// Assembly: DigitalData.DDoc.Common, Version=4.210.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 37D7F3E1-2CA5-48B0-AB22-FE81D67009C0
// Assembly location: C:\Users\herna\Downloads\ss\BradesCard_DEV\bin\DigitalData.DDoc.Common.dll

using System;
using System.Runtime.Serialization;

namespace DigitalData.DDoc.Common.Entities.Config
{
  [DataContract]
  [Serializable]
  public class DdocFeatures
  {
    [DataMember]
    public string LicenseStatus { get; set; }

    [DataMember]
    public bool MobileUi { get; set; }

    [DataMember]
    public bool ReportsEnabled { get; set; }

    [DataMember]
    public bool TextIndexing { get; set; }

    [DataMember]
    public bool ValidLicense { get; set; }

    [DataMember]
    public bool WebTwain { get; set; }

    [DataMember]
    public bool WebUi { get; set; }

    [DataMember]
    public int MaxUsers { get; set; }

    [DataMember]
    public bool Ocr { get; set; }

    public void SetValues(DdocFeatures features)
    {
      this.LicenseStatus = features.LicenseStatus;
      this.MobileUi = features.MobileUi;
      this.ReportsEnabled = features.ReportsEnabled;
      this.Ocr = features.Ocr;
      this.TextIndexing = features.TextIndexing;
      this.ValidLicense = features.ValidLicense;
      this.WebTwain = features.WebTwain;
      this.WebUi = features.WebUi;
      this.MaxUsers = features.MaxUsers;
    }
  }
}
