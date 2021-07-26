using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VehicleTracking.Core.Model
{
    public class DataAboutVehicleFuelResponse
    {
        public decimal Fuel { get; set; }//in gallons, litres etc.
        public long VehicleId { get; set; }
    }
}
