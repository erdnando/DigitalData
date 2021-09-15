
namespace DigitalData.Open.Common.Entities.Api
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
