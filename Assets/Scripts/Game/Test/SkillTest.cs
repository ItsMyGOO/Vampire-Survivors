using Combat.Buff;
using Combat.Skill;
using Core;

namespace Game.Test
{
    public class SkillTest
    {
        void Main()
        {
            var attacker = new Player();
            var defender = new Player();

            var sourceBuffs = new BuffController();
            var targetBuffs = new BuffController();

            sourceBuffs.AddBuff(BuffFactory.CreateIncreaseDamage(10), attacker);
            targetBuffs.AddBuff(BuffFactory.CreateReduceDamage(5), defender);

            var ctx = new SkillContext
            {
                Caster = attacker,
                Target = defender,
                SourceBuffs = sourceBuffs,
                TargetBuffs = targetBuffs
            };

            var executor = SkillFactory.CreateDefault();
            executor.Execute(ctx);
        }
    }

    public class Player : ISkillSource, ISkillTarget
    {
        public int Level { get; }
        public int Attack { get; }

        public bool HasTag(string tag)
        {
            throw new System.NotImplementedException();
        }

        public int Defense { get; }
        public bool IsAlive { get; }

        public void ApplyDamage(int value)
        {
        }

        public bool HasBuff(string buffId)
        {
            return false;
        }

        public void TakeDamage(float damage)
        {
        }
    }
}