
using System.Collections.Generic;

namespace DigitalData.Open.Common.Entities
{
  public class DdocChildEntityComparer : EqualityComparer<DdocChildEntity>
  {
    public override bool Equals(DdocChildEntity x, DdocChildEntity y) => x.Id == y.Id;

    public override int GetHashCode(DdocChildEntity obj) => this.GetHashCode();
  }
}
