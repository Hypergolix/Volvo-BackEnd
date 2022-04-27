using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebTestApp.Services;

namespace WebTestApp.Controllers
{
    [Route("api")]
    [ApiController]
    public class TaxController : ControllerBase
    {
        private readonly ITaxCalculatorClient _client;

        public TaxController()
        {
            _client = new TaxCalculatorClient();
        }

        [HttpGet]
        public ActionResult<int> Get(string vehicle, string[] dates)
        {
            if (dates.Length == 0) return BadRequest();
            IVehicle parsedVehicle;
            switch(vehicle.ToLower())
            {
                case "motorbike":
                    parsedVehicle = new Motorbike();
                    break;
                case "car":
                    parsedVehicle = new Car();
                    break;
                default:
                    return BadRequest();
            }

            List<DateTime> parsedDates = new List<DateTime>();
            foreach (var date in dates)
            {
                DateTime dateTime = new DateTime();
                if (DateTime.TryParse(date, out dateTime))
                {
                    parsedDates.Add(dateTime);
                }
            }
            return _client.GetTax(parsedVehicle, parsedDates.ToArray());
        }
    }
}
