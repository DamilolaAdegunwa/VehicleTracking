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
    public class VehiclePosition:  FullAuditedEntity
    {
        [Required]
        [Column(TypeName = "decimal(18,15)")]
        public decimal Latitude { get; set; }//-90 to 90 (N+/S-)
        [Required]
        [Column(TypeName = "decimal(18,15)")]
        public decimal Longitude { get; set; }//-180 to 180 (E+/W-)
        [Required]
        public long VehicleId { get; set; }
    }
}
