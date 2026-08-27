using Microsoft.EntityFrameworkCore;
using VinylStore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=vinylstore.db"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
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
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<VinylStore.AppDbContext>();
    
    // Ищем оба жанра по названиям
    var genresToDelete = context.Genres
        .Where(g => g.Name == "фыв" || g.Name == "я гей")
        .ToList();
    
    if (genresToDelete.Any())
    {
        context.Genres.RemoveRange(genresToDelete);
        context.SaveChanges();
    }
}
app.Run();