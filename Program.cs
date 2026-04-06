using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Lägg till API-tjänster
builder.Services.AddControllers();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))
    ));

// Bygg app
var app = builder.Build();

// --- Viktigt: statiska filer för frontend ---
app.UseDefaultFiles();   // gör att root "/" öppnar index.html
app.UseStaticFiles();    // gör wwwroot tillgänglig

app.UseRouting();

// --- Endpoints för API ---
app.MapControllers();

// Ta bort HTTPS-redirect under utveckling
// app.UseHttpsRedirection(); // <-- kommentera eller ta bort

app.Run();