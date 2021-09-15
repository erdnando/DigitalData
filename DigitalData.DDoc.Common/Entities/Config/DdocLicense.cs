// Decompiled with JetBrains decompiler
// Type: DigitalData.DDoc.Common.Entities.Config.DDocLicense
// Assembly: DigitalData.DDoc.Common, Version=4.210.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 37D7F3E1-2CA5-48B0-AB22-FE81D67009C0
// Assembly location: C:\Users\herna\Downloads\ss\BradesCard_DEV\bin\DigitalData.DDoc.Common.dll

using System.Xml.Serialization;

namespace DigitalData.Open.Common.Entities.Config
{
  [XmlRoot]
  public class DDocLicense
  {
    [XmlElement("customer")]
    public string Customer { get; set; }

    [XmlElement("gidredir")]
    public string Gidredir { get; set; }

    [XmlElement("maxusers")]
    public string MaxUsers { get; set; }

    [XmlElement("mobileddoc")]
    public string MobileDdoc { get; set; }

    [XmlElement("reports")]
    public string ReportsEnabled { get; set; }

    [XmlElement("ocr")]
    public string Ocr { get; set; }

    [XmlElement("textsearchddoc")]
    public string TextSearch { get; set; }

    [XmlElement("vtygwddoc")]
    public string Vtygwddoc { get; set; }

    [XmlElement("webddoc")]
    public string WebDdoc { get; set; }

    [XmlElement("webtwain")]
    public string WebTwain { get; set; }
  }
}
