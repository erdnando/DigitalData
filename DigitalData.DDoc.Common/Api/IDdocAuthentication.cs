
using DigitalData.Open.Common.Entities.Security;
using System.Threading.Tasks;

namespace DigitalData.Open.Common.Api
{
  public interface IDdocAuthentication
  {
    string Name { get; }

    Task<User> AuthenticateUser(User userLogin);
  }
}
