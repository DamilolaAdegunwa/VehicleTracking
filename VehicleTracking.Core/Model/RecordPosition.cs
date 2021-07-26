using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VehicleTracking.Core.Model
{
    public class RecordPositionRequest
    {
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public long VehicleId { get; set; }
    }
}
