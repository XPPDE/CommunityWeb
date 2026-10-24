using DbClassLibrary.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CommunityWeb.Pages
{
    public class BlogModel(AppDbContext context) : PageModel
    {
        public List<Post> Posts { get; set; } = new();

        public async Task OnGetAsync()
        {

            Posts = await context.Posts
                .Include(p => p.Author)
                .OrderByDescending(p => p.Id)
                .ToListAsync();
        }
    }
}
