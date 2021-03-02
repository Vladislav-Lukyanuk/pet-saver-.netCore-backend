using animalFinder.Service.Interface;
using System;

namespace animalFinder.Service
{
    public class Geo : IGeo
    {
        private const short EARTH_RADIUS = 6371;
        public double GetDistance(double point1Lat, double point1Lng, double point2Lat, double point2Lng)
        {
            double dLat = point2Lat - point1Lat;
            double dLng = point2Lng - point1Lng;
            double dLatInRadians = ToRadians(dLat);
            double dLngInRadians = ToRadians(dLng);
            double a =
                Math.Sin(dLatInRadians / 2) * Math.Sin(dLatInRadians / 2) +
                Math.Cos(ToRadians(point1Lat)) * Math.Cos(ToRadians(point2Lat)) *
                Math.Sin(dLngInRadians / 2) * Math.Sin(dLngInRadians / 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            double d = EARTH_RADIUS * c;
            
            return d;

        }

        private double ToRadians(double degrees)
        {
            return degrees * Math.PI / 180;
        }
    }
}
