using System.Threading.Tasks;

namespace WebApplication.Proxies
{
    public interface ICityClient
    {
        Task<string> Get();
    }
}