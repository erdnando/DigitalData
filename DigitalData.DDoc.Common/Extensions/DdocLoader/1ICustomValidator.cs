
using DigitalData.Open.Common.Entities;
using System;

namespace DigitalData.Open.Common.Extensions.DdocLoader
{
  public interface ICustomValidator
  {
    Action<SourceData> Validate { get; set; }
  }
}
