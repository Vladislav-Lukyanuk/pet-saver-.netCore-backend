using animalFinder.DTO.API;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace animalFinder.Validator
{
    public class ValidateModelAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                foreach (ModelStateEntry value in context.ModelState.Values)
                {
                    foreach (ModelError error in value.Errors)
                    {
                        context.Result = new BadRequestObjectResult(new Error() { Message = error.ErrorMessage });
                        return;
                    }
                }
            }
        }
    }
}
