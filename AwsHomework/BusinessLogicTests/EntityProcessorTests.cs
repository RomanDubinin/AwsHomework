using System.Threading.Tasks;
using BusinessLogic;
using BusinessLogic.SourceData;
using BusinessLogic.TargetData;
using NSubstitute;
using NUnit.Framework;

namespace BusinessLogicTests
{
    public class EntityProcessorTests
    {
        private EntityProcessor entityProcessor;
        private ISourceDataRepository sourceDataRepository;
        private ITargetDataRepository targetDataRepository;

        [SetUp]
        public void Setup()
        {
            sourceDataRepository = Substitute.For<ISourceDataRepository>();
            targetDataRepository = Substitute.For<ITargetDataRepository>();

            entityProcessor = new EntityProcessor(sourceDataRepository, targetDataRepository);
        }

        [Test]
        public async Task TestProcessEntities()
        {
            sourceDataRepository
                .ReadAllRecursively("folder1")
                .Returns(new[]
                {
                    new SourceEntity
                    {
                        Folder = "folder1",
                        Id = "id1",
                        Name = "Name1",
                        Password = "password1",
                        Created = "2020.01.01"
                    },
                    new SourceEntity
                    {
                        Folder = "folder1",
                        Id = "id2",
                        Name = "Name2",
                        Password = "password2",
                        Created = "2020.01.02"
                    }
                });

            await entityProcessor.ProcessFolder("folder1");

            await targetDataRepository
                .Received()
                .Write(Arg.Is(new TargetEntity
                {
                    Folder = "folder1",
                    Id = "id1",
                    Name = "Name1",
                    PasswordHash = "0b14d501a594442a01c6859541bcb3e8164d183d32937b851835442f69d5c94e",
                    Created = "2020.01.01"
                }));
            await targetDataRepository
                .Received()
                .Write(Arg.Is(new TargetEntity
                {
                    Folder = "folder1",
                    Id = "id2",
                    Name = "Name2",
                    PasswordHash = "6cf615d5bcaac778352a8f1f3360d23f02f34ec182e259897fd6ce485d7870d4",
                    Created = "2020.01.02"
                }));
        }
    }
}