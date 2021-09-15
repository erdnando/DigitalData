// Decompiled with JetBrains decompiler
// Type: DigitalData.DDoc.Common.Entities.DDocFileResponse
// Assembly: DigitalData.DDoc.Common, Version=4.210.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 37D7F3E1-2CA5-48B0-AB22-FE81D67009C0
// Assembly location: C:\Users\herna\Downloads\ss\BradesCard_DEV\bin\DigitalData.DDoc.Common.dll

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
