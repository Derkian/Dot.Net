using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TotalLoss.Domain.Model
{
    public class Question
    {
        [Required]
        public int Id { get; set; }        

        public bool Answer { get; set; } = false;

        public string Label { get; set; }

        public QuestionType Type { get; set; }

        [JsonIgnore]
        public int Point { get; set; }

        [JsonIgnore]
        public bool Enable { get; set; }
    }

    public enum QuestionType
    {
        Default = 0,
        Button = 1
    }
}
