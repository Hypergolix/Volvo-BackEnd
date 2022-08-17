using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebTestApp.Services
{
    public class Bus : IVehicle
    {
        public string GetVehicleType() => "Bus";
    }
}