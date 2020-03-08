using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Repeat.Models
{
    public class Question
    {
        public int ID { get; set; }
        [Required, MaxLength(1000), Display(Name = "Question Text")]
        public string QuestionText { get; set; }
        [MaxLength(1000)]
        public string Code { get; set; }
        [Display(Name = "Category ID")]
        public int CategoryID { get; set; }
        [Required]
        public string OwnerID { get; set; }
        [MaxLength(1000)]
        public string Reference { get; set; }
        public bool IsDeleted { get; set; }
        public Picture Picture { get; set; }
        public Category Category { get; set; }
        public List<Answer> Answers { get; set; }
        public virtual List<QuestionSet> QuestionSets { get; set; }

        public Question()
        {
        }

        public Question(DeletedQuestion deletedQuestion)
        {
            this.ID = deletedQuestion.ID;
            this.QuestionText = deletedQuestion.QuestionText;
            this.Code = deletedQuestion.Code;
            this.CategoryID = deletedQuestion.CategoryID;
            this.OwnerID = deletedQuestion.OwnerID;
            this.Reference = deletedQuestion.Reference;
            if (deletedQuestion.Picture != null) this.Picture = deletedQuestion.Picture;
            if (deletedQuestion.DeletedAnswers != null)
            {
                this.Answers = new List<Answer>();
                foreach (var deletedAnswer in deletedQuestion.DeletedAnswers)
                {
                    this.Answers.Add(new Answer(deletedAnswer));
                }
            }
            this.IsDeleted = true;
        }
    }
}
