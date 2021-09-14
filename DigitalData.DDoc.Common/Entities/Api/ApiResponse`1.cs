// Decompiled with JetBrains decompiler
// Type: DigitalData.DDoc.Common.Entities.Api.ApiResponse`1
// Assembly: DigitalData.DDoc.Common, Version=4.210.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 37D7F3E1-2CA5-48B0-AB22-FE81D67009C0
// Assembly location: C:\Users\herna\Downloads\ss\BradesCard_DEV\bin\DigitalData.DDoc.Common.dll

namespace DigitalData.DDoc.Common.Entities.Api
{
  public class ApiResponse<T> : ApiResponse
  {
    public ApiResponse()
    {
    }

    public ApiResponse(T result) => this.Result = result;

    public ApiResponse(System.Collections.Generic.List<T> result)
    {
      this.List = result;
      this.Count = new int?(this.List.Count);
    }

    public T Result { get; set; }

    public System.Collections.Generic.List<T> List { get; set; }
  }
}
