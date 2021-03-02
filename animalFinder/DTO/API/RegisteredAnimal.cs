namespace animalFinder.DTO.API
{
    public class RegisteredAnimal
    {
        public string Id { get; set; }
        public string Image { get; set; }
        public string QR { get; set; }
        public string Name { get; set; }
        public Animal Animal { get; set; }
    }
}
