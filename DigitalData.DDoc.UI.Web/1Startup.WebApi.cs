
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DigitalData.Open.UI.Web
{
  public class JsonFormatter : MediaTypeFormatter
  {
    private readonly JsonSerializerOptions options;

    public JsonFormatter()
    {
      this.options = new JsonSerializerOptions()
      {
        IgnoreNullValues = true,
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = (JsonNamingPolicy) null,
        WriteIndented = true
      };
      this.SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/json"));
      this.SupportedEncodings.Add((Encoding) new UTF8Encoding(false, true));
      this.SupportedEncodings.Add((Encoding) new UnicodeEncoding(false, true, true));
    }

    public override bool CanReadType(Type type)
    {
      if (type == (Type) null)
        throw new ArgumentNullException(nameof (type));
      return true;
    }

    public override bool CanWriteType(Type type)
    {
      if (type == (Type) null)
        throw new ArgumentNullException(nameof (type));
      return true;
    }

    public override Task<object> ReadFromStreamAsync(
      Type type,
      Stream readStream,
      HttpContent content,
      IFormatterLogger formatterLogger)
    {
      return JsonSerializer.DeserializeAsync(readStream, type, this.options).AsTask();
    }

    public override Task WriteToStreamAsync(
      Type type,
      object value,
      Stream writeStream,
      HttpContent content,
      TransportContext transportContext)
    {
      return JsonSerializer.SerializeAsync(writeStream, value, type, this.options);
    }
  }
}
