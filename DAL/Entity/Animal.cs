using DAL.Enum;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace DAL.Entity
{
    public class Animal
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id {get; set;}
        public string Image { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTimeOffset UploadDate { get; set; }
        public Status Status { get; set; }
        public User User { get; set; }
        public string UserId { get; set; }
        public ICollection<Coordinate> Coordinates { get; set; }
    }
}