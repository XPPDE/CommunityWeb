using DbClassLibrary.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CommunityWeb.Pages
{
    public class PostDetailsModel(AppDbContext context) : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public int Id { get; set; } = new();
        [BindProperty]
        public PostComment NewComment { get; set; } = new();
        public Post? Post { get; set; }
        public List<PostComment>? PostComments { get; set; }
        public bool IsOwner { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var post = await context.Posts.FirstOrDefaultAsync(p => p.Id == Id);
            if (post == null) return NotFound();

            Post = post;

            var currentUserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (currentUserIdClaim != null)
            {
                IsOwner = int.Parse(currentUserIdClaim.Value) == id;
            }

            PostComments = await context.PostComments
                .Include(c => c.User)
                .Where(p => p.PostId == id)
                .ToListAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim != null)
                {
                    NewComment.UserId = int.Parse(userIdClaim.Value);
                }

                NewComment.CreatedAt = DateTime.UtcNow;
                NewComment.PostId = Id;

                context.PostComments.Add(NewComment);
                await context.SaveChangesAsync();
            }

            return RedirectToPage(new { id = Id });
                 
        }
    }
}
