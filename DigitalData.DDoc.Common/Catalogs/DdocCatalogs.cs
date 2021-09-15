
using DigitalData.Open.Common.Entities;
using System.Collections.Generic;
using System.Linq;

namespace DigitalData.Open.Common.Catalogs
{
  public class DdocCatalogs
  {
    public static Dictionary<int, string> GetFilterOperatorList() => new Dictionary<int, string>()
    {
      [0] = "Y",
      [1] = "O"
    };

    public static List<KeyValuePair<int, string>> GetFilterComparisonList(
      FieldType fieldType)
    {
      List<KeyValuePair<int, string>> keyValuePairList = (List<KeyValuePair<int, string>>) null;
      switch (fieldType)
      {
        case FieldType.Text:
          keyValuePairList = DdocCatalogs.GetTextComparisonList().ToList<KeyValuePair<int, string>>();
          break;
        case FieldType.Number:
        case FieldType.Money:
          keyValuePairList = DdocCatalogs.GetNumberComparisonList().ToList<KeyValuePair<int, string>>();
          break;
        case FieldType.Date:
          keyValuePairList = DdocCatalogs.GetDateComparisonList().ToList<KeyValuePair<int, string>>();
          break;
        case FieldType.Boolean:
          keyValuePairList = DdocCatalogs.GetBooleanComparisonList().ToList<KeyValuePair<int, string>>();
          break;
      }
      return keyValuePairList;
    }

    private static Dictionary<int, string> GetNumberComparisonList() => new Dictionary<int, string>()
    {
      [1] = "Es igual",
      [2] = "No es igual",
      [5] = "Es mayor a",
      [6] = "Es mayor o igual a",
      [7] = "Es menor a",
      [8] = "Es menor o igual a",
      [11] = "Está entre",
      [12] = "No está entre"
    };

    private static Dictionary<int, string> GetTextComparisonList() => new Dictionary<int, string>()
    {
      [1] = "Es",
      [2] = "No es",
      [13] = "Contiene",
      [14] = "No contiene",
      [15] = "Inicia con",
      [16] = "No inicia con",
      [17] = "Termina con",
      [18] = "No termina con"
    };

    private static Dictionary<int, string> GetDateComparisonList() => new Dictionary<int, string>()
    {
      [1] = "De la fecha",
      [2] = "No de la fecha",
      [5] = "Posterior a",
      [6] = "De fecha o posterior a",
      [7] = "Anterior a",
      [8] = "De fecha o anterior a",
      [11] = "Esta entre",
      [12] = "No esta entre"
    };

    private static Dictionary<int, string> GetBooleanComparisonList() => new Dictionary<int, string>()
    {
      [21] = "Verdadero",
      [22] = "Falso"
    };

    public static Dictionary<int, string> GetSearchTypeList() => new Dictionary<int, string>()
    {
      [0] = "Todas las palabras",
      [1] = "Cualquier palabra",
      [2] = "Frase exacta"
    };
  }
}
