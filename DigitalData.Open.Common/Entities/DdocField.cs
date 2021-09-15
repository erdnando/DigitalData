
using DigitalData.Open.Common.Entities.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

namespace DigitalData.Open.Common.Entities
{
  [DataContract]
  public class DdocField : IEquatable<DdocField>
  {
    private List<string> allowedValues;
    private string allowedValuesString;
    private string typeString;

    [DataMember]
    [DisplayName("Valores permitidos")]
    public List<string> AllowedValues
    {
      get => this.allowedValues;
      set => this.allowedValues = value;
    }

    public string AllowedValuesString
    {
      get => this.allowedValuesString;
      set
      {
        if (!string.IsNullOrEmpty(value))
          this.allowedValues = ((IEnumerable<string>) value.Split(',')).ToList<string>();
        this.allowedValuesString = value;
      }
    }

    [DataMember]
    public bool ForceStructChange { get; set; }

    [DataMember]
    [DisplayName("Incluir en búsqueda global")]
    public bool IncludeInGlobalSearch { get; set; } = true;

    [DataMember]
    public string Format { get; set; }

    [DataMember]
    [DisplayName("Oculto")]
    public bool Hidden { get; set; }

    [DataMember]
    public int Id { get; set; }

    [DataMember]
    [DisplayName("Heredable")]
    public bool Inheritable { get; set; }

    [DataMember]
    [DisplayName("Máscara de entrada: ")]
    public string InMask { get; set; }

    [DataMember]
    public bool IsNew { get; set; }

    [DataMember]
    [DisplayName("Nombre: ")]
    public string Name { get; set; }

    [DataMember]
    [DisplayName("Opcional")]
    public bool Nullable { get; set; }

    [DataMember]
    [DisplayName("Máscara de salida: ")]
    public string OutMask { get; set; }

    [DataMember]
    public int Sequence { get; set; }

    private FieldType type { get; set; }

    [DataMember]
    public FieldType Type
    {
      get
      {
        string lowerInvariant = this.TypeString.Trim().ToLowerInvariant();
        if (lowerInvariant == "bit")
          return FieldType.Boolean;
        if (lowerInvariant.Contains("char"))
          return FieldType.Text;
        if (lowerInvariant == "money" || lowerInvariant == "smallmoney")
          return FieldType.Money;
        if (((IEnumerable<string>) new string[3]
        {
          "int",
          "bigint",
          "decimal"
        }).Contains<string>(lowerInvariant))
          return FieldType.Number;
        return !lowerInvariant.Contains("datetime") ? FieldType.Text : FieldType.Date;
      }
      set => this.type = value;
    }

    [DataMember]
    [DisplayName("Longitud: ")]
    public int? TypeLength { get; set; }

    [DataMember]
    [DisplayName("Precisión: ")]
    public int? TypePrecision { get; set; }

    [DataMember]
    [DisplayName("Tipo: ")]
    public string TypeString
    {
      get => this.typeString;
      set
      {
        if (value.Contains<char>('('))
        {
          if (value.Contains<char>(','))
          {
            string str = value.Substring(value.IndexOf('('));
            this.TypeLength = new int?(int.Parse(str.Substring(1, str.IndexOf(',') - 1)));
            this.TypePrecision = new int?(int.Parse(str.Substring(str.IndexOf(',') + 1, str.IndexOf(')') - (str.IndexOf(',') + 1))));
          }
          else
            this.TypeLength = new int?(int.Parse(Regex.Match(value, "\\((\\d+)\\)").Groups[1].Value));
          this.typeString = value.Split('(')[0];
        }
        this.typeString = value;
      }
    }

    [DataMember]
    [DisplayName("Único")]
    public bool Unique { get; set; }

    [DataMember]
    public string Value { get; set; }

    [DataMember]
    [DisplayName("Ligar con Colección Padre")]
    public bool CreateRule { get; set; }

    [DataMember]
    [DisplayName("Calculado")]
    public bool Computed { get; set; }

    public object GetValue()
    {
      object obj = (object) null;
      switch (this.Type)
      {
        case FieldType.Text:
          obj = (object) this.Value;
          break;
        case FieldType.Number:
        case FieldType.Money:
          obj = (object) Decimal.Parse(string.IsNullOrEmpty(this.Value) ? "0" : this.Value);
          break;
        case FieldType.Date:
          if (!string.IsNullOrEmpty(this.Value))
          {
            if (!string.IsNullOrEmpty(this.Format))
            {
              DateTime result;
              if (!DateTime.TryParseExact(this.Value.TrimEnd('A', 'P', 'M', 'a', 'p', 'm', '.', ' '), this.Format, (IFormatProvider) CultureInfo.InvariantCulture, DateTimeStyles.None, out result))
                throw new DdocException("No se pudo convertir el valor de Fecha del campo " + this.Name);
              obj = (object) result.ToString("s");
              break;
            }
            obj = (object) DateTime.Parse(this.Value).ToString("s");
            break;
          }
          break;
        case FieldType.Boolean:
          obj = (object) bool.Parse(this.Value ?? "False");
          break;
      }
      return obj;
    }

    public bool Equals(DdocField other) => this.Id == other.Id;

    public override bool Equals(object obj) => !(obj.GetType() != typeof (DdocField)) && this.Equals((DdocField) obj);

    public override int GetHashCode() => this.Id;
  }
}
