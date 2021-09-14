// Decompiled with JetBrains decompiler
// Type: DigitalData.Common.WebUtils.AspNet4.ChunkedHandler
// Assembly: DigitalData.Common.WebUtils.AspNet4, Version=6.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 74D4764F-0348-418D-8077-4ECEFD368363
// Assembly location: C:\Users\herna\Downloads\ss\BradesCard_DEV\bin\DigitalData.Common.WebUtils.AspNet4.dll

using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace DigitalData.Common.WebUtils.AspNet4
{
  public class ChunkedHandler : DelegatingHandler
  {
    protected override async Task<HttpResponseMessage> SendAsync(
      HttpRequestMessage request,
      CancellationToken cancellationToken)
    {
      HttpResponseMessage response = await base.SendAsync(request, cancellationToken);
      response.Headers.TransferEncodingChunked = new bool?(true);
      HttpResponseMessage httpResponseMessage = response;
      response = (HttpResponseMessage) null;
      return httpResponseMessage;
    }
  }
}
