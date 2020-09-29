using Repeat.Domain.SeedWork;
using System.ComponentModel.DataAnnotations;

namespace Repeat.Domain.Models
{
    public class Share : IEntity
    {
        [Required]
        public int SetID { get; set; }
        [Required]
        public string UserID { get; set; }
        public Set Set { get; set; }
        public ApplicationUser User { get; set; }
    }
}