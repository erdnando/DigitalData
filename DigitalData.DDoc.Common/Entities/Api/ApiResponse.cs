// Decompiled with JetBrains decompiler
// Type: DigitalData.DDoc.Common.Entities.Api.ApiResponse
// Assembly: DigitalData.DDoc.Common, Version=4.210.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 37D7F3E1-2CA5-48B0-AB22-FE81D67009C0
// Assembly location: C:\Users\herna\Downloads\ss\BradesCard_DEV\bin\DigitalData.DDoc.Common.dll

using System;

namespace DigitalData.Open.Common.Entities.Api
{
  public class ApiResponse
  {
    public ApiResponse() => this.Success = true;

    public ApiResponse(Exception ex)
    {
      this.Success = false;
      this.Message = ex.Message;
      if (!ex.Data.Contains((object) nameof (ErrorId)))
        return;
      this.ErrorId = ex.Data[(object) nameof (ErrorId)].ToString();
    }

    public ApiResponse(int itemId)
      : this()
    {
      this.ItemId = new int?(itemId);
    }

    public ApiResponse(string itemGid)
      : this()
    {
      this.ItemGid = itemGid;
    }

    public string ApiToken { get; set; }

    public bool Success { get; set; } = true;

    public string Message { get; set; }

    public string ItemGid { get; set; }

    public int? ItemId { get; set; }

    public int? Count { get; set; }

    public long? Total { get; set; }

    public string Text { get; set; }

    public bool? Exists { get; set; }

    public string Url { get; set; }

    public string ErrorId { get; set; }
  }
}
