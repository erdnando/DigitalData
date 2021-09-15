
using DigitalData.Open.Common.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DigitalData.Open.Common.Extensions
{
  public interface ICustomSearch
  {
    Func<DdocSearchParameters, Task<List<DdocCollection>>> Search { get; set; }
  }
}
