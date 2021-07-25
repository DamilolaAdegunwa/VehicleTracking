using IdentityModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Threading.Tasks;
using VehicleTracking.Core.Entities;
using VehicleTracking.Core.Model;
using VehicleTracking.Data.Context;
using VehicleTracking.Services.Extensions;
using VehicleTracking.Services.Services;

namespace VehicleTracking.API.Controllers
{
    public class TokenController : BaseController
    {
        private readonly ITokenService _tokenSvc;
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly UserManager<User> _userManager;
        public TokenController(
            ITokenService tokenSvc
            , ApplicationDbContext applicationDbContext
            , UserManager<User> userManager
            )
        {
            _tokenSvc = tokenSvc;
            _applicationDbContext = applicationDbContext;
            _userManager = userManager;
        }

        [HttpPost]/*Sign In - checked*/
        public async Task<IServiceResponse<TokenDTO>> Index([FromBody] LoginModel model)
        {//worked locally and online
            return await HandleApiOperationAsync(async () => {

                var response = new ServiceResponse<TokenDTO>();
                
                var user = await _userManager.FindByNameAsync(model.UserName)
                        ?? await _userManager.FindByEmailAsync(model.UserName);

                if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
                {

                    if (!user.IsConfirmed())
                    {

                        response.Code = ((int)HttpStatusCode.BadRequest).ToString();
                        response.ShortDescription = "Account not active. Please activate your acccount to continue.";
                        return response;
                    }

                    if (user.AccountLocked())
                    {
                        response.Code = ((int)HttpStatusCode.BadRequest).ToString();
                        response.ShortDescription = "Account locked. Please contact the system administrator.";
                        return response;
                    }

                    var userClaims = user.UserToClaims();

                    var token = _tokenSvc.GenerateAccessTokenFromClaims(userClaims.ToArray());

                    user.RefreshToken = token.RefreshToken;
                    await _userManager.UpdateAsync(user);

                    response.Object = token;
                }

                else
                 {
                    response.Code = ((int)HttpStatusCode.BadRequest).ToString();
                    response.ShortDescription = "Invalid credentials supplied.";
                }

                return response;
            });
        }

        [HttpPost]
        [Route("RefreshToken")]
        public async Task<IServiceResponse<TokenDTO>> Refresh(RefreshTokenModel model)
        {
            return await HandleApiOperationAsync(async () => {

                var response = new ServiceResponse<TokenDTO>();

                var principal = _tokenSvc.GetPrincipalFromExpiredToken(model.AccessToken);
                if (principal != null)
                {
                    var username = principal.FindFirst(JwtClaimTypes.Name).Value;

                    var user = await _userManager.FindByNameAsync(username);

                    if (user is null || user.RefreshToken != model.RefreshToken)
                    {
                        response.Code = ((int)HttpStatusCode.BadRequest).ToString();
                        response.ShortDescription = "Invalid token supplied.";
                        return response;
                    }

                    var userClaims = user.UserToClaims();

                    var token = _tokenSvc.GenerateAccessTokenFromClaims(userClaims.ToArray());

                    user.RefreshToken = token.RefreshToken;
                    await _userManager.UpdateAsync(user);

                    response.Object = token;

                    return response;
                }

                response.Code = ((int)HttpStatusCode.BadRequest).ToString();
                response.ShortDescription = "User is invalid.";
                return response;
            });
        }
    }
}
