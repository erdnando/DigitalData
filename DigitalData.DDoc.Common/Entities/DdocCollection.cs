// Decompiled with JetBrains decompiler
// Type: DigitalData.DDoc.Common.Entities.DdocCollection
// Assembly: DigitalData.DDoc.Common, Version=4.210.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 37D7F3E1-2CA5-48B0-AB22-FE81D67009C0
// Assembly location: C:\Users\herna\Downloads\ss\BradesCard_DEV\bin\DigitalData.DDoc.Common.dll

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace DigitalData.Open.Common.Entities
{
  [DataContract]
  public class DdocCollection : DdocEntity
  {
    [DataMember]
    [DisplayName("Visor de CFDI: ")]
    public bool Cfdi { get; set; }

    [DataMember]
    [DisplayName("Descripción: ")]
    public string Description { get; set; }

    [DataMember]
    [DisplayName("Numero de campos: ")]
    public int FieldCount { get; set; }

    [DataMember]
    public List<DdocField> Fields { get; set; }

    [DataMember]
    [DisplayName("Plantilla de nombre para descarga: ")]
    public string FileDownloadTemplate { get; set; }

    [DataMember]
    public bool HasChildren { get; set; }

    [DataMember]
    public bool HasData { get; set; }

    [DataMember]
    public bool HasDataTable { get; set; }

    [DataMember]
    public bool IsNew { get; set; }

    [DataMember]
    [DisplayName("Ruta para el nuevo almacén: ")]
    public string NewWarehousePath { get; set; }

    [DataMember]
    [DisplayName("Campos Heredados de Colección Padre: ")]
    public List<int> ParentFieldIds { get; set; } = new List<int>();

    [DataMember]
    [DisplayName("Colección Padre: ")]
    public string ParentId { get; set; }

    [DataMember]
    [DisplayName("Persistencia: ")]
    public int Persistence { get; set; }

    [DataMember]
    public List<DdocRule> Rules { get; set; }

    [DataMember]
    public long TotalSearchResults { get; set; }

    [DataMember]
    public List<DdocChildEntity> SearchResults { get; set; } = new List<DdocChildEntity>();

    [DataMember]
    [DisplayName("Tipo de colección: ")]
    public CollectionType Type { get; set; }

    public char TypeChar
    {
      get
      {
        switch (this.Type)
        {
          case CollectionType.C:
            return 'C';
          case CollectionType.F:
            return 'F';
          case CollectionType.D:
            return 'D';
          default:
            return '0';
        }
      }
    }

    [DataMember]
    [DisplayName("Almacen: ")]
    public int? WarehouseId { get; set; }

    public class CollectionComparer : IEqualityComparer<DdocCollection>
    {
      public bool Equals(DdocCollection x, DdocCollection y) => throw new NotImplementedException();

      public int GetHashCode(DdocCollection obj) => this.GetHashCode();
    }
  }
}
