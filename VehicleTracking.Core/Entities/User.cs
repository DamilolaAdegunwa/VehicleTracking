using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using VehicleTracking.Core.Entities.EnumeratedTypes;

namespace VehicleTracking.Core.Entities
{
    public class User : IdentityUser<long>
    {
        public string RefreshToken { get; set; }
        public string AccountConfirmationCode { get; set; }
        public UserType UserType { get; set; }
    }
    public static class UserExtensions
    {

        public static bool IsNull(this User user)
        {
            return user == null;
        }

        public static bool IsConfirmed(this User user)
        {
            return user.EmailConfirmed || user.PhoneNumberConfirmed;
        }

        public static bool AccountLocked(this User user)
        {
            return user.LockoutEnabled == true;
        }

        public static bool HasNoPassword(this User user)
        {
            return string.IsNullOrWhiteSpace(user.PasswordHash);
        }
    }
}
