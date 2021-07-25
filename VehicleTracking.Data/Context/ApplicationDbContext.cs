using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VehicleTracking.Core.Entities;

namespace VehicleTracking.Data.Context
{
    public class ApplicationDbContext : IdentityDbContext<User, Role, long>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) :base (options) 
        {
            
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<VehiclePosition> VehiclePositions { get; set; }
        public DbSet<VehicleFuel> VehicleFuel { get; set; }
        public DbSet<VehicleSpeed> VehicleSpeed { get; set; }
    }
}
