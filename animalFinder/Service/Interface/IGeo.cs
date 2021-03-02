namespace animalFinder.Service.Interface
{
    public interface IGeo
    {
        public double GetDistance(double point1Lat, double point1Lng, double point2Lat, double point2Lng);
    }
}
