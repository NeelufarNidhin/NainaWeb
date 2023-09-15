using System;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace NainaBoutique.Utility
{
    public class ErrorHandlingMiddleware : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch(Exception ex)
            {
                var msg = ex.Message.ToString();
                _logger.LogInformation(msg);
            }
        }

        private readonly ILogger<ErrorHandlingMiddleware> _logger;

        public ErrorHandlingMiddleware(ILogger<ErrorHandlingMiddleware> logger)
        {
            _logger = logger;
        }
    }
}

