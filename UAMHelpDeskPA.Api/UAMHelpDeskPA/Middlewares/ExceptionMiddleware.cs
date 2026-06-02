using UamHelpDeskPA.Api.DTOs;


namespace UamHelpDeskPA.Api.Middlewares
{
    public class ExceptionMiddleware(RequestDelegate next)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                context.Response.ContentType = "application/json";

                var response = new ApiOperationResultDto<object>
                {
                    Success = false,
                    Code = StatusCodes.Status500InternalServerError.ToString(),
                    Message = ex.Message,
                    Result = null
                };

                await context.Response.WriteAsJsonAsync(response);
            }
        }
    }
}
