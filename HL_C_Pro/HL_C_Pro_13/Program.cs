/*
 * ДЗ 12. Каталог фільмів

Розробити додаток з використанням AspNet MVC, домашній менеджер фільмів.

Функціонал:

Відобразити список фільмів за жанром, роком
Додати фільм в список
Редагувати фільм
Видалити фільм
зберігати фільми в памʼяті (без Баз Даних)
 */

var builder = WebApplication.CreateBuilder(args);

const long MaxFileSize = 10L * 1024 * 1024 * 1024; // 10 GB

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSingleton<MovieManager.Services.MovieService>();

builder.Services.Configure<Microsoft.AspNetCore.Http.Features.FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = MaxFileSize;
});

builder.WebHost.ConfigureKestrel(kestrel =>
{
    kestrel.Limits.MaxRequestBodySize = MaxFileSize;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
