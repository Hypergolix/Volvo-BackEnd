using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebTestApp.Services
{
    public interface ITaxCalculatorClient
    {
        int GetTax(IVehicle vehicle, DateTime[] dates);
    }
}
