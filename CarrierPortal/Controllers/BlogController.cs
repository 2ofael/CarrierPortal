using CarrierPortal.Models;
using CarrierPortal.Models.DataModel;
using CarrierPortal.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data;
using System.Security.Claims;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

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
                TempData["isCreated"] = true;

                return RedirectToAction(nameof(Details), new {id = blogPost.Id});
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
                TempData["isEdited"] = true;
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
            TempData["isDeleted"] = true;
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> ApproveBlog(int Id)
        {
            var ans = await _blogRepository.GetPostByIdAsync(Id);
            if (ans == null)
            {
                return NotFound();
            }

            ans.IsApproved = true;
            await _blogRepository.UpdatePostAsync(ans);

            // Redirect back to the ActorsList action after approval
            TempData["isApproved"] = true;
            return RedirectToAction( nameof(Details), new {id = Id});

        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //[Authorize]
        //public async Task<IActionResult> UpvoteBlogPost(int id)
        //{
        //    // Check if the user is authenticated
        //    if (!User.Identity.IsAuthenticated)
        //    {
        //        return RedirectToAction("Login", "Account"); // Redirect to login page or display an error message
        //    }

        //    var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        //    var blogPost = await _blogRepository.GetPostByIdAsync(id);

        //    if (blogPost == null)
        //    {
        //        return NotFound();
        //    }

        //    var existingVote = await _blogRepository.GetBlogPostVoteAsync(userId, id);

        //    if (existingVote == null)
        //    {
        //        // Create a new upvote
        //        var newVote = new BlogPostVote
        //        {
        //            UserId = userId,
        //            BlogPostId = id,
        //            IsUpvote = true
        //        };

        //        blogPost.Votes++;
        //        await _blogRepository.CreateBlogPostVoteAsync(newVote);
        //        await _blogRepository.UpdatePostAsync(blogPost);
        //    }
        //    else if (!existingVote.IsUpvote)
        //    {
        //        // Switch from downvote to upvote
        //        existingVote.IsUpvote = true;
        //        blogPost.Votes += 2; // Increment by 2 since we're changing from downvote to upvote
        //        await _blogRepository.UpdatePostAsync(blogPost);
        //    }

        //    return RedirectToAction("Details", new { id = id });
        //}

        //// POST: Blog/DownvoteBlogPost/5
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //[Authorize]
        //public async Task<IActionResult> DownvoteBlogPost(int id)
        //{
        //    // Check if the user is authenticated
        //    if (!User.Identity.IsAuthenticated)
        //    {
        //        return RedirectToAction("Login", "Account"); // Redirect to login page or display an error message
        //    }

        //    var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        //    var blogPost = await _blogRepository.GetPostByIdAsync(id);

        //    if (blogPost == null)
        //    {
        //        return NotFound();
        //    }

        //    var existingVote = await _blogRepository.GetBlogPostVoteAsync(userId, id);

        //    if (existingVote == null)
        //    {
        //        // Create a new downvote
        //        var newVote = new BlogPostVote
        //        {
        //            UserId = userId,
        //            BlogPostId = id,
        //            IsUpvote = false
        //        };

        //        blogPost.Votes--;
        //        await _blogRepository.CreateBlogPostVoteAsync(newVote);
        //        await _blogRepository.UpdatePostAsync(blogPost);
        //    }
        //    else if (existingVote.IsUpvote)
        //    {
        //        // Switch from upvote to downvote
        //        existingVote.IsUpvote = false;
        //        blogPost.Votes -= 2; // Decrement by 2 since we're changing from upvote to downvote
        //        await _blogRepository.UpdatePostAsync(blogPost);
        //    }

        //    return RedirectToAction("Details", new { id = id });
        //}


        [HttpPost]
        public async Task<IActionResult> LovePost(int postId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Implement a method to get the current logged-in user's Id
            var post = await _blogRepository.GetPostByIdAsync(postId);
            var isLoved = post.Loved.Select(l => l.UserNameIdentifier == userId).FirstOrDefault();
            if (post != null)
            {
                if (isLoved)
                {
                    var curLoved = post.Loved.Select(l => l).Where(l => l.UserNameIdentifier == userId).FirstOrDefault();
                    post.Loved.Remove(curLoved); // Remove the user's Id from the Loved list
                    post.Votes--; // Decrease the total number of votes/loves for the post
                }
                else
                {
                    post.Loved.Add(new Love { UserNameIdentifier= userId}); // Add the user's Id to the Loved list
                    post.Votes++; // Increment the total number of votes/loves for the post
                }

               await  _blogRepository.UpdatePostAsync(post);
            }

            return RedirectToAction("Details", new { id = postId });
        }


    }
}
