using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SharedProject.Factory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using WebAppMeet.Data;
using WebAppMeet.Data.Entities;
using WebAppMeet.Data.Models;
using WebAppMeet.DataAcess.Repository;

namespace WebAppMeet.Services.Services
{
    public class UserServices:ServiceBaseProvider
    {
       
        private UserManager<AppUser> _userManager { get; }
       

        public UserServices(GenericFactory factory, UserManager<AppUser> userManager)
        {
            _factory = factory;
            _userManager = userManager;
        }

        public async Task<Response> UsersLike(string searchTerm)
        {
            var userRepo =await _factory.GetRepositoryAsync<AppUser>();

            var res = (await userRepo.GetAll(x => x.Email.StartsWith($"{searchTerm}") , x => x)).Select(x=>x.Email).ToList();

            if(res is { Count:0})
            {
                return Factory.GetResponse<Response>(res, messages: new string[] { Factory.GetStringResponse(StringResponseEnum.NotFound, "email") });
            }

            return Factory.GetResponse<Response>(res);
        }


        public async Task<Response> Exists(Expression<Func<AppUser, bool>> selector)
        {
            bool existe = false;
            try
            {
                existe = await _userManager.Users.AnyAsync(selector);
            }
            catch (Exception)
            {

                throw;
            }
           return Factory.GetResponse<Response>(existe);
        }
        public override async Task<Response> Create<EntityModel>(EntityModel Model)
        {
            var model = Model as CreateUserModel;
            string email = model.Email.ToLowerInvariant();

            if (await _userManager.Users.AnyAsync(x => x.Email.ToLower() == email))
                return Factory.GetResponse<ErrorServerResponse>(null, messages: new string[] { Factory.GetStringResponse(StringResponseEnum.AlreadyInUse, "email") });

            var user = new AppUser { UserName = model.Email.ToLower(), Email = model.Email.ToLower() };

            var result = await _userManager.CreateAsync(user, model.Password);

            await _userManager.AddClaimAsync(user, new Claim(ClaimTypes.Email, model.Email));

            if (!result.Succeeded)
                return Factory.GetResponse<ErrorServerResponse>(null, messages: (new string[] { Factory.GetStringResponse(StringResponseEnum.InternalServerError) }.Concat(result.Errors.Select(x => x.Description))).ToArray());

            return Factory.GetResponse<Response>(user);
        }

        

        public async Task<Response> DeleteUser(string email)
        {
            if (!await _userManager.Users.AnyAsync(x => x.Email.ToLower() == email))
                return Factory.GetResponse<ErrorServerResponse>(null, messages: new string[] { Factory.GetStringResponse(StringResponseEnum.NotFound, "email") });

            var repo =await GetRepository<AppUser>();
            email = email.ToLower();

            var user =await repo.FirstOrDefault((AppUser x) => x.Email.ToLower() == email);
            user.IsEnabled = false;

            bool saved= await repo.SaveChanges();
            return Factory.GetResponse<Response>(saved);
        }


        public async Task<Response> Update(AppUser appUser)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(x => x.Id == appUser.Id);

            if (user is null)
                return Factory.GetResponse<ErrorServerResponse>(null, messages: new string[] { Factory.GetStringResponse(StringResponseEnum.NotFound, "user") });

            var repo = await GetRepository<AppUser>();

            var updated = await repo.UpdateAndSave(appUser, appUser.Id);

            if (updated is null)
                return Factory.GetResponse<ErrorServerResponse>(null, messages: new string[] { Factory.GetStringResponse(StringResponseEnum.InternalServerError), "The user couldnt be updated" });

            return Factory.GetResponse<Response>(updated);
        }
        public async Task<Response> GetBy<TId>(TId id)
        {
            string idV = Convert.ToString(id);

            var user= await _userManager.Users.FirstOrDefaultAsync(x => x.Id == idV);
            if(user is null)
                return Factory.GetResponse<ErrorServerResponse>(null, messages: new string[] { Factory.GetStringResponse(StringResponseEnum.NotFound, "user") });
          
            var cpy = user.Copy();

            
            return Factory.GetResponse<Response>(cpy);
        }

        public async Task<Response> GetUser(string email)
        {

            email = email.ToLower();

            var user = await _userManager.Users.FirstOrDefaultAsync(x => x.Email.ToLower() == email);

            if (user is null)
                return Factory.GetResponse<ErrorServerResponse>(null, messages: new string[] { Factory.GetStringResponse(StringResponseEnum.NotFound, "email") });

            var cpy = user.Copy();

            
            return Factory.GetResponse<Response>(cpy);
        }
    }
}
