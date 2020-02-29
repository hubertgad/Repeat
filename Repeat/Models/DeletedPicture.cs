using System.ComponentModel.DataAnnotations.Schema;

namespace Repeat.Models
{
    public class DeletedPicture
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None), ForeignKey("DeletedQuestion")]
        public int ID { get; set; }
        public byte[] Data { get; set; }
        public DeletedQuestion DeletedQuestion { get; set; }

        public DeletedPicture()
        {
        }

        public DeletedPicture(Picture picture)
        {
            this.ID = picture.ID;
            this.Data = picture.Data;
        }
    }
}