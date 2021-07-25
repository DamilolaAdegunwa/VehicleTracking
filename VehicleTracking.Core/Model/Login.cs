using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VehicleTracking.Core.Entities.EnumeratedTypes;

namespace VehicleTracking.Core.Model
{
    public class LoginModel
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public UserType UserType { get; set; }
    }


    public class RefreshTokenModel
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
    public class PassordResetDTO
    {
        public string UserName { get; set; }
        public string Code { get; set; }
        public string NewPassword { get; set; }
    }


    public class ChangePassordDTO
    {
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
    }
}
