// Decompiled with JetBrains decompiler

using System.Data;

namespace DigitalData.Open.Common.Api.DataAccess
{
  public interface ICommonDAL
  {
    IDbTransaction Transaction { get; set; }
  }
}
