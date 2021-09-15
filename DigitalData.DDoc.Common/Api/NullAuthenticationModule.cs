
using DigitalData.Open.Common.Entities.Security;
using System;
using System.Threading.Tasks;

namespace DigitalData.Open.Common.Api
{
  public class NullAuthenticationModule : IDdocAuthentication
  {
    public string Name { get; } = "None";

    public Task<User> AuthenticateUser(User userLogin) => throw new NotImplementedException();
  }
}
