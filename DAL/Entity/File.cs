using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Entity
{
    public class File
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public byte[] FileBytes { get; set; }
        public string Type { get; set; }
        public User User { get; set; }
        public string UserId { get; set; }
    }
}
