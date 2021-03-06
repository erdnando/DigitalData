
using System.ComponentModel;
using System.Runtime.Serialization;

namespace DigitalData.Open.Common.Entities
{
  [DataContract]
  public class DdocRule
  {
    [DataMember]
    public char ChildColType { get; set; }

    [DataMember]
    [DisplayName("Campo Hijo: ")]
    public int? ChildField { get; set; }

    [DataMember]
    public string ChildFieldName { get; set; }

    [DataMember]
    [DisplayName("Colección Hija: ")]
    public string ChildId { get; set; }

    [DataMember]
    public string ChildName { get; set; }

    public CollectionType ChildType
    {
      get
      {
        if (this.ChildColType == 'R')
          return CollectionType.R;
        if (this.ChildColType == 'F')
          return CollectionType.F;
        return this.ChildColType == 'D' ? CollectionType.D : CollectionType.C;
      }
    }

    [DataMember]
    public bool Expand { get; set; }

    [DataMember]
    public int Id { get; set; }

    [DataMember]
    public bool IsNew { get; set; }

    public CollectionType ParentType
    {
      get
      {
        if (this.ParentColType == 'R')
          return CollectionType.R;
        if (this.ParentColType == 'F')
          return CollectionType.F;
        return this.ParentColType == 'D' ? CollectionType.D : CollectionType.C;
      }
    }

    [DataMember]
    [DisplayName("Campo Padre: ")]
    public int? ParentField { get; set; }

    [DataMember]
    public string ParentFieldName { get; set; }

    [DataMember]
    [DisplayName("Colección Padre: ")]
    public string ParentId { get; set; }

    [DataMember]
    public string ParentName { get; set; }

    [DataMember]
    public char ParentColType { get; set; }

    [DataMember]
    public int Sequence { get; set; }
  }
}
