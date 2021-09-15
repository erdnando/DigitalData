
using System;
using System.Runtime.Serialization;

namespace DigitalData.Open.Common.Entities.Config
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
