using IdentityModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using VehicleTracking.Core.Entities;
using VehicleTracking.Core.Model;
using VehicleTracking.Data.Context;
using VehicleTracking.Services.Services;
namespace VehicleTracking.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : BaseController
    {
        #region main account endpoints
        private readonly ILogger<AccountController> _logger;
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IServiceHelper _serviceHelper;
        private readonly IAccountService _accountService;
        public AccountController(
            ILogger<AccountController> logger
            , SignInManager<User> signInManager
            , UserManager<User> userManager
            , RoleManager<Role> roleManager
            , ApplicationDbContext applicationDbContext
            , IServiceHelper serviceHelper
            , IAccountService accountService
            )
        {
            _logger = logger;
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
            _applicationDbContext = applicationDbContext;
            _serviceHelper = serviceHelper;
            _accountService = accountService;
        }

        [HttpGet]
        [Route("GetProfile")]
        public async Task<IServiceResponse<User>> GetCurrentUserProfile()
        {
            return await HandleApiOperationAsync(async () => {

                var response = new ServiceResponse<User>();

                var profile = await _userManager.FindByNameAsync(User.FindFirst(JwtClaimTypes.Name)?.Value);
                response.Object = profile;
                return response;
            });
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("SignUp")]/*SignUp - Create your account */
        public async Task<IServiceResponse<bool>> SignUp(LoginModel loginModel)
        {
            return await HandleApiOperationAsync(async () => {
                await _accountService.SignUp(loginModel);
                return new ServiceResponse<bool>(true);
            });
        }


        #endregion

        [HttpPost]
        [Route("AddUser")]
        public async Task<ServiceResponse<bool>> AddUser(LoginModel model)
        {
            return await HandleApiOperationAsync(async () => {
                var response = new ServiceResponse<bool>();
                var data = await _accountService.AddUser(model, _serviceHelper.GetCurrentUserEmail());
                response.Object = data;
                return response;
            });
        }        
    }
}
