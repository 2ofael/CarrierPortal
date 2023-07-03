using CarrierPortal.Models;
using CarrierPortal.Repository;
using CarrierPortal.Services.PhotoServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;

var builder = WebApplication.CreateBuilder(args);

var config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();

builder.Services.AddControllersWithViews();





var ConnectionString = config.GetConnectionString("PortalDB");
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(config.GetConnectionString("PortalDB")));

builder.Services.AddScoped<DbContext, AppDbContext>();

builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IPhotoService, PhotoService>();
builder.Services.AddScoped<IPhotoListService, PhotoListService>();
builder.Services.AddScoped<IActorRepository, ActorRepository>();



builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{

})
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();
 







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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
