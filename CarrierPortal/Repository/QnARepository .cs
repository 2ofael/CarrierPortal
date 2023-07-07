using CarrierPortal.Models;
using CarrierPortal.Models.DataModel;
using Microsoft.EntityFrameworkCore;

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
                .Include(q => q.User).Include(q=>q.Answers)
                .ToListAsync();
        }

        public async Task<Question> GetQuestionByIdAsync(string questionId)
        {
            return await _context.Questions
                .Include(q => q.User)
                .FirstOrDefaultAsync(q => q.Id == questionId);
        }

        public async Task<List<Answer>> GetAnswersForQuestionAsync(string questionId)
        {
            return await _context.Answers
                .Where(a => a.QuestionId == questionId)
                .Include(a => a.User)
                .ToListAsync();
        }

        public async Task<Answer> GetAnswerByIdAsync(string answerId)
        {
            return await _context.Answers
                .Include(a => a.User)
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




    }
}
