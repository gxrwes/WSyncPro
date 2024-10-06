using System.Threading.Tasks;

namespace WSyncPro.Util.Services
{
    public interface IFileCopyMoveService
    {
        Task CopyFileAsync(string srcFilePath, string destinationPath);
        Task MoveFileAsync(string srcFilePath, string destinationPath);
    }
}
