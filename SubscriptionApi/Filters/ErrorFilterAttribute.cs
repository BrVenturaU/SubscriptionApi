using Microsoft.AspNetCore.Mvc.Filters;

namespace SubscriptionApi.Filters
{
    public class ErrorFilterAttribute: ExceptionFilterAttribute
    {
        private readonly ILogger<ErrorFilterAttribute> _logger;

        public ErrorFilterAttribute(ILogger<ErrorFilterAttribute> logger)
        {
            _logger = logger;
        }

        public override void OnException(ExceptionContext context)
        {
            _logger.LogError(context.Exception, context.Exception.Message);
            base.OnException(context);
        }
    }
}
