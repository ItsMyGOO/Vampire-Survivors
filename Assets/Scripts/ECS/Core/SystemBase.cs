namespace ECS.Core
{
    public abstract class SystemBase : ISystem
    {
        public abstract void Update(World world, float deltaTime);
    }
}