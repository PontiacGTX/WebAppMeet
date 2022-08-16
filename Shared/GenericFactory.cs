using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAppMeet.DataAcess.DataContext;
using WebAppMeet.DataAcess.Repository;

namespace Shared
{
    public class GenericFactory
    {
        private AppDbContext _ctx { get; }

        public GenericFactory(AppDbContext ctx)
        {
            _ctx = ctx;
        }
        public async Task<EntityRepository<T>> GetRepository<T>()
        =>  await Task.FromResult( new EntityRepository<T>(_ctx));
        
    }
}
