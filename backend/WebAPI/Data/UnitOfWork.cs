using AutoMapper;
using WebAPI.Data.Repo;
using WebAPI.Interfaces;

namespace WebAPI.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DataContext dc;
        private readonly IMapper mapper;

        public UnitOfWork(DataContext dc, IMapper mapper)
        {
            this.dc = dc;
            this.mapper = mapper;
        }
        public ICityRepository CityRepository => new CityRepository(dc);

        public IUserRepository UserRepository => new UserRepository(dc);

        public IPropertyRepository PropertyRepository => new PropertyRepository(dc);

        public IPropertyTypeRepository PropertyTypeRepository => new PropertyTypeRepository(dc);

        public IFurnishingTypeRepository FurnishingTypeRepository => new FurnishingTypeRepository(dc);

        public async Task<bool> SaveAsync()
        {
            return await dc.SaveChangesAsync() > 0;
        }
    }
}
