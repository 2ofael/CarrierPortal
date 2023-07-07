using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;

namespace CarrierPortal.ViewModels
{
    public class AnswerViewModel
    {
        public string Id { get; set; }

        [BindRequired]
       
        public string Content { get; set; }

        public DateTime CreationDate { get; set; }

        public int Votes { get; set; }

        public bool IsApproved { get; set; }

        public string QuestionId { get; set; }

        public string UserId { get; set; }
    }
}
