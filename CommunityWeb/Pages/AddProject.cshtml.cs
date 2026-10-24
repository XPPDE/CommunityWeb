using DbClassLibrary.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CommunityWeb.Pages
{
    [Authorize]
    public class AddProjectModel(AppDbContext context) : PageModel
    {
        [BindProperty]
        public Project NewProject { get; set; } = new();

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Автоматически подвязываем ID текущего авторизованного пользователя из куки!
            // (Мы зашивали ClaimTypes.NameIdentifier при авторизации)
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim != null)
            {
                NewProject.AuthorId = int.Parse(userIdClaim.Value);
            }

            NewProject.CreatedAt = DateTime.UtcNow;

            context.Projects.Add(NewProject);
            await context.SaveChangesAsync();

            return RedirectToPage("/Index");
        }
    }
}
