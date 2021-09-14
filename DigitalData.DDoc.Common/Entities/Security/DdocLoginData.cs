// Decompiled with JetBrains decompiler
// Type: DigitalData.DDoc.Common.Entities.Security.DdocLoginData
// Assembly: DigitalData.DDoc.Common, Version=4.210.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 37D7F3E1-2CA5-48B0-AB22-FE81D67009C0
// Assembly location: C:\Users\herna\Downloads\ss\BradesCard_DEV\bin\DigitalData.DDoc.Common.dll

using System;

namespace DigitalData.DDoc.Common.Entities.Security
{
  public class DdocLoginData
  {
    public bool Active;
    public DateTime CreationDate;
    public DateTime LastLogin;
    public string Password;
    public long Tries;
    public string Username;
  }
}
