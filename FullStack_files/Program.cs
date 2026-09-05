using CoolCompanyEstore.Data;
using CoolCompanyEstore.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);


// إعداد الاتصال بقاعدة البيانات
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString)
            .EnableSensitiveDataLogging()
            .LogTo(Console.WriteLine, Microsoft.Extensions.Logging.LogLevel.Information));


// إعداد الهوية (المستخدمين والأدوار)
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.AccessDeniedPath = "/Account/AccessDenied";
});

// الإيميل التجريبي
builder.Services.AddTransient<Microsoft.AspNetCore.Identity.UI.Services.IEmailSender, CoolCompanyEstore.Services.EmailSender>();

builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// **إضافة خدمات الجلسة وذاكرة التخزين المؤقتة**
builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});


var app = builder.Build();

// **التحقق من مجلد الصور في wwwroot/img/products**
var uploadsPath = Path.Combine(builder.Environment.WebRootPath, "img", "products");
if (!Directory.Exists(uploadsPath))
{
    Directory.CreateDirectory(uploadsPath);
}

// تشغيل الخدمات المطلوبة
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

// **تفعيل static files: مجلد wwwroot بالكامل**
app.UseStaticFiles();

app.UseRouting();

// **تمكين استخدام الجلسة**
app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

// إعداد الراوت الأساسي
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

// **تنفيذ Seeder قبل تشغيل التطبيق**
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ApplicationDbContext>();

    // 1. إضافة فئة Uncategorized
    try
    {
        var uncategorizedCategory = await context.Categories
            .FirstOrDefaultAsync(c => c.Name == "Uncategorized");

        if (uncategorizedCategory == null)
        {
            var newCategory = new Category { Name = "Uncategorized" };
            context.Categories.Add(newCategory);
            await context.SaveChangesAsync();
            Console.WriteLine("Added 'Uncategorized' category to the database.");
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the 'Uncategorized' category.");
    }

    // 2. إنشاء المستخدمين والأدوار التجريبية
    await SeedRolesAndUsersAsync(services);

    // 3. تشغيل Seeder الرئيسي
    ApplicationDbSeeder.Seed(context);
}

// إنشاء المستخدمين والأدوار التجريبية
async Task SeedRolesAndUsersAsync(IServiceProvider services)
{
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

    string[] roles = { "SuperAdmin", "ContentManager", "NormalUser", "Moderator" };

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new IdentityRole(role));
    }

    string adminEmail = "admin@example.com";
    string adminPassword = "Admin@123";

    if (await userManager.FindByEmailAsync(adminEmail) is null)
    {
        var admin = new ApplicationUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            FullName = "Super Admin",
            PhoneNumber = "0000000000",
            ProfileImageUrl = "/img/default.jpg"
        };
        if ((await userManager.CreateAsync(admin, adminPassword)).Succeeded)
            await userManager.AddToRoleAsync(admin, "SuperAdmin");
    }

    string contentEmail = "content@estore.com";
    string contentPassword = "Content123!";

    if (await userManager.FindByEmailAsync(contentEmail) is null)
    {
        var contentUser = new ApplicationUser
        {
            UserName = contentEmail,
            Email = contentEmail,
            FullName = "Content Manager"
        };
        if ((await userManager.CreateAsync(contentUser, contentPassword)).Succeeded)
            await userManager.AddToRoleAsync(contentUser, "ContentManager");
    }
}

app.Run();
