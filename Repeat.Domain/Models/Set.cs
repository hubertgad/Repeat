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
        public string OwnerId { get; set; }
        public ApplicationUser Owner { get; set; }
        public ISet<QuestionSet> QuestionSets { get; set; }
        public ISet<Share> Shares { get; set; }
        public bool IsPublic { get; set; }
    }
}