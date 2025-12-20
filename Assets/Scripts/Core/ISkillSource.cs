namespace Core
{
    public interface ISkillSource
    {
        int Level { get; }
        int Attack { get; }
        bool HasTag(string tag);
    }
}