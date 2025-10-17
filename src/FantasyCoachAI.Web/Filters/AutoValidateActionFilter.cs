using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using FluentValidation;
using System.Text.Json;

namespace FantasyCoachAI.Web.Filters;

/// <summary>
/// Action Filter that automatically validates DTOs using FluentValidation
/// </summary>
public class AutoValidateActionFilter : IActionFilter
{
    private readonly ILogger<AutoValidateActionFilter> _logger;

    public AutoValidateActionFilter(ILogger<AutoValidateActionFilter> logger)
    {
        _logger = logger;
    }

    public void OnActionExecuting(ActionExecutingContext context)
    {
        var serviceProvider = context.HttpContext.RequestServices;
        
        foreach (var parameter in context.ActionDescriptor.Parameters)
        {
            if (context.ActionArguments.TryGetValue(parameter.Name, out var argument) && argument != null)
            {
                var argumentType = argument.GetType();
                var validatorType = typeof(IValidator<>).MakeGenericType(argumentType);
                var validator = serviceProvider.GetService(validatorType);
                
                if (validator != null)
                {
                    _logger.LogDebug("Validating {ArgumentType} with {ValidatorType}", argumentType.Name, validator.GetType().Name);
                    
                    // Use reflection to call the generic Validate method
                    var validateMethod = validatorType.GetMethod("Validate", new[] { argumentType });
                    var validationResult = validateMethod?.Invoke(validator, new[] { argument });
                    
                    if (validationResult is FluentValidation.Results.ValidationResult result && !result.IsValid)
                    {
                        _logger.LogWarning("Validation failed for {ArgumentType}: {Errors}", 
                            argumentType.Name, 
                            string.Join(", ", result.Errors.Select(e => e.ErrorMessage)));
                        
                        var errorResponse = new
                        {
                            message = "Validation failed",
                            errors = result.Errors.Select(e => new
                            {
                                field = e.PropertyName,
                                message = e.ErrorMessage,
                                attemptedValue = e.AttemptedValue
                            })
                        };
                        
                        context.Result = new BadRequestObjectResult(errorResponse);
                        return;
                    }
                    
                    _logger.LogDebug("Validation passed for {ArgumentType}", argumentType.Name);
                }
            }
        }
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        // No action needed after execution
    }
}

/// <summary>
/// Attribute to mark actions/controllers for automatic validation
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
public class AutoValidateAttribute : Attribute, IFilterFactory
{
    public bool IsReusable => true;

    public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
    {
        var logger = serviceProvider.GetRequiredService<ILogger<AutoValidateActionFilter>>();
        return new AutoValidateActionFilter(logger);
    }
}
