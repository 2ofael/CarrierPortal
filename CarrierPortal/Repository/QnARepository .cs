using CarrierPortal.Models;
using CarrierPortal.Models.DataModel;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

namespace CarrierPortal.Repository
{
    public class QnARepository : IQnARepository
    {
        private readonly AppDbContext _context;

        public QnARepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Question>> GetAllQuestionsAsync()
        {
            return await _context.Questions
                .Include(q => q.User).Include(q=>q.Answers).Include(q=>q.QuestionVotes)
                .ToListAsync();
        }

        public async Task<Question> GetQuestionByIdAsync(string questionId)
        {
            return await _context.Questions
                .Include(q => q.User).Include(q => q.QuestionVotes)
                .FirstOrDefaultAsync(q => q.Id == questionId);
        }

        public async Task<List<Answer>> GetAnswersForQuestionAsync(string questionId)
        {
            return await _context.Answers
                .Where(a => a.QuestionId == questionId)
                .Include(a => a.User)
                .Include(a=>a.AnswerVotes)
                .ToListAsync();
        }

        public async Task<Answer> GetAnswerByIdAsync(string answerId)
        {
            return await _context.Answers
                .Include(a => a.User)
                .Include(a=>a.AnswerVotes)
                .FirstOrDefaultAsync(a => a.Id == answerId);
        }

        public async Task CreateQuestionAsync(Question question)
        {
            question.CreationDate = DateTime.Now;
            question.Votes = 0;
            question.IsApproved = false;
            _context.Questions.Add(question);
            await _context.SaveChangesAsync();
        }

        public async Task CreateAnswerAsync(Answer answer)
        {
            answer.CreationDate = DateTime.Now;
            answer.Votes = 0;
            answer.IsApproved = false;
            _context.Answers.Add(answer);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateQuestionAsync(Question question)
        {
            _context.Questions.Update(question);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAnswerAsync(Answer answer)
        {
            _context.Answers.Update(answer);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteQuestionAsync(Question question)
        {
            _context.Questions.Remove(question);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAnswerAsync(Answer answer)
        {
            _context.Answers.Remove(answer);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Question>> GetPostsAsync(string searchTerm, int page, int pageSize)
        {
            var query = _context.Questions.Include(a=>a.Answers).Include(q=>q.User).AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(b => b.Title.Contains(searchTerm) || b.Content.Contains(searchTerm));
            }

            var skip = (page - 1) * pageSize;

            if (!query.Any())
            {
                return new List<Question>();
            }

            return await query.OrderByDescending(b => b.CreationDate)
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> GetTotalPostsCountAsync(string searchTerm)
        {
            var query = _context.Questions.Include(q=>q.Answers).Include(q=>q.User).AsQueryable();


            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(b => b.Title.Contains(searchTerm) || b.Content.Contains(searchTerm));
            }

            return await query.CountAsync();
        }

        public async Task CreateQuestionVoteAsync(QuestionVote vote)
        {
            vote.Id = Guid.NewGuid().ToString();
            _context.QuestionVotes.Add(vote);
            await _context.SaveChangesAsync();
        }

        public async Task CreateAnswerVoteAsync(AnswerVote vote)
        {
            vote.Id = Guid.NewGuid().ToString();
            _context.AnswerVotes.Add(vote);
            await _context.SaveChangesAsync();
        }

        public bool HasUserVotedQuestion(string userId, string questionId)
        {
            return _context.QuestionVotes.Any(v => v.UserId == userId && v.QuestionId == questionId);
        }

        public bool HasUserVotedAnswer(string userId, string answerId)
        {
            return _context.AnswerVotes.Any(v => v.UserId == userId && v.AnswerId == answerId);
        }

        public async Task<List<Answer>> GetAllAnswersAsync()
        {
            return await _context.Answers.ToListAsync(); 
        }

    }
}
