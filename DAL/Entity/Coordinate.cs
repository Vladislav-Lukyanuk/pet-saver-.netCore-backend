using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Entity
{
    public class Coordinate
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public float Latitude { get; set; }
        public float Longitude { get; set; }
        public Guid AnimalId { get; set; }
    }
}
