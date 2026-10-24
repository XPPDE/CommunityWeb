using DbClassLibrary.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CommunityWeb.Pages
{
    public class ProfileModel(AppDbContext context) : PageModel
    {

        [BindProperty(SupportsGet = true)]
        public int Id { get; set; } = new();
        public User? Profile { get; set; }
        public List<Project> Projects { get; set; } = new();
        public List<Post> Posts { get; set; } = new();

        public bool IsOwner { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var user = await context.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null) return NotFound();

            Profile = user;

            var currentUserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (currentUserIdClaim != null)
            {
                IsOwner = int.Parse(currentUserIdClaim.Value) == id;
            }

            Posts = await context.Posts
                .Where(p => p.AuthorId == id)
                .OrderByDescending(p => p.Id)
                .ToListAsync();

            Projects = await context.Projects
                .Where(p => p.AuthorId == id)
                .OrderByDescending(p => p.Id)
                .ToListAsync();

            return Page();
        }
    }
}
