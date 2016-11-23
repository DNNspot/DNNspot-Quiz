using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DNNspot.Quiz.Model
{
    public class QuizResult
    {
        public Quiz Quiz { get; set; }
        public int PercentScore { get; set; }
        public bool IsPassingScore { get; set; }
        public List<QuizAction> Actions { get; set; }

        public QuizResult()
        {
            Actions = new List<QuizAction>();
        }
    }
}