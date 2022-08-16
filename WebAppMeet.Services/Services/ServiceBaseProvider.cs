using SharedProject.Factory;
using WebAppMeet.Data;
using WebAppMeet.Data.Entities;
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

        public virtual  Task<Response> Create<EntityModel>(EntityModel model)
        {
            return Task.FromResult(Factory.GetResponse<Response>(model));
        }


        public virtual  Task<Response> Delete<TId>(TId id)
        {
            return  Task.FromResult(Factory.GetResponse<Response>(true));
        }
    }

}