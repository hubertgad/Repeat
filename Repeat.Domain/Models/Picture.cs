using Repeat.Domain.SeedWork;
using System.ComponentModel.DataAnnotations.Schema;

namespace Repeat.Domain.Models
{
    public class Picture : IEntity
    {
        [ForeignKey("Question")]
        public int ID { get; set; }
        public byte[] Data { get; set; }        
        public Question Question { get; set; }
    }
}