using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace Yafa.Application.Common.Utility;

public class ApiResponseService
{
    public ActionResult<APIResponse> SuccessResponse(object result)
    {
        var response = new APIResponse
        {
            StatusCode = HttpStatusCode.OK,
            IsSuccess = true,
            Result = result is string ? result.ToString() : result
        };
        return new OkObjectResult(response);
    }

    public ActionResult<APIResponse> NotFoundResponse(string errorMessage)
    {
        var response = new APIResponse
        {
            StatusCode = HttpStatusCode.NotFound,
            IsSuccess = false,
            ErrorMessages = new List<string> { errorMessage }
        };
        return new NotFoundObjectResult(response);
    }

    public ActionResult<APIResponse> BadRequestResponse(string errorMessage)
    {
        var response = new APIResponse
        {
            StatusCode = HttpStatusCode.BadRequest,
            IsSuccess = false,
            ErrorMessages = new List<string> { errorMessage }
        };
        return new BadRequestObjectResult(response);
    }

    public ActionResult<APIResponse> InternalServerResponse(string errorMessage)
    {
        var response = new APIResponse
        {
            StatusCode = HttpStatusCode.InternalServerError,
            IsSuccess = false,
            ErrorMessages = new List<string> { errorMessage }
        };
        return new ObjectResult(response)
        {
            StatusCode = (int)HttpStatusCode.InternalServerError
        };
    }

    public ActionResult<APIResponse> ErrorResponse(string errorMessage)
    {
        var response = new APIResponse
        {
			StatusCode = HttpStatusCode.NotFound,
			IsSuccess = false,
            ErrorMessages = new List<string> { errorMessage }
        };
        return new NotFoundObjectResult(response);
    }

    public ActionResult<APIResponse> UnAuthorize(string errorMessage)
    {
        var response = new APIResponse
        {
            StatusCode = HttpStatusCode.Unauthorized,
            IsSuccess = false,
            ErrorMessages = new List<string> { errorMessage }
        };
        return new UnauthorizedObjectResult(response);
    }

}

public class APIResponse
{
    public APIResponse()
    {
        ErrorMessages = new List<string>();
    }
    public HttpStatusCode StatusCode { get; set; }
    public bool IsSuccess { get; set; } = true;
    public List<string> ErrorMessages { get; set; }
    public object? Result { get; set; }
}
