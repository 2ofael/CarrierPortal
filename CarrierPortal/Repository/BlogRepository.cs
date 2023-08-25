using CarrierPortal.Models;
using CarrierPortal.Models.DataModel;
using Microsoft.EntityFrameworkCore;

namespace CarrierPortal.Repository
{
    public class BlogRepository : IBlogRepository
    {
        private readonly AppDbContext _dbContext;

        public BlogRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<BlogPost>> GetAllPostsAsync()
        {
            return await _dbContext.BlogPosts.ToListAsync();
        }

        public async Task<BlogPost> GetPostByIdAsync(int id)
        {
            return await _dbContext.BlogPosts.Include(b=>b.Loved).Include(b=>b.ApplicationUser).Include(b=>b.ApplicationUser.Mentor).FirstOrDefaultAsync (p => p.Id == id);
        }

        public async Task CreatePostAsync(BlogPost post)
        {
            _dbContext.BlogPosts.Add(post);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdatePostAsync(BlogPost post)
        {
            _dbContext.Entry(post).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeletePostAsync(int id)
        {
            var post = await _dbContext.BlogPosts.FindAsync(id);
            if (post != null)
            {
                _dbContext.BlogPosts.Remove(post);
                await _dbContext.SaveChangesAsync();
            }
        }
        public async Task<List<BlogPost>> GetPostsAsync(string searchTerm, int page, int pageSize)
        {
   
            var query = _dbContext.BlogPosts.Include(b=>b.Loved).AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(b => b.Title.Contains(searchTerm) || b.Content.Contains(searchTerm));
            }

            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            // Validate page to ensure it's within the range of 1 to totalPages
           // page = Math.Max(1, Math.Min(page, totalPages));

            // Calculate the correct skip value to ensure it's non-negative
            var skip = (page - 1) * pageSize;
            //skip = Math.Max(0, skip);

            if(!query.Any())
            {
                return new List<BlogPost>();
            }

            return await query
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> GetTotalPostsCountAsync(string searchTerm)
        {
            var query = _dbContext.BlogPosts.Include(b=>b.Loved).AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(b => b.Title.Contains(searchTerm) || b.Content.Contains(searchTerm));
            }

            return await query.CountAsync();
        }
        //public async Task CreateBlogPostVoteAsync(BlogPostVote vote)
        //{
        //    _dbContext.BlogPostVotes.Add(vote);
        //    await _dbContext.SaveChangesAsync();
        //}

        //public async Task<BlogPostVote> GetBlogPostVoteAsync(string userId, int blogPostId)
        //{
        //    return await _dbContext.BlogPostVotes
        //        .FirstOrDefaultAsync(v => v.UserId == userId && v.BlogPostId == blogPostId);
        //}

        //public async Task CreatePostAsync(BlogPost blogPost)
        //{
        //    _dbContext.BlogPosts.Add(blogPost);
        //    await _dbContext.SaveChangesAsync();
        //}


    }

}

