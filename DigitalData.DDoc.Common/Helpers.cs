

using DigitalData.Open.Common.Entities;
using System;
using System.Collections.Generic;

namespace DigitalData.Open.Common
{
  public static class Helpers
  {
    public static string GetValue(
      this IReadOnlyDictionary<string, object> itemData,
      int fieldId,
      FieldType type,
      string outputMask = null)
    {
      object obj = itemData[string.Format("C{0}", (object) fieldId)];
      if (obj == null)
        return string.Empty;
      return type != FieldType.Date ? obj.ToString() : ((DateTime) obj).ToString(outputMask);
    }
  }
}
