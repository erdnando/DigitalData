// Decompiled with JetBrains decompiler
// Type: DigitalData.DDoc.Common.Helpers
// Assembly: DigitalData.DDoc.Common, Version=4.210.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 37D7F3E1-2CA5-48B0-AB22-FE81D67009C0
// Assembly location: C:\Users\herna\Downloads\ss\BradesCard_DEV\bin\DigitalData.DDoc.Common.dll

using DigitalData.DDoc.Common.Entities;
using System;
using System.Collections.Generic;

namespace DigitalData.DDoc.Common
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
