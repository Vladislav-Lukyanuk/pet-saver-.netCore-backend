using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Entity
{
    public class RegisteredAnimal
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
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
