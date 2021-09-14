// Decompiled with JetBrains decompiler
// Type: DigitalData.DDoc.Common.Api.DataAccess.IActivityLogDAL
// Assembly: DigitalData.DDoc.Common, Version=4.210.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 37D7F3E1-2CA5-48B0-AB22-FE81D67009C0
// Assembly location: C:\Users\herna\Downloads\ss\BradesCard_DEV\bin\DigitalData.DDoc.Common.dll

using DigitalData.DDoc.Common.Entities.Helpers;
using System.Threading.Tasks;

namespace DigitalData.DDoc.Common.Api.DataAccess
{
  public interface IActivityLogDAL : ICommonDAL
  {
    Task WriteLogEntry(DdocActionLogEntry entry);
  }
}
