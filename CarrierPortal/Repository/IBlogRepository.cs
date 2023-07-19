using CarrierPortal.Models.DataModel;

namespace CarrierPortal.Repository
{
    public interface IBlogRepository
    {
        Task CreatePostAsync(BlogPost post);
        Task DeletePostAsync(int id);
        Task<List<BlogPost>> GetAllPostsAsync();
        Task<BlogPost> GetPostByIdAsync(int id);
        Task UpdatePostAsync(BlogPost post);
        Task<List<BlogPost>> GetPostsAsync(string searchTerm, int page, int pageSize);
        Task<int> GetTotalPostsCountAsync(string searchTerm);

    }
}