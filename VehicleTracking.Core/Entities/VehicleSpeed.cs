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
    public class VehicleSpeed : FullAuditedEntity
    {
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Speed { get; set; }
        [Required]
        public long VehicleId { get; set; }
    }
}
