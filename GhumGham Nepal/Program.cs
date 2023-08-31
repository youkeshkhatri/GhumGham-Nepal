using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using GhumGham_Nepal.Repository;
using GhumGhamNepal.Core.ApplicationDbContext;
using GhumGhamNepal.Core.Services.EmailService;
using GhumGhamNepal.Core.Models.Identity;
using GhumGhamNepal.Core.Services.AttachmentService;
using GhumGhamNepal.Core.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddScoped(typeof(IUnitOfWork), typeof(UnitOfWork));
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped(typeof(ICommonAttachmentService), typeof(CommonAttachmentService));
builder.Services.AddScoped(typeof(ISmtpEmailService), typeof(SmtpEmailService));
builder.Services.AddScoped(typeof(IHttpContextService), typeof(HttpContextService));
builder.Services.AddLogging();


builder.Services.AddDbContext<ProjectContext>
    (options => options.UseSqlServer
    (builder.Configuration.GetConnectionString("dbconn")));

builder.Services.AddDbContext<IdentityProjectContext>
    (options => options.UseSqlServer
    (builder.Configuration.GetConnectionString("dbconn")));


builder.Services.AddDefaultIdentity<GhumGhamNepalUser>(options => options.SignIn.RequireConfirmedAccount = true)
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
app.UseAuthentication();

app.UseAuthorization();

app.MapRazorPages();//for identity pages only

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
