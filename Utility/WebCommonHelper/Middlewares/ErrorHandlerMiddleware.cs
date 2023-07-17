using Microsoft.AspNetCore.Http;
using System.Net;
using System.Text.Json;
using LogHelper;
using Microsoft.AspNetCore.Hosting;

namespace WebCommonHelper.Middlewares
{
    public class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;

        private LogHelper.Nlog.LogModule logger = new LogHelper.Nlog.LogModule();

        public ErrorHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, IWebHostEnvironment env)
        {
            bool logged = false;
            try
            {
                try
                {
                    await _next(context);
                }
                catch(AggregateException errors) // 處理async Task的 Exception
                {
                    foreach (var error in errors.InnerExceptions)
                    {
                        logger.Info("AggregateException error =========================");
                        logger.Info(error.ToString());
                        logged = true;
                        throw error;
                    }
                }
                catch(Exception ex)
                {
                    if (!logged)
                    {
                        logger.Info("ErrorHandlerMiddleware inner error =========================");
                        logger.Info(ex.ToString());
                    }
                    
                    throw ex;
                }
                
            }
            catch(Exception error)
            {
                if (!logged)
                {
                    logger.Info("ErrorHandlerMiddleware error =========================");
                    logger.Info(error.GetType().Name);
                    logger.Info(error.ToString());
                }

                var response = context.Response;
                response.ContentType = "application/json";

                string result = string.Empty;

                switch (error)
                {
                    case WebCommonHelperException e:
                        // custom application error
                        response.StatusCode = (int)HttpStatusCode.BadRequest;
                        result = JsonSerializer.Serialize(new { message = error?.Message });
                        break;
                    case ProtocolViolationException pve:
                    case InvalidDataException ide:
                    case InvalidOperationException ioe:
                    case RequestApiException e:
                        response.StatusCode = (int)HttpStatusCode.BadRequest;
                        if (env.EnvironmentName == "Development")
                        {
                            result = JsonSerializer.Serialize(new { message = error?.Message });
                        }
                        else
                        {
                            result = JsonSerializer.Serialize(new { message = "Something went error." });
                        }
                        break;
                    case OptionMissingException e:
                        response.StatusCode = (int)HttpStatusCode.BadRequest;
                        if (env.EnvironmentName == "Development")
                        {
                            result = JsonSerializer.Serialize(new { message = error?.Message });
                        }
                        else
                        {
                            result = JsonSerializer.Serialize(new { message = "Something went error, please contact Computer User Help Desk." });
                        }
                        break;
                    case UnauthorizedAccessException e:
                        response.StatusCode = (int)HttpStatusCode.Unauthorized;
                        result = JsonSerializer.Serialize(new { message = error?.Message });
                        break;
                    case KeyNotFoundException e:
                        // not found error
                        response.StatusCode = (int)HttpStatusCode.NotFound;
                        result = JsonSerializer.Serialize(new { message = error?.Message });
                        break;
                    case TokenTimeoutException e:
                        // not found error
                        response.StatusCode = (int)HttpStatusCode.RequestTimeout;
                        if (env.EnvironmentName == "Development")
                        {
                            result = JsonSerializer.Serialize(new { message = error?.Message });
                        }
                        else
                        {
                            result = JsonSerializer.Serialize(new { message = "Please try again, the connection has timed out." });
                        }
                        break;
                    case TimeoutException e:
                        // not found error
                        response.StatusCode = (int)HttpStatusCode.RequestTimeout;
                        if (env.EnvironmentName == "Development")
                        {
                            result = JsonSerializer.Serialize(new { message = error?.Message });
                        }
                        else
                        {
                            result = JsonSerializer.Serialize(new { message = "Please try again, the connection has timed out." });
                        }
                        break;
                    case ArgumentException arge:
                    case LogicErrorException e:
                        response.StatusCode = (int)HttpStatusCode.Conflict;
                        result = JsonSerializer.Serialize(new { message = error?.Message });
                        break;
                    default:
                        // unhandled error
                        response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        if (env.EnvironmentName == "Development" || env.EnvironmentName == "Staging")
                        {
                            string internalErrMsg = error?.Message;
                            if (!string.IsNullOrEmpty(internalErrMsg) && internalErrMsg.Contains("RequestApiException"))
                            {
                                result = JsonSerializer.Serialize(new { message = "Internal Server Error (AP)." });
                            }
                            else
                            {
                                result = JsonSerializer.Serialize(new { message = error?.Message });
                            }
                        } 
                        else
                        {
                            result = JsonSerializer.Serialize(new { message = "Please try again, if problem persists, contact Computer User Help Desk." });
                        }
                        break;
                }

                await response.WriteAsync(result);
            }

           
        }
    }
}
