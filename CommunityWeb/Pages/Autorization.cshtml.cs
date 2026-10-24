using DbClassLibrary.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CommunityWeb.Pages
{
    public class AutorizationModel(AppDbContext context) : PageModel
    {

        [BindProperty]
        public LoginInputModel Input { get; set; } = new();

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            // 1. Ищем пользователя в БД
            var user = await context.Users.FirstOrDefaultAsync(u => u.Name == Input.Name);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Неверный логин или пароль.");
                return Page();
            }

            // Инициализируем стандартный хэшер .NET
            var hasher = new PasswordHasher<User>(); // В качестве generic-типа лучше передавать класс пользователя

            // ИСПРАВЛЕННЫЙ ВАРИАНТ: 
            // 1 параметр: сам объект пользователя (нужен хэшеру по спецификации .NET)
            // 2 параметр: эталонный зашифрованный хэш из вашей базы данных PostgreSQL
            // 3 параметр: ЧИСТЫЙ СЫРОЙ пароль, который человек только что ввел в инпут формы!
            PasswordVerificationResult result = hasher.VerifyHashedPassword(user, user.PasswordHash, Input.Password);

            // Проверяем результат сравнения
            if (result == PasswordVerificationResult.Failed)
            {
                ModelState.AddModelError(string.Empty, "Неверный логин или пароль.");
                return Page();
            }

            // 2. Создаем список "утверждений" (Claims) о пользователе
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Role, user.Role)
            };

            // 3. Создаем удостоверение
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            // 4. Записываем зашифрованную куку в браузер пользователя
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity));

            return RedirectToPage("/Index");
        }

    }

    public class LoginInputModel
    {
        public string Name { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
