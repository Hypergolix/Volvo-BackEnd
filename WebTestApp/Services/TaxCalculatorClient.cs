using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace WebTestApp.Services
{
    public class ShortTime
    {
        public int Hour { get; set; }
        public int Minute { get; set; }
    }

    public class TimeFee
    {
        public ShortTime From { get; set; } = new ShortTime();
        public ShortTime To { get; set; } = new ShortTime();
        public int Fee { get; set; }
    }

    public class TaxCalculatorClient : ITaxCalculatorClient
    {
        public int GetTax(IVehicle vehicle, DateTime[] dates)
        {
            Array.Sort(dates);
            DateTime intervalStart = dates[0];
            int totalFee = 0;
            for (int i = 0; i < dates.Length; i++)
            {
                int nextFee = GetTollFee(dates[i], vehicle);
                int tempFee = GetTollFee(intervalStart, vehicle);

                long diffInMinutes = (long)dates[i].Subtract(intervalStart).TotalMinutes;

                if (diffInMinutes <= 60)
                {
                    if (totalFee > 0) totalFee -= tempFee;
                    if (nextFee >= tempFee) tempFee = nextFee;
                    totalFee += tempFee;
                }
                else
                {
                    totalFee += nextFee;
                    intervalStart = dates[i];
                }
            }
            if (totalFee > 60) totalFee = 60;
            return totalFee;
        }

        private bool IsTollFreeVehicle(IVehicle vehicle)
        {
            if (vehicle == null) return false;
            string vehicleType = vehicle.GetVehicleType();
            return vehicleType.Equals(TollFreeVehicles.Motorbike.ToString()) ||
                   vehicleType.Equals(TollFreeVehicles.Bus.ToString()) ||
                   vehicleType.Equals(TollFreeVehicles.Emergency.ToString()) ||
                   vehicleType.Equals(TollFreeVehicles.Diplomat.ToString()) ||
                   vehicleType.Equals(TollFreeVehicles.Foreign.ToString()) ||
                   vehicleType.Equals(TollFreeVehicles.Military.ToString());
        }

        private List<TimeFee> ParseCSV()
        {
            //string path = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName;
            string path = Environment.CurrentDirectory;
            path += "\\Data\\Fees.csv";

            List<TimeFee> timeFees = new List<TimeFee>();
            var file = File.ReadAllLines(path);

            foreach (string line in file.Skip(1))
            {
                var splitLine = line.Split(',');
                var parsedFrom = splitLine[0].Split(':');

                TimeFee timeFee = new TimeFee();
                timeFee.From.Hour = int.Parse(parsedFrom[0]);
                timeFee.From.Minute = int.Parse(parsedFrom[1]);

                var parsedTo = splitLine[1].Split(':');
                timeFee.To.Hour = int.Parse(parsedTo[0]);
                timeFee.To.Minute = int.Parse(parsedTo[1]);

                timeFee.Fee = int.Parse(splitLine[2]);

                timeFees.Add(timeFee);
            }
            return timeFees;
        }

        private int GetTollFee(DateTime date, IVehicle vehicle)
        {
            if (IsTollFreeDate(date) || IsTollFreeVehicle(vehicle)) return 0;

            int hour = date.Hour;
            int minute = date.Minute;

            var timeFees = new List<TimeFee>(ParseCSV());

            foreach (var timeFee in timeFees)
            {
                if (timeFee.From.Hour - timeFee.To.Hour == 0)
                {
                    if (hour == timeFee.From.Hour && minute >= timeFee.From.Minute && minute <= timeFee.To.Minute)
                        return timeFee.Fee;
                }
                else
                {
                    if (hour >= timeFee.From.Hour && hour <= timeFee.To.Hour && minute >= timeFee.From.Minute && minute <= timeFee.To.Minute)
                        return timeFee.Fee;
                }
            }
            return 0;
        }

        private bool IsTollFreeDate(DateTime date)
        {
            int year = date.Year;
            int month = date.Month;
            int day = date.Day;

            if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday) return true;

            if (year == 2013)
            {
                if (month == 1 && day == 1 ||
                    month == 3 && (day == 28 || day == 29) ||
                    month == 4 && (day == 1 || day == 30) ||
                    month == 5 && (day == 1 || day == 8 || day == 9) ||
                    month == 6 && (day == 5 || day == 6 || day == 21) ||
                    month == 7 ||
                    month == 11 && day == 1 ||
                    month == 12 && (day == 24 || day == 25 || day == 26 || day == 31))
                {
                    return true;
                }
            }
            return false;
        }

        private enum TollFreeVehicles
        {
            Motorbike = 0,
            Bus = 1,
            Emergency = 2,
            Diplomat = 3,
            Foreign = 4,
            Military = 5
        }
    }
}