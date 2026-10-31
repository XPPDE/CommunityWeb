using DbClassLibrary.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
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
            if (!string.IsNullOrEmpty(InputModel.Bio))
            {
                // Убираем \r (возврат каретки), оставляя только \n
                InputModel.Bio = InputModel.Bio.Replace("\r\n", "\n");

                // Убираем лишние пробелы в начале и в конце
                InputModel.Bio = InputModel.Bio.Trim();
            }

            ModelState.Remove("InputModel.Bio");

            if (!TryValidateModel(InputModel, nameof(InputModel)))
            {
                return Page(); 
            }

            if (!ModelState.IsValid)
            {
                foreach (var key in ModelState.Keys)
                {
                    var errors = ModelState[key].Errors;
                    foreach (var error in errors)
                    {
                        Console.WriteLine($"Ошибка в поле {key}: {error.ErrorMessage}");
                    }
                }
                return Page();
            }

            // 1. Снова получаем ID юзера из сессии куки
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return RedirectToPage("/Autorization");

            int userId = int.Parse(userIdClaim.Value);

            // 2. Достаем оригинальную запись из базы Postgres для обновления
            var userInDb = await context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (userInDb == null) return NotFound();


            // 4. Обновляем тек стовые поля профиля
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
        [StringLength(3000, ErrorMessage = "Описание должно быть от 0 до 3000 символов")]
        public string? Bio { get; set; }
        public string? TelegramChannel { get; set; }
        public string? AvatarUrl { get; set; }
        public string? Name { get; set; }
    }
}