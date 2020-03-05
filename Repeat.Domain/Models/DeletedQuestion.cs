using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Repeat.Models
{
    public class DeletedQuestion
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ID { get; set; }
        [Required, MaxLength(1000), Display(Name = "Question Text")]
        public string QuestionText { get; set; }
        [MaxLength(1000)]
        public string Code { get; set; }
        [Required, Display(Name = "Category ID"), NotMapped]
        public int CategoryID { get; set; }
        [Required]
        public string OwnerID { get; set; }
        public string Reference { get; set; }
        public Picture Picture { get; set; }
        public Category Category { get; set; }
        public List<DeletedAnswer> DeletedAnswers { get; set; }

        public DeletedQuestion()
        {
        }

        public DeletedQuestion(Question question)
        {
            this.ID = question.ID;
            this.QuestionText = question.QuestionText;
            this.Code = question.Code;
            this.CategoryID = question.CategoryID;
            this.OwnerID = question.OwnerID;
            this.Reference = question.Reference;
            if (question.Picture != null) this.Picture = question.Picture;
        }
    }
}