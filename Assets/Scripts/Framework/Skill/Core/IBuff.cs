namespace Core.Buff
{
    public interface IBuff
    {
        void OnAdd(ISkillTarget owner);
        void OnRemove();
        void Modify(SkillContext ctx);
    }
}