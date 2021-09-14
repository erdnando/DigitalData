// Decompiled with JetBrains decompiler
// Type: DigitalData.DDoc.UI.Web.Modules.LicenseValidator
// Assembly: DigitalData.DDoc.UI.Web, Version=4.210.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1482FEFA-CA2F-44DF-981C-51EA2DF7CBB5
// Assembly location: C:\Users\herna\Downloads\ss\BradesCard_DEV\bin\DigitalData.DDoc.UI.Web.dll

using DigitalData.Common.Cryptography.AES;
using DigitalData.Common.Entities;
using DigitalData.Common.IoC;
using DigitalData.Common.WebUtils.AspNet4;
using DigitalData.DDoc.Common.Entities;
using DigitalData.DDoc.Common.Entities.Config;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml;
using System.Xml.Serialization;

namespace DigitalData.DDoc.UI.Web.Modules
{
  public class LicenseValidator : IHttpModule
  {
    public void Init(HttpApplication context) => context.BeginRequest += (EventHandler) ((o, args) =>
    {
      DdocFeatures features = LicenseValidator.ValidateDdocLicense(DdocInstanceType.Web, Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, "DDocLic.xli"));

        features.ValidLicense = true;
        if (!features.ValidLicense)
          {
            context.Response.Clear();
            if (context.Request.IsAjaxRequest())
              context.Response.Write("Error de licencia: " + features.LicenseStatus);
            else
              context.Response.Write(File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, "Views/Shared/LicenseError.html")).Replace("{$message}", features.LicenseStatus));
            context.Response.End();
          }

      IoCContainer.GetService<DdocFeatures>().SetValues(features);
    });

    public void Dispose()
    {
    }

    public static DdocFeatures ValidateDdocLicense(
      DdocInstanceType instanceType,
      string licenseFile)
    {
      DiDaSettings diDaSettings = IoCContainer.GetService<IOptions<DiDaSettings>>().Value;
      DdocFeatures ddocFeatures = new DdocFeatures();
      try
      {
        JObject jobject = JObject.Load((JsonReader) new JsonTextReader((TextReader) File.OpenText(Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, "config.json"))));
        string str1 = jobject["DbConnections"].ToArray<JToken>()[0][(object) "DataSource"].Value<string>();
        string str2 = jobject["DbConnections"].ToArray<JToken>()[0][(object) "Database"].Value<string>();
        string str3 = jobject["AppSettings"][(object) "AppKey"].Value<string>();
        //if (!AesEncryption.DecryptText(str3, "D1d@*2019!").Equals("DDOC_ID_" + str1 + "_" + str2))
        //  throw new ApplicationException("El archivo de licencia no es válido para esta instalación");
        string inputUri = licenseFile;
        DDocLicense ddocLicense;
        using (XmlReader xmlReader = XmlReader.Create(inputUri, new XmlReaderSettings()
        {
          ConformanceLevel = ConformanceLevel.Fragment
        }))
          ddocLicense = (DDocLicense) new XmlSerializer(typeof (DDocLicense)).Deserialize(xmlReader);
        if (ddocLicense.MaxUsers == null)
          throw new ApplicationException("El número de licencias de conexión no es válido");
        string s;
        try
        {
          s = AesEncryption.DecryptText(ddocLicense.MaxUsers, str3);
        }
        catch
        {
          throw new ApplicationException("El valor del campo de usuarios no es válido");
        }
        ddocFeatures.MaxUsers = int.Parse(s);
        string cypherText = string.Empty;
        switch (instanceType)
        {
          case DdocInstanceType.Web:
            cypherText = ddocLicense.WebDdoc ?? throw new ApplicationException("No existe licencia para DDOC WEB");
            ddocFeatures.WebUi = true;
            break;
          case DdocInstanceType.Mobile:
            cypherText = ddocLicense.MobileDdoc ?? throw new ApplicationException("No existe licencia para DDOC Mobile");
            ddocFeatures.MobileUi = true;
            break;
        }
        if (ddocLicense.Ocr != null)
          ddocFeatures.Ocr = true;
        if (ddocLicense.TextSearch != null)
          ddocFeatures.TextIndexing = true;
        if (ddocLicense.WebTwain != null)
          ddocFeatures.WebTwain = true;
        if (ddocLicense.ReportsEnabled != null)
          ddocFeatures.ReportsEnabled = true;
        long num;
        try
        {
          num = long.Parse(AesEncryption.DecryptText(cypherText, str3));
        }
        catch
        {
          throw new ApplicationException("El valor del campo de licencia dDoc no es válido");
        }
        if (num != new DateTime(2000, 1, 1, 0, 0, 0).Ticks)
        {
          if (num < DateTime.Now.Ticks)
            throw new ApplicationException("Licencia expirada. Contacte a " + (diDaSettings.AppConfig != null ? diDaSettings.GetConfig<string>("CompanyName") ?? "Digital Data" : "Digital Data") + " para renovar su licencia.");
          DateTime dateTime = DateTime.Now;
          dateTime = dateTime.AddDays(30.0);
          if (dateTime.Ticks > num)
            ddocFeatures.LicenseStatus = string.Format("Restan días {0} antes de que la licencia expire", (object) TimeSpan.FromTicks(num).Days);
        }
        ddocFeatures.ValidLicense = true;
        if (string.IsNullOrEmpty(ddocFeatures.LicenseStatus))
          ddocFeatures.LicenseStatus = "OK";
      }
      catch (ApplicationException ex)
      {
        ddocFeatures.LicenseStatus = ex.Message;
      }
      catch (Exception ex)
      {
        ddocFeatures.LicenseStatus = "No fue posible validar la licencia debido al error: " + ex.Message;
      }
            ddocFeatures.MaxUsers = 100;
           // ddocFeatures.Ocr = true;
            
      return ddocFeatures;
    }
  }
}
