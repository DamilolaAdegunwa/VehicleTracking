using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VehicleTracking.Core.Model
{
    public class PositionOfaVehicleResponse
    {
        public decimal Latitude { get; set; }//-90 to 90 (N+/S-)
        public decimal Longitude { get; set; }//-180 to 180 (E+/W-)
        public long VehicleId { get; set; }
        public string Address { get; set; }
    }
}
