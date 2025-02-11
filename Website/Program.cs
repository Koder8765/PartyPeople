using Dapper;
using FluentValidation;
using System.Reflection;
using Website.Persistence;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? string.Empty;

services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly(), ServiceLifetime.Transient);

services.AddTransient<IDbConnectionFactory>(_ => new SqliteConnectionFactory(connectionString));
services.AddTransient<DbContext>();

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Initialise our SQLite database.
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<DbContext>();
    await context.InitialiseDatabase(CancellationToken.None);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

SqlMapper.AddTypeHandler(new SqlDateOnlyTypeHandler());

app.Run();
