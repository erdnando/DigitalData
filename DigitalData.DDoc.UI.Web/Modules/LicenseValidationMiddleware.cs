
using DigitalData.Common.Cryptography.AES;
using DigitalData.Common.Entities;
using DigitalData.Common.IoC;
using DigitalData.Open.Common.Entities;
using DigitalData.Open.Common.Entities.Api;
using DigitalData.Open.Common.Entities.Config;
using DigitalData.Open.Common.Entities.Helpers;
using Microsoft.Extensions.Options;
using Microsoft.Owin;
using System;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace DigitalData.DDoc.UI.Web.Modules
{
  public class LicenseValidationMiddleware : OwinMiddleware
  {
    public LicenseValidationMiddleware(OwinMiddleware next, DdocFeatures features)
      : base(next)
    {
      this.DdocFeatures = features;
    }

    public DdocFeatures DdocFeatures { get; }

    public override async Task Invoke(IOwinContext context)
    {
      LicenseValidationMiddleware validationMiddleware = this;
      DdocFeatures features = validationMiddleware.ValidateDdocLicense(DdocInstanceType.Web, Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, "DDocLic.xli"));
      validationMiddleware.DdocFeatures.SetValues(features);
      if (!validationMiddleware.DdocFeatures.ValidLicense)
      {
        IOwinResponse owinResponse = context.Response;
        Stream owinResponseStream = owinResponse.Body;
        owinResponse.Body = (Stream) new MemoryStream();
        Stream customResponseStream;
        if (LicenseValidationMiddleware.IsAjaxRequest(context.Request))
        {
          context.Response.StatusCode = 403;
          customResponseStream = await new StringContent(JsonSerializer.Serialize<ApiResponse>(new ApiResponse()
          {
            Success = false,
            Message = validationMiddleware.DdocFeatures.LicenseStatus
          })).ReadAsStreamAsync();
          await customResponseStream.CopyToAsync(owinResponseStream);
          owinResponse.ContentType = "application/json";
          owinResponse.ContentLength = new long?(customResponseStream.Length);
          owinResponse.Body = owinResponseStream;
          customResponseStream = (Stream) null;
        }
        else
        {
          string message = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, "Views/Shared/LicenseError.html"));
          message = message.Replace("{$message}", validationMiddleware.DdocFeatures.LicenseStatus);
          await context.Response.WriteAsync(message);
          customResponseStream = await new StringContent(message).ReadAsStreamAsync();
          await customResponseStream.CopyToAsync(owinResponseStream);
          owinResponse.ContentType = "text/plain";
          owinResponse.ContentLength = new long?(customResponseStream.Length);
          owinResponse.Body = owinResponseStream;
          message = (string) null;
          customResponseStream = (Stream) null;
        }
        owinResponse = (IOwinResponse) null;
        owinResponseStream = (Stream) null;
      }
      else
        await validationMiddleware.Next.Invoke(context);
    }

    private static bool IsAjaxRequest(IOwinRequest request)
    {
      IReadableStringCollection query = request.Query;
      if (query != null && query["X-Requested-With"] == "XMLHttpRequest")
        return true;
      IHeaderDictionary headers = request.Headers;
      return headers != null && headers["X-Requested-With"] == "XMLHttpRequest";
    }

    public DdocFeatures ValidateDdocLicense(
      DdocInstanceType instanceType,
      string licenseFile)
    {
      DiDaSettings diDaSettings = IoCContainer.GetService<IOptions<DiDaSettings>>().Value;
      DdocFeatures ddocFeatures = new DdocFeatures();
      try
      {
        DbSettings connectionSettings = diDaSettings.GetConnectionSettings("DDocConnection");
        string setting = diDaSettings.GetSetting<string>("AppKey");
        if (!AesEncryption.DecryptText(setting, "D1d@*2019!").Equals("DDOC_ID_" + connectionSettings.DataSource + "_" + connectionSettings.Database))
          throw new DdocException("El archivo de licencia no es válido para esta instalación");
        string inputUri = licenseFile;
        DDocLicense ddocLicense;
        using (XmlReader xmlReader = XmlReader.Create(inputUri, new XmlReaderSettings()
        {
          ConformanceLevel = ConformanceLevel.Fragment
        }))
          ddocLicense = (DDocLicense) new XmlSerializer(typeof (DDocLicense)).Deserialize(xmlReader);
        if (ddocLicense.MaxUsers == null)
          throw new DdocException("El número de licencias de conexión no es válido");
        string s;
        try
        {
          s = AesEncryption.DecryptText(ddocLicense.MaxUsers, setting);
        }
        catch
        {
          throw new DdocException("El valor del campo de usuarios no es válido");
        }
        ddocFeatures.MaxUsers = int.Parse(s);
        string cypherText = string.Empty;
        switch (instanceType)
        {
          case DdocInstanceType.Web:
            cypherText = ddocLicense.WebDdoc ?? throw new DdocException("No existe licencia para DDOC WEB");
            ddocFeatures.WebUi = true;
            break;
          case DdocInstanceType.Mobile:
            cypherText = ddocLicense.MobileDdoc ?? throw new DdocException("No existe licencia para DDOC Mobile");
            ddocFeatures.MobileUi = true;
            break;
        }
        if (ddocLicense.TextSearch != null)
          ddocFeatures.TextIndexing = true;
        if (ddocLicense.WebTwain != null)
          ddocFeatures.WebTwain = true;
        if (ddocLicense.ReportsEnabled != null)
          ddocFeatures.ReportsEnabled = true;
        long num1;
        try
        {
          num1 = long.Parse(AesEncryption.DecryptText(cypherText, setting));
        }
        catch
        {
          throw new DdocException("El valor del campo de licencia dDoc no es válido");
        }
        long num2 = num1;
        DateTime dateTime = new DateTime(2000, 1, 1, 0, 0, 0);
        long ticks1 = dateTime.Ticks;
        if (num2 != ticks1)
        {
          long num3 = num1;
          dateTime = DateTime.Now;
          long ticks2 = dateTime.Ticks;
          if (num3 < ticks2)
            throw new DdocException("Licencia expirada. Contacte a " + (diDaSettings.AppConfig != null ? diDaSettings.GetConfig<string>("CompanyName") ?? "Digital Data" : "Digital Data") + " para renovar su licencia.");
          dateTime = DateTime.Now;
          dateTime = dateTime.AddDays(30.0);
          if (dateTime.Ticks > num1)
            ddocFeatures.LicenseStatus = string.Format("Restan días {0} antes de que la licencia expire", (object) TimeSpan.FromTicks(num1).Days);
        }
        ddocFeatures.ValidLicense = true;
        if (string.IsNullOrEmpty(ddocFeatures.LicenseStatus))
          ddocFeatures.LicenseStatus = "OK";
      }
      catch (DdocException ex)
      {
        ddocFeatures.LicenseStatus = "Error de licencia: " + ex.Message;
      }
      catch (Exception ex)
      {
        ddocFeatures.LicenseStatus = "No fue posible validar la licencia debido al error: " + ex.Message;
      }
      return ddocFeatures;
    }
  }
}
