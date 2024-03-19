using SocialMedia.Core.Entities;
using System.Threading.Tasks;

namespace SocialMedia.Core.Interfaces
{
    public interface ISecurityRepository: IRepository<Security>
    {
        Task<Security> GetLoginByCredential(UserLogin login);
    }
}
