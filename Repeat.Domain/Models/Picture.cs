using System.ComponentModel.DataAnnotations.Schema;

namespace Repeat.Models
{
    public class Picture
    {
        [ForeignKey("Question")]
        public int ID { get; set; }
        public byte[] Data { get; set; }        
        public Question Question { get; set; }

        public Picture()
        {
        }

        public Picture(DeletedPicture deletedPicture)
        {
            this.ID = deletedPicture.ID;
            this.Data = deletedPicture.Data;
        }
    }
}
