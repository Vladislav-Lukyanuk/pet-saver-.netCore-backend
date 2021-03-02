using DAL.Enum;
using System;
using System.Collections.Generic;

namespace animalFinder.DTO.Service
{
    public class Animal
    {
        public Guid Id { get; set; }
        public string Image { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string PhoneNumber { get; set; }
        public DateTimeOffset UploadDate { get; set; }
        public Status Status { get; set; }
        public IEnumerable<Coordinate> Coordinates { get; set; }
        public string UserId { get; set; }
    }
}
