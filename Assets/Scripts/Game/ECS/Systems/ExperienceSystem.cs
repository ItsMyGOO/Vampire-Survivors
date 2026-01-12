using ECS.Core;
using UnityEngine;

namespace ECS.Systems
{
    /// <summary>
    /// 经验和升级系统
    /// 职责: 处理经验累积和升级逻辑
    /// </summary>
    public class ExperienceSystem : SystemBase
    {
        // 升级经验曲线配置
        private const float BASE_EXP = 100f;
        private const float EXP_GROWTH_RATE = 1.15f;

        public override void Update(World world, float deltaTime)
        {
            foreach (var (entity, exp) in world.GetComponents<ExperienceComponent>())
            {
                // 检查是否升级
                while (exp.current_exp >= exp.exp_to_next_level)
                {
                    LevelUp(world, entity, exp);
                }
            }
        }

        private void LevelUp(World world, int entity, ExperienceComponent exp)
        {
            // 减去升级所需经验
            exp.current_exp -= exp.exp_to_next_level;
            
            // 升级
            exp.level++;
            
            // 计算下一级所需经验
            exp.exp_to_next_level = CalculateExpForLevel(exp.level + 1);
            
            Debug.Log($"Level Up! New Level: {exp.level}, Next Level EXP: {exp.exp_to_next_level}");

            // 添加升级事件组件
            if (!world.HasComponent<LevelUpEventComponent>(entity))
            {
                world.AddComponent(entity, new LevelUpEventComponent(exp.level));
            }
            else
            {
                var levelUpEvent = world.GetComponent<LevelUpEventComponent>(entity);
                levelUpEvent.new_level = exp.level;
                levelUpEvent.processed = false;
            }

            // 升级奖励
            ApplyLevelUpRewards(world, entity, exp.level);
        }

        private void ApplyLevelUpRewards(World world, int entity, int newLevel)
        {
            // 恢复生命值
            if (world.HasComponent<HealthComponent>(entity))
            {
                var health = world.GetComponent<HealthComponent>(entity);
                health.current = health.max;
            }

            // 增加属性（可根据需要扩展）
            // 例如：每5级增加最大生命值
            if (newLevel % 5 == 0 && world.HasComponent<HealthComponent>(entity))
            {
                var health = world.GetComponent<HealthComponent>(entity);
                health.max += 10f;
                health.current = health.max;
                Debug.Log($"Max Health increased to {health.max}");
            }

            // 增加拾取范围
            if (newLevel % 3 == 0 && world.HasComponent<PickupRangeComponent>(entity))
            {
                var pickupRange = world.GetComponent<PickupRangeComponent>(entity);
                pickupRange.radius += 0.2f;
                Debug.Log($"Pickup Range increased to {pickupRange.radius}");
            }

            // 播放升级特效和音效（TODO）
        }

        /// <summary>
        /// 计算指定等级所需的经验值
        /// </summary>
        private float CalculateExpForLevel(int level)
        {
            // 使用指数增长公式: BASE_EXP * (GROWTH_RATE ^ (level - 1))
            return BASE_EXP * Mathf.Pow(EXP_GROWTH_RATE, level - 1);
        }

        /// <summary>
        /// 获取总共需要的经验（从1级到目标等级）
        /// </summary>
        public static float GetTotalExpForLevel(int targetLevel)
        {
            float total = 0f;
            for (int i = 1; i <= targetLevel; i++)
            {
                total += BASE_EXP * Mathf.Pow(EXP_GROWTH_RATE, i - 1);
            }
            return total;
        }
    }
}
