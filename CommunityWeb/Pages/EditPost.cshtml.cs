using DbClassLibrary.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CommunityWeb.Pages
{
    public class EditPostModel(AppDbContext context) : PageModel
    {
        [BindProperty]
        public Post Post { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var post = await context.Posts.FindAsync(id);

            if (post == null)
                return NotFound();

            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (int.Parse(userIdClaim.Value) != post.AuthorId)
            {
                return Forbid();
            }

            Post = post;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            if (!ModelState.IsValid) return Page();

            var existingPost = await context.Posts.FindAsync(id);

            if (existingPost == null) return NotFound();

            existingPost.Title = Post.Title;
            existingPost.Content = Post.Content;

            await context.SaveChangesAsync();

            return RedirectToPage("/Index");
        }
    }
}