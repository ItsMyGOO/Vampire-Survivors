namespace ECS.Core
{
    public interface ISystem
    {
        void Update(World world, float deltaTime);
    }

    public abstract class SystemBase : ISystem
    {
        public abstract void Update(World world, float deltaTime);
    }
}