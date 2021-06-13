using System.Threading.Tasks;

namespace Transporter.Core
{
    public interface IInsertable
    {
        Task SetAsync(string data);
    }
}