namespace Repeat.Models
{
    public class SetUser // #TODO: rename - Shares (?)
    {
        public int SetID { get; set; }
        public string UserID { get; set; }
        public Set Set { get; set; }
    }
}
