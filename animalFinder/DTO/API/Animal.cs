namespace animalFinder.DTO.API
{
    public class Animal
    {
        public string Id { get; set; }
        public string Image { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string PhoneNumber { get; set; }
        public Coordinate[] Coordinates { get; set; }
    }
}
