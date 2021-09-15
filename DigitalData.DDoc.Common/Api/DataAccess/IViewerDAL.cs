// Decompiled with JetBrains decompiler
// Type: DigitalData.DDoc.Common.Api.DataAccess.IViewerDAL
// Assembly: DigitalData.DDoc.Common, Version=4.210.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 37D7F3E1-2CA5-48B0-AB22-FE81D67009C0
// Assembly location: C:\Users\herna\Downloads\ss\BradesCard_DEV\bin\DigitalData.DDoc.Common.dll

using System.Threading.Tasks;

namespace DigitalData.Open.Common.Api.DataAccess
{
  public interface IViewerDAL : ICommonDAL
  {
    Task<string> GetPageType(string pageId);

    Task<string> GetWarehousePath(string pageId);
  }
}
