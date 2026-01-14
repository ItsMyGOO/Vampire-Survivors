namespace Core.Buff
{
    public interface ISkillTarget
    {
        int Defense { get; }
        bool IsAlive { get; }

        void ApplyDamage(int value);
        bool HasBuff(string buffId);
        void TakeDamage(float damage);
    }
}