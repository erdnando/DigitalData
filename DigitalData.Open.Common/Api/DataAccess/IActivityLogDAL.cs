
using DigitalData.Open.Common.Entities.Helpers;
using System.Threading.Tasks;

namespace DigitalData.Open.Common.Api.DataAccess
{
  public interface IActivityLogDAL : ICommonDAL
  {
    Task WriteLogEntry(DdocActionLogEntry entry);
  }
}
