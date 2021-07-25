using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IdentityModel;
using System.Collections.Generic;
using System.Security.Claims;
using VehicleTracking.Core.Entities;

namespace VehicleTracking.Services.Extensions
{
    public static class ClaimsExtensions
    {
        public static List<Claim> UserToClaims(this User user)
        {
            //These wont be null
            var claims = new List<Claim>
            {
                new Claim(JwtClaimTypes.Id, user.Id.ToString()),
                new Claim(JwtClaimTypes.Name, user.UserName),
                new Claim(JwtClaimTypes.Email, user.Email),
            };
            return claims;
        }
    }
}
