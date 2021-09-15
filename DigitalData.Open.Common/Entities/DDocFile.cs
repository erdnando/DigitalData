
using System;
using System.IO;

namespace DigitalData.Open.Common.Entities
{
  public class DDocFileResponse : IDisposable
  {
    public string ContentType { get; set; }

    public Stream FileByteStream { get; set; }

    public string Filename { get; set; }

    public void Dispose()
    {
      if (this.FileByteStream == null)
        return;
      this.FileByteStream.Close();
      this.FileByteStream = (Stream) null;
    }
  }
}
