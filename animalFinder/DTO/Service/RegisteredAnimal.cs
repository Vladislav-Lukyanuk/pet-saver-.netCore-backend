using DAL.Entity;
using System;

namespace animalFinder.DTO.Service
{
    public class RegisteredAnimal
    {
        public Guid Id { get; set; }
        public string Image { get; set; }
        public string QR { get; set; }
        public string Name { get; set; }
        public DateTimeOffset UploadDate { get; set; }
        public Animal Animal { get; set; }
        public Guid? AnimalId { get; set; }
        public User User { get; set; }
        public string UserId { get; set; }
    }
}
