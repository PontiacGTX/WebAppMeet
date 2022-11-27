using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.Circuits;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using NuGet.Common;
using SharedProject.HelperClass;
using System.Buffers.Text;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Text.Json;
using WebAppMeet.CircuitHandler;
using WebAppMeet.Components.Helper;
using WebAppMeet.Data;
using WebAppMeet.Data.Entities;
using WebAppMeet.Data.Models;
using WebAppMeet.DataAcess.DataContext;
using WebAppMeet.DataAcess.Factory;
using WebAppMeet.DataAcess.Repository;
using WebAppMeet.DataAcess.UserStore;
using WebAppMeet.Hubs;
using WebAppMeet.Services;
using WebAppMeet.Services.Services;

var builder = WebApplication.CreateBuilder(args);
    //.UseSetting(WebHostDefaults.DetailedErrorsKey, "true");

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor().AddCircuitOptions(options =>
{
    options.DetailedErrors = true;

});
var conString = builder.Configuration.GetConnectionString("AppDbContextConnection");

builder.Services.AddDbContextFactory<AppDbContext>(options =>
    options.UseSqlServer(conString), ServiceLifetime.Scoped);

builder.Services
    .AddDbContext<AppDbContext>(options => { options.UseSqlServer(conString); })
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
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme/*"Identity.Application"*/;
    //options.DefaultChallengeScheme = "JWT_OR_COOKIE";
}).AddJwtBearer(options => {
        //options.Authority = "https://localhost:7044/Security/Token/Validate=access_token={0}";
        options.TokenValidationParameters =new TokenValidationParameters()
        {
            ValidateAudience=false,
            ValidateIssuer=true,
            ValidateLifetime=true,
            ValidateIssuerSigningKey=true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience=builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration["Jwt:Key"])),


        };
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];

                // If the request is for our hub...
                var path = context.HttpContext.Request.Path;
                
                if (!string.IsNullOrEmpty(accessToken) &&
                    (path.StartsWithSegments("/ConnectionsHub")))
                {
                     //options.Authority =options.Authority.Replace("{0}", accessToken);
                    // Read the token out of the query string
                    context.Token = accessToken;
                    if(context.HttpContext?.User?.Identity?.Name==null && !string.IsNullOrEmpty(accessToken))
                    {
                        
                        context.HttpContext.User = GetUser(accessToken);
                    }
                    context.Response.Headers.Authorization = accessToken;
                    Console.Write(context.Scheme.Name);
                }
                return Task.CompletedTask;
            }
        };
})
//.AddCookie("Cookies", options =>
//{
//    options.LoginPath = "/identity/account/login";
//    options.ExpireTimeSpan = TimeSpan.FromDays(1);
//})
//.AddJwtBearer("Bearer",options =>
//{
//    options.RequireHttpsMetadata = false;
//    // Configure the Authority to the expected value for
//    // the authentication provider. This ensures the token
//    // is appropriately validated.
//    options.Authority = "/Security/Token/Validate"; // TODO: Update URL
//    options.TokenValidationParameters = new TokenValidationParameters
//    {
//        ValidIssuer = builder.Configuration["Jwt:Issuer"],
//        ValidAudience=builder.Configuration["Jwt:Audience"],
//        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration["Jwt:Key"])),
//    };
//    // We have to hook the OnMessageReceived event in order to
//    // allow the JWT authentication handler to read the access
//    // token from the query string when a WebSocket or 
//    // Server-Sent Events request comes in.

//    // Sending the access token in the query string is required due to
//    // a limitation in Browser APIs. We restrict it to only calls to the
//    // SignalR hub in this code.
//    // See https://docs.microsoft.com/aspnet/core/signalr/security#access-token-logging
//    // for more information about security considerations when using
//    // the query string to transmit the access token.
//    options.Events = new JwtBearerEvents
//    {
//        OnMessageReceived = context =>
//        {
//            var accessToken = context.Request.Query["access_token"];

//            // If the request is for our hub...
//            var path = context.HttpContext.Request.Path;
//            if (!string.IsNullOrEmpty(accessToken) &&
//                (path.StartsWithSegments("/ConnectionsHub")))
//            {
//                // Read the token out of the query string
//                context.Token = accessToken;
//            }
//            return Task.CompletedTask;
//        }
//    };
//})
//.AddPolicyScheme("JWT_OR_COOKIE", "JWT_OR_COOKIE", options =>
//{
//    // runs on each request
//    options.ForwardDefaultSelector = context =>
//    {
//        // filter by auth type
//        string authorization = context.Request.Headers[HeaderNames.Authorization];
//        if (!string.IsNullOrEmpty(authorization) && authorization.StartsWith("Bearer "))
//            return "Bearer";

//        // otherwise always check for cookie auth
//        return "Cookies";
//    };
//})
;

IEnumerable<Claim> ParseJWTClaims(string jwt)
{
    var payload = jwt.Contains('.') ? jwt.Split('.')[1] : jwt;
    var jsonBytes = ParseBase64WihoutPadding(payload);
    var kvPairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);
    return (kvPairs ?? new Dictionary<string, object>()).Select(kv => new Claim(kv.Key, kv.Value.ToString()));

}

byte[] ParseBase64WihoutPadding(string base64)
{
    var x = base64.Length % 4;
    return Convert.FromBase64String((base64.Length % 4) switch
    {

        2 => base64 += "==",
        3 => base64 += "=",
        0 => base64,
        _ => base64,

    });
}

ClaimsPrincipal GetUser(string accessToken)
{
    ClaimsIdentity identity = null;
    ClaimsPrincipal user = null;
    //AuthenticationState state = null;
    string authType = "jwt";
    var claims = ParseJWTClaims(accessToken).ToList();
    claims.Add(new System.Security.Claims.Claim(ClaimTypes.Name, claims.First(x => x.Type == "email").Value));
    var id = new ClaimsIdentity(claims, authType);
    identity = new ClaimsIdentity(claims, authType);
    identity.Actor = id;
    user = new ClaimsPrincipal(identity);
    return user;
}

builder.Services.AddScoped(typeof(EntityRepository<>));
builder.Services.AddScoped(typeof(GenericFactory));
builder.Services.AddScoped<AppUserStore>();
builder.Services.AddScoped<UserServices>();
builder.Services.AddScoped<MeetingsServices>();
builder.Services.AddScoped<JWTGeneratorService>();
builder.Services.AddSingleton<HttpClient>();
builder.Services.AddScoped<AuthTokenServices>();
builder.Services.AddScoped<LocalStorage>();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();
builder.Services.AddScoped<CustomAuthenticationStateProvider>();

builder.Services.AddSingleton<CircuitHandler, TrackingCircuitHandler>();
builder.Services.AddHttpClient();
builder.Services.AddSignalR();
builder.Services.AddSingleton<IUserIdProvider, NameUserIdProvider>();
builder.Services.AddSingleton<IUserIdProvider, CustomEmailProvider>();
//builder.Services.AddSingleton<IUserIdProvider, EmailBasedUserIdProvider>();
builder.Services.AddResponseCompression(options => options.MimeTypes.Concat(new[] { "application/octet-stream" }));
builder.Services.AddCors(options=>options.AddDefaultPolicy(builder=>builder.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin()));
var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.EnsureCreated();
    {
       var jwtHelper = scope.ServiceProvider.GetRequiredService<JWTGeneratorService>();
       var saved =await jwtHelper.SaveDbKey(new SaveDbKeyModel()
        {
            GenerateBy =
            System.Reflection.Assembly.GetExecutingAssembly().FullName,
            GeneratedAt = DateTime.Today.Date.ToDateTimeFormat(),
            IsValid = true,
            Key = await jwtHelper.GenerateKey(),

        });
    }


    
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

app.UseEndpoints(endPoints => { 
    
    endPoints.MapHub<ConnectionHub>("/ConnectionsHub");

    endPoints.MapBlazorHub();
    endPoints.MapFallbackToPage("/_Host");
    app.MapPost("/Security/Token/Create",
    [AllowAnonymous]
    async ([FromBody]WebAppMeet.Data.Models.UserTokenRequest user, [FromServices] SignInManager<AppUser> signInManager)
    =>
    {
        var appUser = await signInManager.UserManager.Users.FirstOrDefaultAsync(x => x.UserName == user.UserName);

        if ((await signInManager.PasswordSignInAsync(appUser, user.Password, true, false)).Succeeded)
        {
            using (var scope = app.Services.CreateScope())
            {
                var factory = scope.ServiceProvider.GetRequiredService<GenericFactory>();
                var repo =factory.GetRepository<SKeyValues>();
                var keyObj = await repo.FirstOrDefault(x => x.IsValid);
                var expiry = DateTime.UtcNow.AddDays(1);
                var issuer = builder.Configuration["Jwt:Issuer"];
                var audience = builder.Configuration["Jwt:Audience"];
                var key = Encoding.ASCII.GetBytes(keyObj.Key);

                var roles = await signInManager.UserManager.GetRolesAsync(appUser);
                var claims = new Dictionary<string, object>();
                foreach (var role in roles)
                {
                    claims.Add(role, new Claim(ClaimTypes.Role, role));
                }
               
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[]
                    {
                        new Claim("Id", appUser.Id),
                        new Claim(JwtRegisteredClaimNames.Sub, appUser.UserName),
                        new Claim(JwtRegisteredClaimNames.Email, appUser.UserName),
                        new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString())
                     }),

                    Expires = DateTime.UtcNow.AddHours(14),
                    Issuer = issuer,
                    Audience = audience,
                    IssuedAt= DateTime.Now,
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),SecurityAlgorithms.HmacSha512Signature),
                    Claims = claims
                };
                var tokenHandler = new JwtSecurityTokenHandler();
                var token = tokenHandler.CreateToken(tokenDescriptor);
                var jwtToken = tokenHandler.WriteToken(token);
                return Results.Ok(Factory.GetResponse<Response<TokenResponse>, TokenResponse>(new TokenResponse { Token = jwtToken, Expiration = expiry, User = appUser?.UserName}));

            }
        }
        return Results.Ok((StatusCodes.Status401Unauthorized, Factory.GetResponse<Response<TokenResponse>, TokenResponse>(null, 401,false,Factory.GetStringResponse(StringResponseEnum.Unathorized))));
    });

    app.MapGet("/Security/Token/Validate=access_token={token}",
    [AllowAnonymous]
    async ([FromQuery]string token) =>
    {

        var tokenHandler = new JwtSecurityTokenHandler();
        byte[] key = null;
        try
        {
            using (var scope = app.Services.CreateScope())
            {
                var factory = scope.ServiceProvider.GetRequiredService<GenericFactory>();
                var repo = factory.GetRepository<SKeyValues>();
                var keyObj = await repo.FirstOrDefault(x => x.IsValid);
                key=Encoding.ASCII.GetBytes(keyObj.Key);
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var accountId = int.Parse(jwtToken.Claims.First(x => x.Type == "id").Value);

                // return account id from JWT token if validation successful

                Results.Ok(Factory.GetResponse<Response<int?>, int?>(accountId));
               
            }
        }
        catch(Exception ex)
        {
            // return null if validation fails
            return Results.Ok((StatusCodes.Status401Unauthorized, Factory.GetResponse<Response<int?>,int?>(null, 401, false, Factory.GetStringResponse(StringResponseEnum.Unathorized))));
        }
        return Results.Ok((StatusCodes.Status401Unauthorized, Factory.GetResponse<Response<int?>, int?>(null, 401, false, Factory.GetStringResponse(StringResponseEnum.Unathorized))));
    });

});


//app.MapBlazorHub();
//app.MapFallbackToPage("/_Host");

app.Run();
