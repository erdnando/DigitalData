
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace DigitalData.Open.Common.Entities
{
  [DataContract]
  [KnownType(typeof (DdocFolder))]
  [KnownType(typeof (DdocDocument))]
  public class DdocChildEntity : DdocEntity
  {
    [DataMember]
    public string CollectionId { get; set; }

    [DataMember]
    public DateTime? CreationDate { get; set; }

    [DataMember]
    public List<DdocField> Data { get; set; }

    [DataMember]
    public string DataKey { get; set; }

    [DataMember]
    public DateTime? DeletionDate { get; set; }

    [DataMember]
    public bool IsNew { get; set; }

    public DdocField this[int key] => this.Data.SingleOrDefault<DdocField>((Func<DdocField, bool>) (field => field.Id == key));

    public DdocField this[string key] => this.Data.SingleOrDefault<DdocField>((Func<DdocField, bool>) (field => field.Name == key));

    [DataMember]
    public DateTime ModificationDate { get; set; }
  }
}
