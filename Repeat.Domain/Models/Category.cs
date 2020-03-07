using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Repeat.Models
{
    public class Category
    {
        public int ID { get; set; }
        [Required, MaxLength(50)]
        public string Name { get; set; }
        [Required]
        public string OwnerID { get; set; }
        public ICollection<Question> Questions { get; set; }
    }
}