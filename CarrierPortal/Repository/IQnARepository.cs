﻿using CarrierPortal.Models.DataModel;

namespace CarrierPortal.Repository
{
    public interface IQnARepository
    {
        Task CreateAnswerAsync(Answer answer);
        Task CreateQuestionAsync(Question question);
        Task DeleteAnswerAsync(Answer answer);
        Task DeleteQuestionAsync(Question question);
        Task<List<Question>> GetAllQuestionsAsync();
        Task<List<Answer>> GetAllAnswersAsync();
        Task<Answer> GetAnswerByIdAsync(string answerId);
        Task<List<Answer>> GetAnswersForQuestionAsync(string questionId);
        Task<Question> GetQuestionByIdAsync(string questionId);
        Task UpdateAnswerAsync(Answer answer);
        Task UpdateQuestionAsync(Question question);
        Task<List<Question>>GetPostsAsync(string searchTerm, int page, int pageSize);
        Task<int> GetTotalPostsCountAsync(string searchTerm);
        Task CreateQuestionVoteAsync(QuestionVote vote);
        Task CreateAnswerVoteAsync(AnswerVote vote);
        bool HasUserVotedQuestion(string userId, string questionId);
        bool HasUserVotedAnswer(string userId, string answerId);
    }
}