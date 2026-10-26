using DbClassLibrary.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace CommunityWeb.Pages
{
    public class RegistrationModel(AppDbContext context) : PageModel
    {
        [BindProperty]
        public RegisterInputModel Input { get; set; } = new();
        public User NewUser { get; set; } = new();

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            var user = await context.Users.FirstOrDefaultAsync(u => u.Username == Input.Username);

            if (user != null)
            {
                ModelState.AddModelError(string.Empty, "Пользователь с таким именем уже зарегистрирован.");
                return Page();
            }

            if (Input.Password != Input.RepeatPassword)
            {
                ModelState.AddModelError(string.Empty, "Пароли не совпадают.");
                return Page();
            }

            var hasher = new PasswordHasher<User>();
            string strongPasswordHash = hasher.HashPassword(null!, Input.Password);

            NewUser.Username = Input.Username.ToLower();
            NewUser.PasswordHash = strongPasswordHash;
            NewUser.Role = "Member";

            context.Users.Add(NewUser);
            await context.SaveChangesAsync();

            user = await context.Users.FirstOrDefaultAsync(u => u.Username == Input.Username);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity));

            return RedirectToPage("/Index");
        }

    }

    public class RegisterInputModel
    {
        [Required(ErrorMessage = "Введите логин")]
        [StringLength(30, MinimumLength = 3, ErrorMessage = "Логин должен быть от 3 до 30 символов")]
        [RegularExpression(@"^[a-z0-9_]+$", ErrorMessage = "Логин может содержать только строчные латинские буквы, цифры и подчеркивание")]
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string RepeatPassword { get; set; } = string.Empty;
    }
}