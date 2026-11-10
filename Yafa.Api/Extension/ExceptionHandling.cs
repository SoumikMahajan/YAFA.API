using System.Net;
using System.Text.Json;
using Yafa.Application.Common.Interfaces;
using Yafa.Application.Common.Utility;
using Yafa.Infrastructure.Repository;

namespace Yafa.Api.Extension
{
    #pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
	public class ExceptionHandling : IMiddleware
    {
        private readonly ILogService _logService;
        private readonly IUnitOfWork _unitOfWork;
        public ExceptionHandling(ILogService logService, IUnitOfWork unitOfWork)
        {
            _logService = logService;
            _unitOfWork = unitOfWork;
        }
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
			catch (OperationCanceledException) when (context.RequestAborted.IsCancellationRequested)
			{
				context.Response.StatusCode = 499;
				return;
			}
			catch (Exception ex)
            {
                if (_unitOfWork.HasActiveTransaction)
					await _unitOfWork.RollbackTransactionAsync();

				await _logService.LogExceptionErrorAsync(ex);
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            var response = context.Response;
            response.ContentType = "application/json";

            var statusCode = ex switch
            {
                UnauthorizedAccessException => HttpStatusCode.Unauthorized, // 401
                ArgumentException => HttpStatusCode.BadRequest, // 400
                KeyNotFoundException => HttpStatusCode.NotFound, // 404
                InvalidOperationException => HttpStatusCode.Conflict, // 409
                NotImplementedException => HttpStatusCode.NotImplemented, // 501
                TimeoutException => HttpStatusCode.RequestTimeout, // 408
                AccessViolationException => HttpStatusCode.Forbidden, // 403
                FormatException => HttpStatusCode.UnprocessableEntity, // 422
                OperationCanceledException => HttpStatusCode.RequestTimeout, // 499
                DivideByZeroException => HttpStatusCode.InternalServerError, // 500 (Internal logic error)                
                _ => HttpStatusCode.InternalServerError // Default fallback
            };

            var errorResponse = new APIResponse
            {
                StatusCode = statusCode,
                IsSuccess = false,
                ErrorMessages = new List<string> { ex.Message }
                //ErrorMessages = new List<string> { ex.Message, ex.StackTrace ?? "No stack trace available" }
            };

            var result = JsonSerializer.Serialize(errorResponse);
            response.StatusCode = (int)statusCode;
            await response.WriteAsync(result);

        }

    }

    public static class ExceptionMiddlewareExtension
    {
        public static void ConfigureExceptionMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<ExceptionHandling>();
        }
    }
}
