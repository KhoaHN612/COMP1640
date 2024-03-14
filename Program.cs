using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using COMP1640.Models;
using COMP1640.Areas.Identity.Data;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("MyConnect");

builder.Services.AddDbContext<Comp1640Context>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("MyConnect")
    ?? throw new InvalidOperationException("Connection string 'comp1640context' not found.")));

builder.Services.AddDefaultIdentity<COMP1640User>(options =>
    options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<Comp1640Context>();


// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

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
    pattern: "{controller=Students}/{action=Index}/{id?}");

app.MapRazorPages();
app.Run();
