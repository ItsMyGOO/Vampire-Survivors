namespace ECS.Core
{
    public interface ISystem
    {
        void Update(World world, float deltaTime);
    }
}