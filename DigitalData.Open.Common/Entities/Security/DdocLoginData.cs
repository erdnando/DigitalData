
using System;

namespace DigitalData.Open.Common.Entities.Security
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
