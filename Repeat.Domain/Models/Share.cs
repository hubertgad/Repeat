using Repeat.Domain.SeedWork;
using System.ComponentModel.DataAnnotations;

namespace Repeat.Domain.Models
{
    public class Share : IEntity
    {
        [Required]
        public int SetId { get; set; }
        [Required]
        public string UserId { get; set; }
        public Set Set { get; set; }
        public ApplicationUser User { get; set; }
    }
}