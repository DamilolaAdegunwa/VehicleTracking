using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VehicleTracking.Core.Model
{
    public class Constants
    {
        public class Sections
        {
            public const string AuthJwtBearer = "Authentication:JwtBearer";
            public const string App = "App";
            public const string Smtp = "Smtp";
        }
        public static class Url
        {
            public const string WelcomeEmail = "wwwroot/messaging/emailtemplates/welcome-email.html";
        }
    }
}
