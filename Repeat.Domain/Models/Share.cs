using Repeat.Domain.SeedWork;

namespace Repeat.Domain.Models
{
    public class Share : IEntity
    {
        public int SetID { get; set; }
        public string UserID { get; set; }
        public Set Set { get; set; }
    }
}