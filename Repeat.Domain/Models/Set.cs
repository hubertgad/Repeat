using Repeat.Domain.SeedWork;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Repeat.Domain.Models
{
    public class Set : Entity
    {
        [Required, MaxLength(50)]
        public string Name { get; set; }
        [Required]
        public string OwnerID { get; set; }
        public virtual HashSet<QuestionSet> QuestionSets { get; set; }
        public virtual HashSet<Share> Shares { get; set; }
    }
}