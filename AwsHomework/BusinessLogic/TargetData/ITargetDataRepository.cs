using System.Threading.Tasks;

namespace BusinessLogic.TargetData
{
    public interface ITargetDataRepository
    {
        Task Write(TargetEntity entity);
    }
}