using Banking.API.Helper;
using Banking.Shared.DTOs;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Banking.API.ActionFilters
{
public class FluentValidationFilter : IAsyncActionFilter
    {
        private readonly ILogger<FluentValidationFilter> _logger;

        public FluentValidationFilter(ILogger<FluentValidationFilter> logger)
        {
            _logger = logger;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var actionArguments = context.ActionArguments;

            _logger.LogDebug(nameof(OnActionExecutionAsync));
            _logger.LogDebug("All Action Arguments: {@actionArguments}", actionArguments);


            foreach (var actionArgument in actionArguments)
            {
                _logger.LogDebug("Action Argument: {@actionArgument}", actionArgument);

                if(actionArgument.Value is IBaseValidationModel model) 
                {
                    var modelType = actionArgument.Value.GetType();
                    _logger.LogDebug("Model Type: {@modelType}", modelType);
                    var genericType = typeof(IValidator<>).MakeGenericType(modelType);
                    _logger.LogDebug("Generic Type: {@genericType}", genericType);

                    var validator = context.HttpContext.RequestServices.GetService(genericType);

                    if(validator != null) 
                    {
                        var validationResult = model.Validate(validator, model);

                        if(validationResult != null && !validationResult.IsValid) 
                        {
                            _logger.LogInformation("Validation has been failed");
                            _logger.LogDebug("Validation Error: {@validationResult}", validationResult);
                            var errorDetailsInstance = ErrorDetailsHelper.CreateErrorDetails(validationResult);

                            context.Result = new BadRequestObjectResult(errorDetailsInstance);

                            return;
                        }

                        _logger.LogInformation("Validation has been successful");

                    }
                }
            }


            await next(); // Move to the next stage in the pipeline.
        }


    }
}