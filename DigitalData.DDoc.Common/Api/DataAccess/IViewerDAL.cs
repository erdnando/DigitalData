
using System.Threading.Tasks;

namespace DigitalData.Open.Common.Api.DataAccess
{
  public interface IViewerDAL : ICommonDAL
  {
    Task<string> GetPageType(string pageId);

    Task<string> GetWarehousePath(string pageId);
  }
}
