using log4net;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using VehicleTracking.Services.Services;

namespace VehicleTracking.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseController : ControllerBase
    {
        protected async Task<ServiceResponse<T>> HandleApiOperationAsync<T>(
       Func<Task<ServiceResponse<T>>> action, [CallerLineNumber] int lineNo = 0, [CallerMemberName] string method = "")
        {
            var _logger = LogManager.GetLogger(typeof(BaseController));

            _logger.Info($"ENTERS ({method}) method");

            var serviceResponse = new ServiceResponse<T>
            {
                Code = ((int)HttpStatusCode.OK).ToString(),
                ShortDescription = "SUCCESS"
            };

            try
            {
                if (!ModelState.IsValid)
                {
                    throw new Exception("There were errors in your input, please correct them and try again.");
                }
                var actionResponse = await action();

                serviceResponse.Object = actionResponse.Object;
                serviceResponse.ShortDescription = actionResponse.ShortDescription ?? serviceResponse.ShortDescription;
                serviceResponse.Code = actionResponse.Code ?? serviceResponse.Code;

            }
            catch (Exception ex)
            {
                serviceResponse.ShortDescription = ex.Message;
                serviceResponse.Code = ((int)HttpStatusCode.BadRequest).ToString();
                if (!ModelState.IsValid)
                {
                    serviceResponse.ValidationErrors = ModelState.ToDictionary(
                        m => {
                            var tokens = m.Key.Split('.');
                            return tokens.Length > 0 ? tokens[tokens.Length - 1] : tokens[0];
                        },
                        m => m.Value.Errors.Select(e => e.Exception?.Message ?? e.ErrorMessage)
                    );
                }
                _logger.Error(ex.Message, ex);
            }

            _logger.Info($"EXITS ({method}) method");

            return serviceResponse;
        }
    }
}
