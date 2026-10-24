using DbClassLibrary.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CommunityWeb.Pages
{
    public class IndexModel(AppDbContext context) : PageModel
    {

        public List<Project> Projects { get; set; } = new();

        public int? CurrentUserId { get; set; }

        public async Task OnGetAsync()
        {

            Projects = await context.Projects
                .Include(p => p.Author)
                .OrderByDescending(p => p.Id)
                .ToListAsync();

            // Вытаскиваем ID авторизованного пользователя
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim != null)
            {
                CurrentUserId = int.Parse(userIdClaim.Value);
            }

            Projects = await context.Projects
                .Include(p => p.Author)
                .OrderByDescending(p => p.Id)
                .ToListAsync();
        }
    }
}
