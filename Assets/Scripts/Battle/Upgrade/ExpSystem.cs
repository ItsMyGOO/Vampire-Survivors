using System;
using Battle.Player;
using ECS.Core;
using UniRx;
using UnityEngine;

namespace Battle.Upgrade
{
    public class ExpData
    {
        public IntReactiveProperty level;
        public FloatReactiveProperty current_exp;
        public FloatReactiveProperty exp_to_next_level;
        public float exp_multiplier; // 经验倍率

        public ExpData()
        {
            level = new(1);
            current_exp = new(0f);
            exp_to_next_level = new(10);
            exp_multiplier = 1.0f;
        }
    }

    /// <summary>
    /// 经验 & 升级系统（重构后）
    /// 只负责：经验 → 升级 → 请求 UpgradeService
    /// </summary>
    public sealed class ExpSystem : IExpReceiver
    {
        public event Action<int> OnLevelUp;

        private const float BASE_EXP = 10f;
        private const float EXP_GROWTH_RATE = 1.15f;

        public ExpData ExpData { get; private set; } = new();
        
        // =========================
        // Exp
        // =========================

        public void AddExp(int value)
        {
            ExpData.current_exp.Value += value * ExpData.exp_multiplier;

            while (ExpData.current_exp.Value >= ExpData.exp_to_next_level.Value)
                LevelUp();
        }

        private void LevelUp()
        {
            ExpData.current_exp.Value -= ExpData.exp_to_next_level.Value;
            ExpData.level.Value++;

            ExpData.exp_to_next_level.Value =
                CalculateExpForLevel(ExpData.level.Value + 1);

            Debug.Log($"[ExpSystem] ========== Level Up → {ExpData.level.Value} ==========");

            OnLevelUp?.Invoke(ExpData.level.Value);
        }

        // =========================
        // Utils
        // =========================

        private static float CalculateExpForLevel(int level)
        {
            return BASE_EXP * Mathf.Pow(EXP_GROWTH_RATE, level - 1);
        }

        // =========================
        // 测试辅助方法
        // =========================

        /// <summary>
        /// 测试方法：直接添加经验值
        /// </summary>
        public void AddExpForTest(float amount)
        {
            Debug.Log($"[ExpSystem] 测试添加经验: {amount}");
            AddExp((int)amount);
        }

        /// <summary>
        /// 测试方法：打印当前状态
        /// </summary>
        public void PrintStatus()
        {
            Debug.Log($"[ExpSystem] 当前状态:");
            Debug.Log($"  等级: {ExpData.level.Value}");
            Debug.Log($"  当前经验: {ExpData.current_exp.Value:F1}");
            Debug.Log($"  升级所需: {ExpData.exp_to_next_level.Value:F1}");

            var upgradeState = PlayerContext.Instance.UpgradeState;
            Debug.Log($"  拥有武器数: {upgradeState.weapons.Count}");
            foreach (var weapon in upgradeState.weapons)
            {
                Debug.Log($"    - {weapon.Key}: Lv.{weapon.Value}");
            }
        }
    }
}