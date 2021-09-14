// Decompiled with JetBrains decompiler
// Type: DigitalData.DDoc.Common.Api.DALHelpers
// Assembly: DigitalData.DDoc.Common, Version=4.210.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 37D7F3E1-2CA5-48B0-AB22-FE81D67009C0
// Assembly location: C:\Users\herna\Downloads\ss\BradesCard_DEV\bin\DigitalData.DDoc.Common.dll

using DigitalData.Common.Entities;
using DigitalData.DDoc.Common.Entities;
using SqlKata;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace DigitalData.DDoc.Common.Api
{
  public static class DALHelpers
  {
    public static Query BuildWhere(this Query query, List<DdocSearchFilterGroup> searchFilters)
    {
      foreach (DdocSearchFilterGroup searchFilter in searchFilters)
      {
        DdocSearchFilterGroup group = searchFilter;
        switch (group.GroupOperator)
        {
          case LogicOperator.And:
            query.Where((Func<Query, Query>) (q => DALHelpers.CompileClauses(q, group)));
            continue;
          case LogicOperator.Or:
            query.OrWhere((Func<Query, Query>) (q => DALHelpers.CompileClauses(q, group)));
            continue;
          default:
            continue;
        }
      }
      return query;
    }

    private static Query CompileClauses(Query q, DdocSearchFilterGroup group)
    {
      foreach (DdocSearchFilter clause in group.Clauses)
      {
        string column = clause.FieldId != 99999 ? string.Format("C{0}", (object) clause.FieldId) : "FECHA_CREACION";
        object[] clauseValue = DALHelpers.GetClauseValue(clause);
        switch (group.ClausesOperator)
        {
          case LogicOperator.And:
            switch (clause.Comparison)
            {
              case Comparison.Equals:
                q.Where(column, (object) clause.Value);
                continue;
              case Comparison.NotEquals:
                q.WhereNot(column, clauseValue[0]);
                continue;
              case Comparison.Like:
                q.WhereLike(column, clauseValue[0]);
                continue;
              case Comparison.NotLike:
                q.WhereNotLike(column, clauseValue[0]);
                continue;
              case Comparison.GreaterThan:
                q.Where(column, ">", clauseValue[0]);
                continue;
              case Comparison.GreaterOrEquals:
                q.Where(column, ">=", clauseValue[0]);
                continue;
              case Comparison.LessThan:
                q.Where(column, "<", clauseValue[0]);
                continue;
              case Comparison.LessOrEquals:
                q.Where(column, "<=", clauseValue[0]);
                continue;
              case Comparison.In:
                q.WhereIn<string>(column, (IEnumerable<string>) (string[]) clauseValue[0]);
                continue;
              case Comparison.NotIn:
                q.WhereNotIn<string>(column, (IEnumerable<string>) (string[]) clauseValue[0]);
                continue;
              case Comparison.Between:
                q.WhereBetween<object>(column, clauseValue[0], clauseValue[1]);
                continue;
              case Comparison.NotBetween:
                q.WhereNotBetween<object>(column, clauseValue[0], clauseValue[1]);
                continue;
              case Comparison.Contains:
                q.WhereContains(column, clauseValue[0]);
                continue;
              case Comparison.NotContains:
                q.WhereNotContains(column, clauseValue[0]);
                continue;
              case Comparison.StartsWith:
                q.WhereStarts(column, clauseValue[0]);
                continue;
              case Comparison.NotStartsWith:
                q.WhereNotStarts(column, clauseValue[0]);
                continue;
              case Comparison.EndsWith:
                q.WhereEnds(column, clauseValue[0]);
                continue;
              case Comparison.NotEndsWith:
                q.WhereNotEnds(column, clauseValue[0]);
                continue;
              case Comparison.IsTrue:
                q.WhereTrue(column);
                continue;
              case Comparison.IsFalse:
                q.WhereFalse(column);
                continue;
              default:
                continue;
            }
          case LogicOperator.Or:
            switch (clause.Comparison)
            {
              case Comparison.Equals:
                q.OrWhere(column, clauseValue[0]);
                continue;
              case Comparison.NotEquals:
                q.OrWhereNot(column, clauseValue[0]);
                continue;
              case Comparison.Like:
                q.OrWhereLike(column, clauseValue[0]);
                continue;
              case Comparison.NotLike:
                q.OrWhereNotLike(column, clauseValue[0]);
                continue;
              case Comparison.GreaterThan:
                q.OrWhere(column, ">", clauseValue[0]);
                continue;
              case Comparison.GreaterOrEquals:
                q.OrWhere(column, ">=", clauseValue[0]);
                continue;
              case Comparison.LessThan:
                q.OrWhere(column, "<", clauseValue[0]);
                continue;
              case Comparison.LessOrEquals:
                q.OrWhere(column, "<=", clauseValue[0]);
                continue;
              case Comparison.In:
                q.OrWhereIn<string>(column, (IEnumerable<string>) (string[]) clauseValue[0]);
                continue;
              case Comparison.NotIn:
                q.OrWhereNotIn<string>(column, (IEnumerable<string>) (string[]) clauseValue[0]);
                continue;
              case Comparison.Between:
                q.OrWhereBetween<object>(column, clauseValue[0], clauseValue[1]);
                continue;
              case Comparison.NotBetween:
                q.OrWhereNotBetween<object>(column, clauseValue[0], clauseValue[1]);
                continue;
              case Comparison.Contains:
                q.OrWhereContains(column, clauseValue[0]);
                continue;
              case Comparison.NotContains:
                q.OrWhereNotContains(column, clauseValue[0]);
                continue;
              case Comparison.StartsWith:
                q.OrWhereStarts(column, clauseValue[0]);
                continue;
              case Comparison.NotStartsWith:
                q.OrWhereNotStarts(column, clauseValue[0]);
                continue;
              case Comparison.EndsWith:
                q.OrWhereEnds(column, clauseValue[0]);
                continue;
              case Comparison.NotEndsWith:
                q.OrWhereNotEnds(column, clauseValue[0]);
                continue;
              case Comparison.IsTrue:
                q.OrWhereTrue(column);
                continue;
              case Comparison.IsFalse:
                q.OrWhereFalse(column);
                continue;
              default:
                continue;
            }
          default:
            continue;
        }
      }
      return q;
    }

    private static object[] GetClauseValue(DdocSearchFilter clause)
    {
      object[] objArray = new object[2];
      switch (clause.Type)
      {
        case FieldType.Number:
        case FieldType.Money:
          objArray[0] = (object) Decimal.Parse(clause.Value);
          if (!string.IsNullOrEmpty(clause.Value2))
          {
            objArray[1] = (object) Decimal.Parse(clause.Value2);
            break;
          }
          break;
        case FieldType.Date:
          objArray[0] = (object) DateTime.ParseExact(clause.Value + " 00:00:00", "dd/MM/yyyy HH:mm:ss", (IFormatProvider) CultureInfo.InvariantCulture);
          if (!string.IsNullOrEmpty(clause.Value2))
          {
            objArray[1] = (object) DateTime.ParseExact(clause.Value2 + " 23:59:59", "dd/MM/yyyy HH:mm:ss", (IFormatProvider) CultureInfo.InvariantCulture);
            break;
          }
          break;
        default:
          objArray[0] = (object) clause.Value;
          break;
      }
      switch (clause.Comparison)
      {
        case Comparison.In:
        case Comparison.NotIn:
          objArray[0] = (object) ((string) objArray[0]).Split(',');
          break;
        default:
          if (clause.Type == FieldType.Date)
          {
            DateTime dateTime1 = (DateTime) objArray[0];
            DateTime date = dateTime1.Date;
            dateTime1 = date.AddHours(23.0);
            dateTime1 = dateTime1.AddMinutes(59.0);
            DateTime dateTime2 = dateTime1.AddSeconds(59.0);
            switch (clause.Comparison)
            {
              case Comparison.Equals:
                objArray[0] = (object) date;
                objArray[1] = (object) dateTime2;
                clause.Comparison = Comparison.Between;
                break;
              case Comparison.NotEquals:
                objArray[0] = (object) date;
                objArray[1] = (object) dateTime2;
                clause.Comparison = Comparison.NotBetween;
                break;
              case Comparison.GreaterThan:
              case Comparison.LessOrEquals:
                objArray[0] = (object) dateTime2;
                break;
              case Comparison.GreaterOrEquals:
              case Comparison.LessThan:
                objArray[0] = (object) date;
                break;
            }
          }
          else
          {
            objArray[0] = objArray[0];
            break;
          }
          break;
      }
      return objArray;
    }
  }
}
