using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VehicleTracking.Core.Entities;
using VehicleTracking.Data.Context;
using VehicleTracking.Services.Services;
namespace VehicleTracking.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class VehicleController : BaseController
    {
        private readonly ILogger<VehicleController> _logger;
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IServiceHelper _serviceHelper;
        private readonly IAccountService _accountService;
        private readonly IVehicleService _vehicleService;
        public VehicleController(
            ILogger<VehicleController> logger
            , SignInManager<User> signInManager
            , UserManager<User> userManager
            , RoleManager<Role> roleManager
            , ApplicationDbContext applicationDbContext
            , IServiceHelper serviceHelper
            , IAccountService accountService
            , IVehicleService vehicleService
            )
        {
            _logger = logger;
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
            _applicationDbContext = applicationDbContext;
            _serviceHelper = serviceHelper;
            _accountService = accountService;
            _vehicleService = vehicleService;
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IServiceResponse<bool>> RegisterVehicle(Vehicle vehicle)
        {
            return await HandleApiOperationAsync(async () => {
                var response = new ServiceResponse<bool>();
                var data = await _vehicleService.RegisterVehicle(vehicle);
                response.Object = data;
                return response;
            });
        }
        [HttpPost]
        [Route("[action]")]
        public async Task<IServiceResponse<bool>> RecordPosition(VehiclePosition vehiclePosition)
        {
            return await HandleApiOperationAsync(async () => {
                var response = new ServiceResponse<bool>();
                var data = await _vehicleService.RecordPosition(vehiclePosition);
                response.Object = data;
                return response;
            });
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IServiceResponse<VehiclePosition>> RetrieveTheCurrentPositionOfaVehicle(long VehicleId)
        {
            return await HandleApiOperationAsync(async () => {
                var response = new ServiceResponse<VehiclePosition>();
                var data = await _vehicleService.RetrieveTheCurrentPositionOfaVehicle(VehicleId);
                response.Object = data;
                return response;
            });
        }
        [HttpGet]
        [Route("[action]")]
        public async Task<IServiceResponse<IEnumerable<VehiclePosition>>> RetrieveThePositionsOfaVehicleDuringaCertainTime(long VehicleId, DateTimeOffset? startDate, DateTimeOffset? endDate)
        {
            return await HandleApiOperationAsync(async () => {
                var response = new ServiceResponse<IEnumerable<VehiclePosition>>();
                var data = await _vehicleService.RetrieveThePositionsOfaVehicleDuringaCertainTime(VehicleId, startDate, endDate);
                response.Object = data;
                return response;
            });
        }

        #region vehicle fuel
        [HttpPost]
        [Route("[action]")]
        public async Task<IServiceResponse<bool>> RecordFuel(VehicleFuel vehicleFuel)
        {
            return await HandleApiOperationAsync(async () => {
                var response = new ServiceResponse<bool>();
                var data = await _vehicleService.RecordFuel(vehicleFuel);
                response.Object = data;
                return response;
            });
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IServiceResponse<VehicleFuel>> RetrieveCurrentDataAboutVehicleFuel(long VehicleId)
        {
            return await HandleApiOperationAsync(async () => {
                var response = new ServiceResponse<VehicleFuel>();
                var data = await _vehicleService.RetrieveCurrentDataAboutVehicleFuel(VehicleId);
                response.Object = data;
                return response;
            });
        }
        [HttpGet]
        [Route("[action]")]
        public async Task<IServiceResponse<IEnumerable<VehicleFuel>>> RetrieveVehicleFuelDataDuringaCertainTime(long VehicleId, DateTimeOffset? startDate, DateTimeOffset? endDate)
        {
            return await HandleApiOperationAsync(async () => {
                var response = new ServiceResponse<IEnumerable<VehicleFuel>>();
                var data = await _vehicleService.RetrieveVehicleFuelDataDuringaCertainTime(VehicleId,startDate,endDate);
                response.Object = data;
                return response;
            });
        }
        #endregion

        #region vehicle speed
        [HttpPost]
        [Route("[action]")]
        public async Task<IServiceResponse<bool>> RecordSpeed(VehicleSpeed vehicleSpeed)
        {
            return await HandleApiOperationAsync(async () => {
                var response = new ServiceResponse<bool>();
                var data = await _vehicleService.RecordSpeed(vehicleSpeed);
                response.Object = data;
                return response;
            });
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IServiceResponse<VehicleSpeed>> RetrieveCurrentDataAboutVehicleSpeed(long VehicleId)
        {
            return await HandleApiOperationAsync(async () => {
                var response = new ServiceResponse<VehicleSpeed>();
                var data = await _vehicleService.RetrieveCurrentDataAboutVehicleSpeed(VehicleId);
                response.Object = data;
                return response;
            });
        }
        [HttpGet]
        [Route("[action]")]
        public async Task<IServiceResponse<IEnumerable<VehicleSpeed>>> RetrieveVehicleSpeedDuringaCertainTime(long VehicleId, DateTimeOffset? startDate, DateTimeOffset? endDate)
        {
            return await HandleApiOperationAsync(async () => {
                var response = new ServiceResponse<IEnumerable<VehicleSpeed>>();
                var data = await _vehicleService.RetrieveVehicleSpeedDuringaCertainTime(VehicleId, startDate, endDate);
                response.Object = data;
                return response;
            });
        }
        #endregion
    }
}
