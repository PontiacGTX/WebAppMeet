using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SharedProject.Factory;
using WebAppMeet.Data;
using WebAppMeet.DataAcess.DataContext;
using WebAppMeet.DataAcess.Repository;
using WebAppMeet.DataAcess.UserStore;
using WebAppMeet.Hubs;
using WebAppMeet.Services;
using WebAppMeet.Services.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
var conString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContextFactory<AppDbContext>(options =>
    options.UseNpgsql(conString), ServiceLifetime.Scoped);

builder.Services
    .AddDbContext<AppDbContext>(options => { options.UseNpgsql(conString); })
    .AddIdentity<AppUser, IdentityRole>(options => 
    {
        options.SignIn.RequireConfirmedAccount = true;
        options.SignIn.RequireConfirmedEmail = true; 
        options.User.RequireUniqueEmail = true;
    })
    .AddUserStore<AppUserStore>()
    .AddDefaultUI()
    .AddSignInManager()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddTokenProvider<DataProtectorTokenProvider<AppUser>>(TokenOptions.DefaultProvider);
builder.Services.AddAuthentication("Identity.Application")
    .AddCookie();

builder.Services.AddScoped(typeof(EntityRepository<>));
builder.Services.AddScoped(typeof(GenericFactory));
builder.Services.AddScoped<AppUserStore>();
builder.Services.AddScoped<UserServices>();
builder.Services.AddScoped<MeetingsServices>();

builder.Services.AddSignalR();
builder.Services.AddResponseCompression(options => options.MimeTypes.Concat(new[] { "application/octet-stream" }));
builder.Services.AddCors(options=>options.AddDefaultPolicy(builder=>builder.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin()));
var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.EnsureCreated();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();
app.UseCors(options => options.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
app.UseAuthentication();

app.UseAuthorization();
app.UseEndpoints(endPoints => { endPoints.MapHub<ConnectionHub>("/ConnectionsHub"); });

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
