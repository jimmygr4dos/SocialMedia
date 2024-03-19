using SocialMedia.Core.Entities;
using SocialMedia.Core.Interfaces;
using System.Threading.Tasks;

namespace SocialMedia.Core.Services
{
    public class SecurityService: ISecurityService
    {
        public readonly IUnitOfWork _unit;

        public SecurityService(IUnitOfWork unit)
        {
            _unit = unit;
        }

        public async Task<Security> GetLoginByCredentials(UserLogin userLogin)
        {
            return await _unit.SecurityRepository.GetLoginByCredential(userLogin);
        }

        public async Task RegisterUser(Security security)
        {
            await _unit.SecurityRepository.Add(security);
            await _unit.SaveChangesAsync();
        }

    }
}
