using DbClassLibrary.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CommunityWeb.Pages
{
    public class UsersModel(AppDbContext context) : PageModel
    {
        public List<User> Users { get; set; }

        public async Task OnGet()
        {
            Users = await context.Users.ToListAsync();
        }
    }
}
