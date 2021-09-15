
using DigitalData.Open.Common.Entities;
using System;
using System.Threading.Tasks;

namespace DigitalData.Open.Common.Extensions
{
  public interface IDocumentCreation
  {
    Func<DdocDocument, Task> TriggerAction { get; set; }
  }
}
