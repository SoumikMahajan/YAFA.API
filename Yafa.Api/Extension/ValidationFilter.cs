using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;
using Yafa.Application.Common.Utility;

namespace Yafa.Api.Extension
{
    #pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
	public class ValidationFilter : IAsyncActionFilter
    {
        private readonly IServiceProvider _serviceProvider;

        public ValidationFilter(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            foreach (var argument in context.ActionArguments.Values)
            {
                if (argument == null) continue;

                var argumentType = argument.GetType();
                var validatorType = typeof(IValidator<>).MakeGenericType(argumentType);

                var validator = _serviceProvider.GetService(validatorType) as IValidator;

                if (validator != null)
                {
                    var contextType = typeof(ValidationContext<>).MakeGenericType(argumentType);
                    var validationContext = Activator.CreateInstance(contextType, argument) as IValidationContext;

                    var validationResult = await validator.ValidateAsync(validationContext);

                    if (!validationResult.IsValid)
                    {
                        // Take only the FIRST error for each property
                        var errors = validationResult.Errors
                            .GroupBy(e => e.PropertyName)
                            .Select(g => g.First().ErrorMessage)
                            .ToList();

                        var response = new APIResponse
                        {
                            StatusCode = HttpStatusCode.BadRequest,
                            IsSuccess = false,
                            ErrorMessages = errors
                        };

                        context.Result = new BadRequestObjectResult(response);
                        return;
                    }
                }
            }

            await next();
        }
    }
}