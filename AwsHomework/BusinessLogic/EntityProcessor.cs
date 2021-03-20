using System.Threading.Tasks;
using BusinessLogic.SourceData;
using BusinessLogic.TargetData;

namespace BusinessLogic
{
    public class EntityProcessor
    {
        private readonly ISourceDataRepository sourceDataRepository;
        private readonly ITargetDataRepository targetDataRepository;

        public EntityProcessor(ISourceDataRepository sourceDataRepository, ITargetDataRepository targetDataRepository)
        {
            this.sourceDataRepository = sourceDataRepository;
            this.targetDataRepository = targetDataRepository;
        }

        public async Task ProcessFolder(string folderName)
        {
            foreach (var sourceEntity in await sourceDataRepository.ReadAllRecursively(folderName))
            {
                await targetDataRepository.Write(ProcessEntity(sourceEntity));
            }
        }

        private TargetEntity ProcessEntity(SourceEntity sourceEntity)
        {
            return new TargetEntity
            {
                Folder = sourceEntity.Folder,
                Id = sourceEntity.Id,
                Name = sourceEntity.Name,
                PasswordHash = HashHelper.Sha256(sourceEntity.Password),
                Created = sourceEntity.Created
            };
        }
    }
}