using DbClassLibrary.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Autorization"; // Куда перенаправлять, если страница требует авторизации, а пользователь не вошел
        options.ExpireTimeSpan = TimeSpan.FromDays(7); // Сколько помнить пользователя (например, 7 дней)

        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest; // Кука будет работать и на http, и на https
        options.Cookie.SameSite = SameSiteMode.Lax;
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseStaticFiles();

app.UseAuthentication(); // Кто ты такой? (Проверка куки)
app.UseAuthorization();  // Можно ли тебе сюда? (Проверка прав)

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();
