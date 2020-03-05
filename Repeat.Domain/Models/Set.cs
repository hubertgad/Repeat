using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Repeat.Models
{
    public class Set
    {
        public int ID { get; set; }
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
        [Required]
        public string OwnerID { get; set; }
        
        public virtual ICollection<QuestionSet> QuestionSets { get; set; }
        public virtual ICollection<SetUser> SetUsers { get; set; }
    }
}
