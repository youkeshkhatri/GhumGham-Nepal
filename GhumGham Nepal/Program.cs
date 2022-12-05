using GhumGham_Nepal.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using GhumGham_Nepal.Areas.Identity.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();


builder.Services.AddDbContext<ProjectContext>
    (options => options.UseSqlServer
    (builder.Configuration.GetConnectionString("dbconn")));

builder.Services.AddDbContext<IdentityProjectContext>
    (options => options.UseSqlServer
    (builder.Configuration.GetConnectionString("dbconn")));


builder.Services.AddDefaultIdentity<GhumGham_NepalUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<IdentityProjectContext>();

builder.Services.Configure<IdentityOptions>(options =>
{
    // Password settings.
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 1;

    // Default SignIn settings.
    options.SignIn.RequireConfirmedEmail = true;
    options.SignIn.RequireConfirmedPhoneNumber = false;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();;

app.UseAuthorization();

app.MapRazorPages();//for identity pages only

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
