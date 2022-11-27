using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Net.Http.Headers;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Xml.Linq;
using WebAppMeet.Data;
using WebAppMeet.Data.Models;
using WebAppMeet.DataAcess.Factory;
using WebAppMeet.Services.Services;

namespace WebAppMeet.Areas.Identity.Pages.Account
{
    public class ReditectModel : PageModel
    {
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }
        }
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ILogger<LoginModel> _logger;
        private readonly AuthTokenServices _authService;
        private readonly UserManager<AppUser> _userManager;

        public ReditectModel(SignInManager<AppUser> signInManager,UserManager<AppUser> userManager, ILogger<LoginModel> logger, AuthTokenServices authServices)
        {
            _signInManager = signInManager;
            _logger = logger;
            _authService = authServices;
            _userManager = userManager;


        }
        public void OnGet()
        {
        }
        //[HttpPost("/Identity/")]
        public async Task<IActionResult> OnPostAsync([FromBody] InputModel model,[FromQuery] string returnUrl = null)
        {
            //if(!_userManager.Users.Any(x=>x.UserName.ToLowerInvariant() == model.Email.ToLowerInvariant()))
            //{

            //    return StatusCode(StatusCodes.Status401Unauthorized, Factory.GetResponse<ErrorServerResponse<object>, object>(null, 401, false, new[] {"Username is not valid "}));
            //}

            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                var response = await _authService.GetJWTToken(new Data.Models.UserTokenRequest { Password = model.Password, UserName = model.Email });
                if (response.StatusCode != 200)
                {

                    TempData["ErrorLogin"] = response.Message;
                    ModelState.AddModelError(string.Empty, response.Message);
                    return StatusCode(response.StatusCode, new {
                        response.Message, 
                        response.StatusCode, 
                        response.Data, 
                        response.Validation, 
                        Redirect = "/Identity/Account/Login" });
                    
                }

                this.Response.Headers[HeaderNames.Authorization] = Convert.ToBase64String(Encoding.UTF8.GetBytes((response.Data as TokenResponse).Token));
                Request.Headers[HeaderNames.Authorization] = Convert.ToBase64String(Encoding.UTF8.GetBytes((response.Data as TokenResponse).Token));
                _logger.LogInformation("User logged in.");
                return  StatusCode(response.StatusCode, new
                {
                    response.Message,
                    response.StatusCode,
                    response.Data ,
                    response.Validation,
                    Redirect = "/",
                    TokenBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes((response.Data as TokenResponse).Token))
                 });
            }
            if (result.RequiresTwoFactor)
            {
                return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
            }
            if (result.IsLockedOut)
            {
                _logger.LogWarning("User account locked out.");
                return RedirectToPage("./Lockout");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return StatusCode(StatusCodes.Status400BadRequest, new { });
            }
            return StatusCode(StatusCodes.Status200OK,new { });
        }
    }
}
