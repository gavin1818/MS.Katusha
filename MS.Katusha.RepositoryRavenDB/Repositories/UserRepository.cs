using MS.Katusha.Domain;
using MS.Katusha.Domain.Entities;
using MS.Katusha.IRepositories.Interfaces;

namespace MS.Katusha.RepositoryRavenDB.Repositories
{
    public class UserRepository : BaseGuidRepository<User>, IUserRepository
    {
        public UserRepository(KatushaContext context) : base(context) { }
    }
}