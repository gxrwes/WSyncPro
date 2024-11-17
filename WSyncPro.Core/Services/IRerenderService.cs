using System.Threading.Tasks;
using WSyncPro.Models.Content;

namespace WSyncPro.Util.Services
{
    public interface IRerenderService
    {
        Task ReRender(RenderJob job);
    }
}
