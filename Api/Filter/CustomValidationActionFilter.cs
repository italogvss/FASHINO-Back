using Domain.Exceptions;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Api.Filter
{
    public class CustomValidationActionFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                var errors = context.ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage);

                throw new ValidationException("Validation Error",errors);
            }
        }

        public void OnActionExecuted(ActionExecutedContext context) { }
    }
}
