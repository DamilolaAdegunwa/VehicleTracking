using log4net;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using VehicleTracking.Core.Entities;
using VehicleTracking.Data.Context;

namespace VehicleTracking.Services.Services
{
    public interface IVehicleService
    {
        Task<bool> RegisterVehicle(Vehicle vehicle, bool allowanonymous = false);
        Task<bool> RecordPosition(VehiclePosition vehiclePosition);
        Task<VehiclePosition> RetrieveTheCurrentPositionOfaVehicle(long VehicleId);
        Task<IEnumerable<VehiclePosition>> RetrieveThePositionsOfaVehicleDuringaCertainTime(long VehicleId, DateTimeOffset? startDate, DateTimeOffset? endDate);
        #region vehicle fuel
        Task<bool> RecordFuel(VehicleFuel vehicleFuel);
        Task<VehicleFuel> RetrieveCurrentDataAboutVehicleFuel(long VehicleId);
        Task<IEnumerable<VehicleFuel>> RetrieveVehicleFuelDataDuringaCertainTime(long VehicleId, DateTimeOffset? startDate, DateTimeOffset? endDate);
        #endregion

        #region vehicle speed
        Task<bool> RecordSpeed(VehicleSpeed vehicleSpeed);
        Task<VehicleSpeed> RetrieveCurrentDataAboutVehicleSpeed(long VehicleId);
        Task<IEnumerable<VehicleSpeed>> RetrieveVehicleSpeedDuringaCertainTime(long VehicleId, DateTimeOffset? startDate, DateTimeOffset? endDate);
        #endregion
    }
    public class VehicleService: IVehicleService
    {
        private readonly ILogger<VehicleService> _logger;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IServiceHelper _serviceHelper;


        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType.Name);
        public VehicleService(
            ILogger<VehicleService> logger
        ,UserManager<User> userManager
        ,RoleManager<Role> roleManager
        ,ApplicationDbContext applicationDbContext
        ,IServiceHelper serviceHelper
            )
        {
            _logger = logger;
            _userManager = userManager;
            _roleManager = roleManager;
            _applicationDbContext = applicationDbContext;
            _serviceHelper = serviceHelper;
        }
        public async Task<bool> RegisterVehicle(Vehicle vehicle, bool allowanonymous = false)
        {
            var username = _serviceHelper.GetCurrentUserEmail();
            try
            {
                #region validate
                
                var user = _userManager.Users.Where(x => x.UserName == username).FirstOrDefaultAsync();
                if(!allowanonymous)
                {
                    if (string.IsNullOrWhiteSpace(username))
                    {
                        throw new Exception("Please input a username!");
                    }

                    if (user == null)
                    {
                        throw new Exception("Unauthorized access! Please login");
                    }
                }
                if (vehicle == null)
                {
                    throw new Exception("Invalid search entry!");
                }
                var userId = user?.Id;
                #endregion

                #region save data
                //basic properties
                vehicle.CreationTime = DateTime.Now;
                vehicle.CreatorUserId = user.Id;
                vehicle.IsDeleted = false;
                vehicle.LastModificationTime = DateTime.Now;
                vehicle.LastModifierUserId = user.Id;
                vehicle.DeleterUserId = null;
                vehicle.DeletionTime = null;
                vehicle.Id = 0;

                _applicationDbContext.Vehicles.Add(vehicle);
                _applicationDbContext.SaveChanges();
                #endregion
                return true;
            }
            catch (Exception ex)
            {
                log.Error($"{ex.Message} :: username {username} :: {MethodBase.GetCurrentMethod().Name} :: {ex.StackTrace} ");
                //return false;
                throw ex;
            }
            //throw new NotImplementedException();
        }
        public async Task<bool> RecordPosition(VehiclePosition vehiclePosition)
        {
            throw new NotImplementedException();
        }
        public async Task<VehiclePosition> RetrieveTheCurrentPositionOfaVehicle(long VehicleId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<VehiclePosition>> RetrieveThePositionsOfaVehicleDuringaCertainTime(long VehicleId, DateTimeOffset? startDate, DateTimeOffset? endDate)
        {
            throw new NotImplementedException();
        }


        #region vehicle fuel
        public Task<bool> RecordFuel(VehicleFuel vehicleFuel)
        {
            throw new NotImplementedException();
        }

        public Task<VehicleFuel> RetrieveCurrentDataAboutVehicleFuel(long VehicleId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<VehicleFuel>> RetrieveVehicleFuelDataDuringaCertainTime(long VehicleId, DateTimeOffset? startDate, DateTimeOffset? endDate)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region vehicle speed
        public Task<bool> RecordSpeed(VehicleSpeed vehicleSpeed)
        {
            throw new NotImplementedException();
        }

        public Task<VehicleSpeed> RetrieveCurrentDataAboutVehicleSpeed(long VehicleId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<VehicleSpeed>> RetrieveVehicleSpeedDuringaCertainTime(long VehicleId, DateTimeOffset? startDate, DateTimeOffset? endDate)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
