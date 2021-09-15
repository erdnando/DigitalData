// Decompiled with JetBrains decompiler
// Type: DigitalData.DDoc.Common.Entities.Security.UserSession
// Assembly: DigitalData.DDoc.Common, Version=4.210.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 37D7F3E1-2CA5-48B0-AB22-FE81D67009C0
// Assembly location: C:\Users\herna\Downloads\ss\BradesCard_DEV\bin\DigitalData.DDoc.Common.dll

using DigitalData.Common.Cryptography.Base64;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Claims;
using System.Text.Json;

namespace DigitalData.Open.Common.Entities.Security
{
  [DataContract]
  [Serializable]
  public class UserSession
  {
    [DataMember]
    public string DdocGroups { get; set; }

    [DataMember]
    public Permissions DdocToken { get; set; }

    [DataMember]
    public string Email { get; set; }

    [DataMember]
    public LoginError LoginError { get; set; }

    [DataMember]
    public bool LoginOk { get; set; }

    [DataMember]
    public string Name { get; set; }

    [DataMember]
    public List<string> Roles { get; set; }

    [DataMember]
    public int ServerId { get; set; }

    [DataMember]
    public string UserGroups { get; set; }

    [DataMember]
    public string Username { get; set; }

    [DataMember]
    public DateTime Expiration { get; set; }

    [DataMember]
    public string ApiToken { get; set; }

    [DataMember]
    public bool Valid => this.Expiration > DateTime.Now;

    public ClaimsIdentity ToIdentity()
    {
      List<Claim> claimList = new List<Claim>();
      claimList.Add(new Claim("DdocGroups", this.DdocGroups));
      claimList.Add(new Claim("DdocToken", this.DdocToken.ToString()));
      claimList.Add(new Claim("Email", this.Email ?? string.Empty));
      claimList.Add(new Claim("Name", this.Name));
      claimList.Add(new Claim("ServerId", this.ServerId.ToString()));
      claimList.Add(new Claim("UserGroups", this.UserGroups));
      claimList.Add(new Claim("Username", this.Username));
      claimList.Add(new Claim("ApiToken", this.ToToken()));
      claimList.AddRange(this.Roles.Select<string, Claim>((Func<string, Claim>) (role => new Claim("http://schemas.microsoft.com/ws/2008/06/identity/claims/role", role))));
      return new ClaimsIdentity((IEnumerable<Claim>) claimList, "DDocAuth");
    }

    public static UserSession FromClaims(IEnumerable<Claim> claims)
    {
      if (claims == null || !claims.Any<Claim>())
        return (UserSession) null;
      return new UserSession()
      {
        DdocGroups = claims.Single<Claim>((Func<Claim, bool>) (c => c.Type == "DdocGroups")).Value,
        DdocToken = (Permissions) Enum.Parse(typeof (Permissions), claims.Single<Claim>((Func<Claim, bool>) (c => c.Type == "DdocToken")).Value),
        Email = claims.SingleOrDefault<Claim>((Func<Claim, bool>) (c => c.Type == "Email")).Value,
        Name = claims.Single<Claim>((Func<Claim, bool>) (c => c.Type == "Name")).Value,
        Roles = claims.Where<Claim>((Func<Claim, bool>) (c => c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role")).Select<Claim, string>((Func<Claim, string>) (c => c.Value)).ToList<string>(),
        ServerId = int.Parse(claims.Single<Claim>((Func<Claim, bool>) (c => c.Type == "ServerId")).Value),
        UserGroups = claims.Single<Claim>((Func<Claim, bool>) (c => c.Type == "UserGroups")).Value,
        Username = claims.Single<Claim>((Func<Claim, bool>) (c => c.Type == "Username")).Value,
        ApiToken = claims.Single<Claim>((Func<Claim, bool>) (c => c.Type == "ApiToken")).Value
      };
    }

    public string ToToken()
    {
      this.Expiration = DateTime.Now.AddHours(3.0);
      return Base64Encryption.EncryptText("TOKEN", JsonSerializer.Serialize<UserSession>(this), "Digital*2020!");
    }

    public static UserSession FromToken(string token) => JsonSerializer.Deserialize<UserSession>(Base64Encryption.DecryptText("TOKEN", token, "Digital*2020!"));
  }
}
