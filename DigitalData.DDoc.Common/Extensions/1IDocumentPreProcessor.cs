
using DigitalData.Open.Common.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DigitalData.Open.Common.Extensions
{
  public interface IDocumentPreProcessor
  {
    Func<List<DdocDocument>, Task<List<DdocDocument>>> PreProcessDocuments { get; set; }
  }
}
