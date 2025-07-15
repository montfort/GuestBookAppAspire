using System;
using System.ComponentModel.DataAnnotations;

namespace GuestBookApp.Core.Models
{
    public class GuestBookEntry
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime Date { get; set; } = DateTime.UtcNow;
        public string Comment { get; set; } = string.Empty;
    }

}