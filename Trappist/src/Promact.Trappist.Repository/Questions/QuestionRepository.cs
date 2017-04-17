﻿using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Promact.Trappist.DomainModel.ApplicationClasses;
using Promact.Trappist.DomainModel.ApplicationClasses.Question;
using Promact.Trappist.DomainModel.DbContext;
using Promact.Trappist.DomainModel.Enum;
using Promact.Trappist.DomainModel.Models.Question;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Promact.Trappist.Repository.Questions
{
    public class QuestionRepository : IQuestionRepository
    {
        private readonly TrappistDbContext _dbContext;

        public QuestionRepository(TrappistDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> IsQuestionExistAsync(int questionId)
        {
            return await _dbContext.Question.AnyAsync(x => x.Id == questionId);
        }

        public async Task<ICollection<Question>> GetAllQuestionsAsync(string userId)
        {
            return (await _dbContext.Question.Where(u => u.CreatedByUserId.Equals(userId)).Include(x => x.Category).Include(x => x.CodeSnippetQuestion).Include(x => x.SingleMultipleAnswerQuestion).ThenInclude(x => x.SingleMultipleAnswerQuestionOption).OrderByDescending(g => g.CreatedDateTime).ToListAsync());
        }

        public async Task<QuestionAC> AddSingleMultipleAnswerQuestionAsync(QuestionAC questionAC, string userId)
        {
            var question = Mapper.Map<QuestionDetailAC, Question>(questionAC.Question);
            var singleMultipleAnswerQuestion = Mapper.Map<SingleMultipleAnswerQuestionAC, SingleMultipleAnswerQuestion>(questionAC.SingleMultipleAnswerQuestion);
            question.CreatedByUserId = userId;

            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                //Add common question details
                await _dbContext.Question.AddAsync(question);
                await _dbContext.SaveChangesAsync();

                //Add single/multiple question and option
                singleMultipleAnswerQuestion.Question = question;
                await _dbContext.SingleMultipleAnswerQuestion.AddAsync(singleMultipleAnswerQuestion);
                await _dbContext.SaveChangesAsync();
                transaction.Commit();
            }
            return (questionAC);
        }

        /// <summary>
        /// Adds new code snippet question to the Database
        /// </summary>
        /// <param name="questionAC">QuestionAC class object</param>
        /// <param name="userId">Id of logged in user</param>
        public async Task AddCodeSnippetQuestionAsync(QuestionAC questionAC, string userId)
        {
            var codeSnippetQuestion = Mapper.Map<CodeSnippetQuestionAC, CodeSnippetQuestion>(questionAC.CodeSnippetQuestion);
            var question = Mapper.Map<QuestionDetailAC, Question>(questionAC.Question);
            question.CreatedByUserId = userId;

            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                //Add common question details
                await _dbContext.Question.AddAsync(question);
                await _dbContext.SaveChangesAsync();

                //Add codeSnippet part of question
                codeSnippetQuestion.Question = question;
                codeSnippetQuestion.CodeSnippetQuestionTestCases = questionAC.CodeSnippetQuestion.TestCases;
                await _dbContext.CodeSnippetQuestion.AddAsync(codeSnippetQuestion);
                await _dbContext.SaveChangesAsync();
                var codingLanguages = await _dbContext.CodingLanguage.ToListAsync();

                //Map language to codeSnippetQuestion
                foreach (var language in questionAC.CodeSnippetQuestion.LanguageList)
                {
                    await _dbContext.QuestionLanguageMapping.AddAsync(new QuestionLanguageMapping
                    {
                        QuestionId = codeSnippetQuestion.Id,
                        LanguageId = codingLanguages.First(x => x.Language.ToLower().Equals(language.ToLower())).Id
                    });
                }
                await _dbContext.SaveChangesAsync();
                transaction.Commit();
            }
        }

        public async Task<ICollection<string>> GetAllCodingLanguagesAsync()
        {
            var codingLanguageList = await _dbContext.CodingLanguage.ToListAsync();

            var languageNameList = new List<string>();

            //Converting Enum value to string and adding it to languageNameList
            codingLanguageList.ForEach(codingLanguage =>
            {
                languageNameList.Add(codingLanguage.Language);
            });
            return languageNameList;
        }

        public async Task<QuestionAC> GetQuestionByIdAsync(int id)
        {
            var questionAC = new QuestionAC();

            var question = await _dbContext.Question
                .Include(x => x.SingleMultipleAnswerQuestion)
                .ThenInclude(x => x.SingleMultipleAnswerQuestionOption)
                .Include(x => x.CodeSnippetQuestion)
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync();

            questionAC.Question = Mapper.Map<Question, QuestionDetailAC>(question);
            questionAC.SingleMultipleAnswerQuestion = Mapper.Map<SingleMultipleAnswerQuestion, SingleMultipleAnswerQuestionAC>(question.SingleMultipleAnswerQuestion);
            questionAC.CodeSnippetQuestion = Mapper.Map<CodeSnippetQuestion, CodeSnippetQuestionAC>(question.CodeSnippetQuestion);

            if(question.QuestionType == QuestionType.Programming)
            {
                questionAC.CodeSnippetQuestion.LanguageList = await _dbContext
                    .QuestionLanguageMapping
                    .Include(x => x.CodeLanguage)
                    .Where(x => x.QuestionId == question.Id)
                    .Select(x => x.CodeLanguage.Language)
                    .ToListAsync();

                questionAC.CodeSnippetQuestion.TestCases = await _dbContext
                    .CodeSnippetQuestionTestCases
                    .Where(x => x.CodeSnippetQuestionId == question.Id)
                    .ToListAsync();
            }

            return questionAC;
        }

        public async Task UpdateSingleMultipleAnswerQuestionAsync(int questionId, QuestionAC questionAC, string userId)
        {
            var updatedQuestion = Mapper.Map<QuestionDetailAC, Question>(questionAC.Question);
            var options = questionAC.SingleMultipleAnswerQuestion.SingleMultipleAnswerQuestionOption;

            //Update common Question details
            updatedQuestion.Id = questionId;
            updatedQuestion.UpdatedByUserId = userId;
            _dbContext.Question.Update(updatedQuestion);
            await _dbContext.SaveChangesAsync();

            //Update single/multiple Question and option
            _dbContext.SingleMultipleAnswerQuestionOption.RemoveRange(await _dbContext.SingleMultipleAnswerQuestionOption.Where(x => x.SingleMultipleAnswerQuestionID == questionId).ToListAsync());
            await _dbContext.SaveChangesAsync();
            options.ForEach(x => x.SingleMultipleAnswerQuestionID = questionId);
            _dbContext.SingleMultipleAnswerQuestionOption.AddRange(options);
            await _dbContext.SaveChangesAsync();
        }
    }
}
