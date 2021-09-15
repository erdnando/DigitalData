
using System.Collections.Generic;

namespace DigitalData.Open.Common.Entities.Helpers
{
  public class CollectionChangeRequest
  {
    public List<DdocField> NewFields { get; set; }

    public string CollectionId { get; set; }

    public List<FieldCorrespondence> FieldLinks { get; set; }
  }
}
