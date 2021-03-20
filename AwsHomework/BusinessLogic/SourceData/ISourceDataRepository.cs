using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLogic.SourceData
{
    public interface ISourceDataRepository
    {
        Task<IEnumerable<SourceEntity>> ReadAllRecursively(string folderName);
    }
}