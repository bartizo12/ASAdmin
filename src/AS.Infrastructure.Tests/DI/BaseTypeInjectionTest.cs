using Autofac;
using Xunit;

namespace AS.Infrastructure.Tests
{
    
    public class BaseTypeInjectionTest
    {
        #region some test classes and interfaces
        internal class Animal
        {
            public string Name { get; set; }
        }
        internal class Bird : Animal
        {
            public bool CanFly { get; set; }
        }
        internal interface IAnimalCreator
        {
            Animal Create(string name);
        }
        internal interface IBirdCreator : IAnimalCreator
        {
        }
        internal class BirdCreator : IBirdCreator
        {
            public Animal Create(string name)
            {
                return new Bird() { Name = name, CanFly = true };
            }
        }
        internal class AnimalProvider
        {
            private readonly IAnimalCreator creator;

            public AnimalProvider(IAnimalCreator _creator)
            {
                this.creator = _creator;
            }
            public Animal Get(string name)
            {
                return this.creator.Create(name);
            }
        }
        #endregion

        [Fact]
        public void BaseType_With_Interface_Should_Be_Injected()
        {
            ContainerBuilder builder = new ContainerBuilder();
            builder.RegisterType<BirdCreator>()
                   .AsImplementedInterfaces();
            builder.RegisterType<AnimalProvider>()
                   .AsSelf();
            using (IContainer container = builder.Build())
            {
                Animal animal = container.Resolve<AnimalProvider>().Get("Crow");
                Assert.NotNull(animal);
                Assert.IsType(typeof(Bird), animal);
            }
        }
    }
}
