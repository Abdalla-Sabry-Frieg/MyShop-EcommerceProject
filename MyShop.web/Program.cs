using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using MyShop_DataAccess.Data;
using MyShop_DataAccess.DbInitializer;
using MyShop_DataAccess.Immplementation;
using MyShop_Entities.EmailSender;
using MyShop_Entities.Helper;
using MyShop_Entities.Models;
using MyShop_Entities.Repositories;
using Stripe;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
//Runtime Compilation 
 builder.Services.AddRazorPages().AddRazorRuntimeCompilation();

builder.Services.AddDbContext<ApplicationDbContext>(options
    => options.UseSqlServer(builder.Configuration.GetConnectionString("DefultConnection")));

// inject StripeSection from appsittings
builder.Services.Configure<StripeSection>(builder.Configuration.GetSection("stripe"));

// to make admin lock or un lock any user  
// 1-  lockoutOnFailure: true => in login.cs make this true 
// 2- and here 
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options => options.Lockout.DefaultLockoutTimeSpan=TimeSpan.FromMinutes(30))
 .AddEntityFrameworkStores<ApplicationDbContext>()
 .AddDefaultUI()
 .AddDefaultTokenProviders();

builder.Services.AddSingleton<IEmailSender, EmailSender>();

builder.Services.AddScoped<IUnitOfWork , UnitOfWork>();
//builder.Services.AddScoped<IDbIntalizer, DbIntalizer>();

builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequiredLength = 5;
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredUniqueChars = 0;
});


// to set session
builder.Services.AddMemoryCache();
builder.Services.AddSession();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// inject StripeSection from appsittings
StripeConfiguration.ApiKey = builder.Configuration.GetSection("stripe:Secretkey").Get<string>();

//SeedDb();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
// to run identity pages
app.MapRazorPages();



app.UseAuthentication();
app.UseAuthorization();

app.UseSession();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
      name: "areas",
      pattern: "{area=Admin}/{controller=Home}/{action=Index}/{id?}"
    );
    endpoints.MapControllerRoute(
     name: "areas",
     pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}"
   ); 
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}"
   );
    endpoints.MapRazorPages();
});


app.Run();


//void SeedDb()
//{
//    using(var scope = app.Services.CreateScope())
//    {
//        var DbIntializer = scope.ServiceProvider.GetRequiredService<IDbIntalizer>();
//        DbIntializer.Initalize();
//    }
//}