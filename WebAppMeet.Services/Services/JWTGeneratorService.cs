using SharedProject.Factory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using WebAppMeet.Data.Entities;

namespace WebAppMeet.Services.Services
{
    public class JWTGeneratorService
    {
        GenericFactory _factory { get; }

        public JWTGeneratorService(GenericFactory dbFactory)
        {
            _factory = dbFactory;
        }

        public async Task<string> GenerateKey()
        {
            var key = new byte[32];
            var res =new RNGCryptoServiceProvider();
            res.GetBytes(key);
            var base64Secret = Convert.ToBase64String(key);
            return base64Secret.TrimEnd('=').Replace('+', '-').Replace('/', '_');
        }

        public async Task<bool> SaveDbKey(SKeyValues details)
        {
            var repo =await _factory.GetRepositoryAsync<SKeyValues>();
            var anyFound = await repo.Any(x => x.IsValid);

            if (anyFound)
                return anyFound;

            var res= await repo.AddAndSave(details);
            return res != null;
        }
    }
}
