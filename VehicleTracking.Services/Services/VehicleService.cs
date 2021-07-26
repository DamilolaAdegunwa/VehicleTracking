using log4net;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using VehicleTracking.Core.Entities;
using VehicleTracking.Core.Model;
using VehicleTracking.Data.Context;
using RestSharp;
using Newtonsoft.Json;
namespace VehicleTracking.Services.Services
{
    public interface IVehicleService
    {
        Task<bool> RegisterVehicle(RegisterVehicleRequest vehicle, bool allowanonymous = false);
        Task<bool> RecordPosition(RecordPositionRequest vehiclePosition, bool allowanonymous = false);
        Task<PositionOfaVehicleResponse> RetrieveTheCurrentPositionOfaVehicle(long VehicleId, bool allowanonymous = false);
        Task<IEnumerable<PositionOfaVehicleResponse>> RetrieveThePositionsOfaVehicleDuringaCertainTime(long VehicleId, DateTimeOffset? startDate, DateTimeOffset? endDate, bool allowanonymous = false, int page = 1, int size = int.MaxValue);
        //extensibility
        #region vehicle fuel
        Task<bool> RecordFuel(RecordFuelRequest vehicleFuel, bool allowanonymous = false);
        Task<DataAboutVehicleFuelResponse> RetrieveCurrentDataAboutVehicleFuel(long VehicleId, bool allowanonymous = false);
        Task<IEnumerable<DataAboutVehicleFuelResponse>> RetrieveVehicleFuelDataDuringaCertainTime(long VehicleId, DateTimeOffset? startDate, DateTimeOffset? endDate, bool allowanonymous = false, int page = 1, int size = int.MaxValue);
        #endregion

        #region vehicle speed
        Task<bool> RecordSpeed(RecordSpeedRequest vehicleSpeed, bool allowanonymous = false);
        Task<DataAboutVehicleSpeedResponse> RetrieveCurrentDataAboutVehicleSpeed(long VehicleId, bool allowanonymous = false);
        Task<IEnumerable<DataAboutVehicleSpeedResponse>> RetrieveVehicleSpeedDuringaCertainTime(long VehicleId, DateTimeOffset? startDate, DateTimeOffset? endDate, bool allowanonymous = false, int page = 1, int size = int.MaxValue);
        #endregion
        //bonus
        ReverseGeoCode GetReverseGeocode(decimal latitude, decimal longitude);
    }
    public class VehicleService: IVehicleService
    {
        private readonly ILogger<VehicleService> _logger;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IServiceHelper _serviceHelper;
        private readonly AppConfig _appConfig;

        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType.Name);
        public VehicleService(
            ILogger<VehicleService> logger
        ,UserManager<User> userManager
        ,RoleManager<Role> roleManager
        ,ApplicationDbContext applicationDbContext
        ,IServiceHelper serviceHelper
        , IOptions<AppConfig> appConfig
            )
        {
            _logger = logger;
            _userManager = userManager;
            _roleManager = roleManager;
            _applicationDbContext = applicationDbContext;
            _serviceHelper = serviceHelper;
            _appConfig = appConfig.Value;
        }

        public VehicleService()
        {

        }

        public async Task<bool> RegisterVehicle(RegisterVehicleRequest vehicleModel, bool allowanonymous = false)
        {
            var username = _serviceHelper.GetCurrentUserEmail();
            try
            {
                #region validate
                
                var user = _userManager.Users.Where(x => x.UserName == username).FirstOrDefault();
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
                if (vehicleModel == null)
                {
                    throw new Exception("Invalid search entry!");
                }
                var userId = user?.Id;
                #endregion

                #region save data
                Vehicle vehicle = new Vehicle();
                vehicle.RegistrationNumber = vehicleModel.RegistrationNumber;
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
        public async Task<bool> RecordPosition(RecordPositionRequest vehiclePosition, bool allowanonymous = false)
        {
            var username = _serviceHelper.GetCurrentUserEmail();
            try
            {
                #region validate

                var user = _userManager.Users.Where(x => x.UserName == username).FirstOrDefault();
                if (!allowanonymous)
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
                if (vehiclePosition == null)
                {
                    throw new Exception("Invalid search entry!");
                }
                var userId = user?.Id;
                //check for valid coordinates
                if(!(vehiclePosition.Latitude >= -90 && vehiclePosition.Latitude <= 90))
                {
                    throw new Exception("Invalid Latitude!");
                }
                if (!(vehiclePosition.Longitude >= -180 && vehiclePosition.Longitude <= 180))
                {
                    throw new Exception("Invalid Longitude!");
                }
                #endregion

                #region save data
                VehiclePosition vehicle = new VehiclePosition();
                vehicle.Latitude = vehiclePosition.Latitude;
                vehicle.Longitude = vehiclePosition.Longitude;
                vehicle.VehicleId = vehiclePosition.VehicleId;
                //basic properties
                vehicle.CreationTime = DateTime.Now;
                vehicle.CreatorUserId = user.Id;
                vehicle.IsDeleted = false;
                vehicle.LastModificationTime = DateTime.Now;
                vehicle.LastModifierUserId = user.Id;
                vehicle.DeleterUserId = null;
                vehicle.DeletionTime = null;
                vehicle.Id = 0;

                _applicationDbContext.VehiclePositions.Add(vehicle);
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
        public async Task<PositionOfaVehicleResponse> RetrieveTheCurrentPositionOfaVehicle(long VehicleId, bool allowanonymous = false)
        {
            var username = _serviceHelper.GetCurrentUserEmail();
            try
            {
                #region validate

                var user = _userManager.Users.Where(x => x.UserName == username).FirstOrDefault();
                if (!allowanonymous)
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
                var vehicle = _applicationDbContext.Vehicles.Find(VehicleId);
                if (vehicle == null)
                {
                    throw new Exception("Invalid vehicle id given!");
                }
                var userId = user?.Id;
                #endregion

                var data =  _applicationDbContext.VehiclePositions.OrderByDescending(a => a.CreationTime).Select(b => new PositionOfaVehicleResponse { 
                    Latitude = b.Latitude,
                    Longitude = b.Longitude,
                    VehicleId = b.VehicleId,
                    
                }).FirstOrDefault();
                data.Address = GetReverseGeocode(data.Latitude, data.Latitude).results[0].formatted_address;

                return data;
            }
            catch (Exception ex)
            {
                log.Error($"{ex.Message} :: username {username} :: {MethodBase.GetCurrentMethod().Name} :: {ex.StackTrace} ");
                //return false;
                throw ex;
            }
            //throw new NotImplementedException();
        }

        public async Task<IEnumerable<PositionOfaVehicleResponse>> RetrieveThePositionsOfaVehicleDuringaCertainTime(long VehicleId, DateTimeOffset? startDate, DateTimeOffset? endDate, bool allowanonymous = false, int page = 1, int size = int.MaxValue)
        {
            var username = _serviceHelper.GetCurrentUserEmail();
            try
            {
                #region validate

                var user = _userManager.Users.Where(x => x.UserName == username).FirstOrDefault();
                if (!allowanonymous)
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
                var vehicle = _applicationDbContext.Vehicles.Find(VehicleId);
                if (vehicle == null)
                {
                    throw new Exception("Invalid vehicle id given!");
                }
                var userId = user?.Id;
                #endregion

                var data =  _applicationDbContext.VehiclePositions.Where(a => a.CreationTime >= startDate && a.CreationTime <= endDate).OrderByDescending(a => a.CreationTime).Select(b => new PositionOfaVehicleResponse
                {
                    Latitude = b.Latitude,
                    Longitude = b.Longitude,
                    VehicleId = b.VehicleId,
                    //Address = GetReverseGeocode(b.Latitude,b.Longitude).results[0].formatted_address
                }).Skip((page - 1) * size).Take(size).ToList();
                List<PositionOfaVehicleResponse> result = new List<PositionOfaVehicleResponse>(); 
                foreach (var row in data)
                {
                    row.Address = GetReverseGeocode(row.Latitude, row.Longitude).results[0].formatted_address;
                    result.Add(row);
                }
                return result;
            }
            catch (Exception ex)
            {
                log.Error($"{ex.Message} :: username {username} :: {MethodBase.GetCurrentMethod().Name} :: {ex.StackTrace} ");
                //return false;
                throw ex;
            }
            //throw new NotImplementedException();
        }

        //extensibility
        #region vehicle fuel
        public async Task<bool> RecordFuel(RecordFuelRequest vehicleFuel, bool allowanonymous = false)
        {
            var username = _serviceHelper.GetCurrentUserEmail();
            try
            {
                #region validate

                var user = _userManager.Users.Where(x => x.UserName == username).FirstOrDefault();
                if (!allowanonymous)
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
                if (vehicleFuel == null)
                {
                    throw new Exception("Invalid search entry!");
                }
                var userId = user?.Id;
                if(vehicleFuel.Fuel < 0)
                {
                    throw new Exception("Fuel quantity cannot be less than 0!");
                }
                #endregion

                #region save data
                VehicleFuel vehicle = new VehicleFuel();
                
                vehicle.Fuel = vehicleFuel.Fuel;
                vehicle.VehicleId = vehicleFuel.VehicleId;
                //basic properties
                vehicle.CreationTime = DateTime.Now;
                vehicle.CreatorUserId = user.Id;
                vehicle.IsDeleted = false;
                vehicle.LastModificationTime = DateTime.Now;
                vehicle.LastModifierUserId = user.Id;
                vehicle.DeleterUserId = null;
                vehicle.DeletionTime = null;
                vehicle.Id = 0;

                _applicationDbContext.VehicleFuel.Add(vehicle);
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

        public async Task<DataAboutVehicleFuelResponse> RetrieveCurrentDataAboutVehicleFuel(long VehicleId, bool allowanonymous = false)
        {
            var username = _serviceHelper.GetCurrentUserEmail();
            try
            {
                #region validate

                var user = _userManager.Users.Where(x => x.UserName == username).FirstOrDefault();
                if (!allowanonymous)
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
                var vehicle = _applicationDbContext.Vehicles.Find(VehicleId);
                if (vehicle == null)
                {
                    throw new Exception("Invalid vehicle id given!");
                }
                var userId = user?.Id;
                #endregion

                return _applicationDbContext.VehicleFuel.OrderByDescending(a => a.CreationTime).Select(b => new DataAboutVehicleFuelResponse
                {
                    Fuel = b.Fuel,
                    VehicleId = b.VehicleId,
                }).FirstOrDefault();
            }
            catch (Exception ex)
            {
                log.Error($"{ex.Message} :: username {username} :: {MethodBase.GetCurrentMethod().Name} :: {ex.StackTrace} ");
                //return false;
                throw ex;
            }
            //throw new NotImplementedException();
        }

        public async Task<IEnumerable<DataAboutVehicleFuelResponse>> RetrieveVehicleFuelDataDuringaCertainTime(long VehicleId, DateTimeOffset? startDate, DateTimeOffset? endDate, bool allowanonymous = false, int page = 1, int size = int.MaxValue)
        {
            var username = _serviceHelper.GetCurrentUserEmail();
            try
            {
                #region validate

                var user = _userManager.Users.Where(x => x.UserName == username).FirstOrDefault();
                if (!allowanonymous)
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
                var vehicle = _applicationDbContext.Vehicles.Find(VehicleId);
                if (vehicle == null)
                {
                    throw new Exception("Invalid vehicle id given!");
                }
                var userId = user?.Id;
                #endregion

                return _applicationDbContext.VehicleFuel.Where(a => a.CreationTime >= startDate && a.CreationTime <= endDate).OrderByDescending(a => a.CreationTime).Select(b => new DataAboutVehicleFuelResponse
                {
                    Fuel = b.Fuel,
                    VehicleId = b.VehicleId,
                }).Skip((page - 1) * size).Take(size).ToList();
            }
            catch (Exception ex)
            {
                log.Error($"{ex.Message} :: username {username} :: {MethodBase.GetCurrentMethod().Name} :: {ex.StackTrace} ");
                //return false;
                throw ex;
            }
            //throw new NotImplementedException();
        }
        #endregion

        #region vehicle speed
        public async Task<bool> RecordSpeed(RecordSpeedRequest vehicleSpeed, bool allowanonymous = false)
        {
            var username = _serviceHelper.GetCurrentUserEmail();
            try
            {
                #region validate

                var user = _userManager.Users.Where(x => x.UserName == username).FirstOrDefault();
                if (!allowanonymous)
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
                if (vehicleSpeed == null)
                {
                    throw new Exception("Invalid search entry!");
                }
                var userId = user?.Id;
                if (vehicleSpeed.Speed < 0)
                {
                    throw new Exception("Speed cannot be less than 0!");
                }
                #endregion

                #region save data
                VehicleSpeed vehicle = new VehicleSpeed();

                vehicle.Speed = vehicleSpeed.Speed;
                vehicle.VehicleId = vehicleSpeed.VehicleId;
                //basic properties
                vehicle.CreationTime = DateTime.Now;
                vehicle.CreatorUserId = user.Id;
                vehicle.IsDeleted = false;
                vehicle.LastModificationTime = DateTime.Now;
                vehicle.LastModifierUserId = user.Id;
                vehicle.DeleterUserId = null;
                vehicle.DeletionTime = null;
                vehicle.Id = 0;

                _applicationDbContext.VehicleSpeed.Add(vehicle);
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
        }

        public async Task<DataAboutVehicleSpeedResponse> RetrieveCurrentDataAboutVehicleSpeed(long VehicleId, bool allowanonymous = false)
        {
            var username = _serviceHelper.GetCurrentUserEmail();
            try
            {
                #region validate

                var user = _userManager.Users.Where(x => x.UserName == username).FirstOrDefault();
                if (!allowanonymous)
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
                var vehicle = _applicationDbContext.Vehicles.Find(VehicleId);
                if (vehicle == null)
                {
                    throw new Exception("Invalid vehicle id given!");
                }
                var userId = user?.Id;
                #endregion

                return _applicationDbContext.VehicleSpeed.OrderByDescending(a => a.CreationTime).Select(b => new DataAboutVehicleSpeedResponse
                {
                    Speed = b.Speed,
                    VehicleId = b.VehicleId,
                }).FirstOrDefault();
            }
            catch (Exception ex)
            {
                log.Error($"{ex.Message} :: username {username} :: {MethodBase.GetCurrentMethod().Name} :: {ex.StackTrace} ");
                //return false;
                throw ex;
            }
        }

        public async Task<IEnumerable<DataAboutVehicleSpeedResponse>> RetrieveVehicleSpeedDuringaCertainTime(long VehicleId, DateTimeOffset? startDate, DateTimeOffset? endDate, bool allowanonymous = false, int page = 1, int size = int.MaxValue)
        {
            var username = _serviceHelper.GetCurrentUserEmail();
            try
            {
                #region validate

                var user = _userManager.Users.Where(x => x.UserName == username).FirstOrDefault();
                if (!allowanonymous)
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
                var vehicle = _applicationDbContext.Vehicles.Find(VehicleId);
                if (vehicle == null)
                {
                    throw new Exception("Invalid vehicle id given!");
                }
                var userId = user?.Id;
                #endregion

                return _applicationDbContext.VehicleSpeed.Where(a => a.CreationTime >= startDate && a.CreationTime <= endDate).OrderByDescending(a => a.CreationTime).Select(b => new DataAboutVehicleSpeedResponse
                {
                    Speed = b.Speed,
                    VehicleId = b.VehicleId,
                }).Skip((page - 1) * size).Take(size).ToList();
            }
            catch (Exception ex)
            {
                log.Error($"{ex.Message} :: username {username} :: {MethodBase.GetCurrentMethod().Name} :: {ex.StackTrace} ");
                //return false;
                throw ex;
            }
            //throw new NotImplementedException();
        }
        #endregion

        //for the bonus task
        public ReverseGeoCode GetReverseGeocode(decimal latitude, decimal longitude)
        {
            try
            {
                RestClient client = new RestClient($"https://maps.googleapis.com/maps/api/geocode/json?latlng={latitude},{longitude}&key={_appConfig.GoogleApiKey}");
                //RestClient client = new RestClient($"https://maps.googleapis.com/maps/api/geocode/json?latlng={latitude},{longitude}&key=AIzaSyAjN-B802v815bzoSkjhwHFimBFvZh6FxA");
                RestRequest request = new RestRequest { 
                    Method = Method.GET,
                    RequestFormat = DataFormat.Json
                };
                IRestResponse response = client.Execute<ReverseGeoCode>(request);
                return JsonConvert.DeserializeObject<ReverseGeoCode>(response.Content);
            }
            catch (Exception ex)
            {
                log.Error($"{ex.Message} :: {MethodBase.GetCurrentMethod().Name} :: {ex.StackTrace} ");
                //return false;
                return null ;
            }
        }
    }
}
