using DbClassLibrary.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CommunityWeb.Pages
{
    [Authorize]
    public class AddPostModel(AppDbContext context) : PageModel
    {
        [BindProperty]
        public Post NewPost { get; set; } = new();

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim != null)
            {
                NewPost.AuthorId = int.Parse(userIdClaim.Value);
            }

            NewPost.CreatedAt = DateTime.UtcNow;

            context.Posts.Add(NewPost);
            await context.SaveChangesAsync();

            return RedirectToPage("/Index");
        }
    }
}
