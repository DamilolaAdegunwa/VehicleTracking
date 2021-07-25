using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VehicleTracking.Core.Entities.Auditing;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VehicleTracking.Core.Entities
{
    public class Vehicle : FullAuditedEntity
    {
        [Required]
        public string RegistrationNumber { get; set; }
    }
}
