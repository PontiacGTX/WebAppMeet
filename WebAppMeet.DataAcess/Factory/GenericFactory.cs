using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAppMeet.Data.Entities;
using WebAppMeet.DataAcess.DataContext;
using WebAppMeet.DataAcess.Repository;

namespace WebAppMeet.DataAcess.Factory
{
    public class GenericFactory:IDisposable
    {
        private bool disposedValue;

        private AppDbContext _ctx { get; set; }
        protected IDbContextFactory<AppDbContext> _factory { get; set; }
        public GenericFactory(IDbContextFactory<AppDbContext> DbFactory)
        {
            _factory = DbFactory;
        }
        public async Task<EntityRepository<T>> GetRepositoryAsync<T>()
            where T: class,IEntity,new()
        => await Task.FromResult(new EntityRepository<T>(_ctx=_factory.CreateDbContext()));

        public  EntityRepository<T> GetRepository<T>()
           where T : class, IEntity, new()
       =>  new EntityRepository<T>(_ctx=_factory.CreateDbContext());

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _ctx?.Dispose();// TODO: eliminar el estado administrado (objetos administrados)
                }

                // TODO: liberar los recursos no administrados (objetos no administrados) y reemplazar el finalizador
                // TODO: establecer los campos grandes como NULL
                disposedValue = true;
            }
        }

        // // TODO: reemplazar el finalizador solo si "Dispose(bool disposing)" tiene código para liberar los recursos no administrados
        // ~GenericFactory()
        // {
        //     // No cambie este código. Coloque el código de limpieza en el método "Dispose(bool disposing)".
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // No cambie este código. Coloque el código de limpieza en el método "Dispose(bool disposing)".
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
