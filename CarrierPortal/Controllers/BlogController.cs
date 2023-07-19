using CarrierPortal.Models;
using CarrierPortal.Models.DataModel;
using CarrierPortal.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data;
using System.Security.Claims;
using Microsoft.AspNetCore;

namespace CarrierPortal.Controllers
{
    public class BlogController : Controller
    {
        private readonly IBlogRepository _blogRepository;

        public BlogController(IBlogRepository blogRepository)
        {
            _blogRepository = blogRepository;
        }

        public async Task<IActionResult> Index(string searchTerm, int page = 1)
        {
            // Set the search term in ViewBag to display in the search bar
            ViewBag.SearchTerm = searchTerm;

            // Get all blog posts paginated
            var pageSize = 10; // Number of items per page
            var totalItems = await _blogRepository.GetTotalPostsCountAsync(searchTerm);
            var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            if (page < 1)
            {
                page = 1;
            }
            else if (page > totalPages)
            {
                page = totalPages;
            }

            var blogPosts = await _blogRepository.GetPostsAsync(searchTerm, page, pageSize);

            // Create a PaginatedList instance
            var paginatedList = new PaginatedList<BlogPost>(blogPosts, page, pageSize, totalItems, totalPages);

            return View(paginatedList);
        }
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blogPost = await _blogRepository.GetPostByIdAsync(id.Value);

            if (blogPost == null)
            {
                return NotFound();
            }

            return View(blogPost);
        }

        // GET: BlogPosts/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: BlogPosts/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Content")] BlogPost blogPost)
        {
            if (blogPost.Title!=null && blogPost.Content!=null)
            {
                // Set the current user's ID as the ApplicationUserId
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                blogPost.ApplicationUserId = userId;

                blogPost.CreatedAt = DateTime.Now; // Set the creation date

                // Save the new blog post
                await _blogRepository.CreatePostAsync(blogPost);

                return RedirectToAction(nameof(Index));
            }

            return View(blogPost);
        }
        // GET: BlogPosts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blogPost = await _blogRepository.GetPostByIdAsync(id.Value);
  
            if (blogPost == null)
            {
                return NotFound();
            }
            return View(blogPost);
        }

        // POST: BlogPosts/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Content,CreatedAt")] BlogPost blogPost)
        {
            //if (id != blogPost.Id)
            //{
            //    return NotFound();
            //}

            if (blogPost.Id!=null && blogPost.Title!=null && blogPost.Content!=null)
            {
                
                var PrevBlog = await _blogRepository.GetPostByIdAsync(blogPost.Id);
                PrevBlog.Title = blogPost.Title;
                PrevBlog.Content = blogPost.Content;

                await _blogRepository.UpdatePostAsync(PrevBlog);
                return RedirectToAction(nameof(Details), new { id = blogPost.Id });
            }
            return View(blogPost);
        }

        // GET: BlogPosts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blogPost = await _blogRepository.GetPostByIdAsync(id.Value);
            if (blogPost == null)
            {
                return NotFound();
            }

            return View(blogPost);
        }

        // POST: BlogPosts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _blogRepository.DeletePostAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
