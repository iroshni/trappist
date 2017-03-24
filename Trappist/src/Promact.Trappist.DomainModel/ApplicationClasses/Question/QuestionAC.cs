﻿using Promact.Trappist.DomainModel.Models.Question;

namespace Promact.Trappist.DomainModel.ApplicationClasses.Question
{
    public class QuestionAC
    {
        public Models.Question.Question Question { get; set; }
        public SingleMultipleAnswerQuestionAC SingleMultipleAnswerQuestionAC { get; set; }
        public CodeSnippetQuestion CodeSnippetQuestion { get; set; }
    }
}
