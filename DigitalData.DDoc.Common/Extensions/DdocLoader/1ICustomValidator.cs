// Decompiled with JetBrains decompiler
// Type: DigitalData.DDoc.Common.Extensions.DdocLoader.ICustomValidator
// Assembly: DigitalData.DDoc.Common, Version=4.210.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 37D7F3E1-2CA5-48B0-AB22-FE81D67009C0
// Assembly location: C:\Users\herna\Downloads\ss\BradesCard_DEV\bin\DigitalData.DDoc.Common.dll

using DigitalData.Open.Common.Entities;
using System;

namespace DigitalData.Open.Common.Extensions.DdocLoader
{
  public interface ICustomValidator
  {
    Action<SourceData> Validate { get; set; }
  }
}
