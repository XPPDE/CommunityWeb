using DbClassLibrary.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CommunityWeb.Pages
{
    public class EditProfileModel(AppDbContext context) : PageModel
    {
        [BindProperty]
        public EditInputModel InputModel { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            // 1. Вытаскиваем ID текущего залогиненного юзера из куки
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return RedirectToPage("/Autorization");

            int userId = int.Parse(userIdClaim.Value);

            // 2. Ищем его в облачной базе данных Render
            var user = await context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) return NotFound();

            // 3. Заполняем поля формы текущими данными из базы, чтобы юзер их видел
            InputModel.Bio = user.Bio;
            // Если у вас колонка в Postgres называется TelegramChannel, укажите её:
            InputModel.TelegramChannel = user.TelegramChannel;
            InputModel.AvatarUrl = user.AvatarUrl;
            InputModel.Name = user.Name;

            return Page();
        }

        // Срабатывает при нажатии кнопки "Сохранить изменения"
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            // 1. Снова получаем ID юзера из сессии куки
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return RedirectToPage("/Autorization");

            int userId = int.Parse(userIdClaim.Value);

            // 2. Достаем оригинальную запись из базы Postgres для обновления
            var userInDb = await context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (userInDb == null) return NotFound();


            // 4. Обновляем текстовые поля профиля
            userInDb.Bio = InputModel.Bio;
            userInDb.TelegramChannel = InputModel.TelegramChannel;
            userInDb.AvatarUrl = InputModel.AvatarUrl;
            userInDb.Name = InputModel.Name;

            // 5. Коммитим изменения в глобальную базу данных Render
            await context.SaveChangesAsync();

            // Перенаправляем друга обратно в его обновленный профиль
            return RedirectToPage("/Profile", new { id = userId });
        }
    }

    public class EditInputModel
    {
        public string? Bio { get; set; }
        public string? TelegramChannel { get; set; }
        public string? AvatarUrl { get; set; }
        public string? Name { get; set; }
    }
}