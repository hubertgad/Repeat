using Repeat.Domain.SeedWork;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Repeat.Domain.Models
{
    public class Category : Entity
    {
        [Required, MaxLength(50)]
        public string Name { get; set; }
        [Required]
        public string OwnerID { get; set; }
        public bool IsDeleted { get; set; }
        public ICollection<Question> Questions { get; set; }
    }
}