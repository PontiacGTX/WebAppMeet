using WebAppMeet.Data;
using WebAppMeet.Data.Entities;
using WebAppMeet.DataAcess.Factory;
using WebAppMeet.DataAcess.Repository;

namespace WebAppMeet.Services.Services
{
    public abstract class ServiceBaseProvider :IService
    {
        protected GenericFactory _factory { get; set; }
        protected async Task<EntityRepository<T>> GetRepository<T>()
          where T : class, IEntity, new()
           => await _factory.GetRepositoryAsync<T>();
        public async Task<GenericFactory> GetGenericRepository()
            => await Task.FromResult(_factory);

      


      
    }

}