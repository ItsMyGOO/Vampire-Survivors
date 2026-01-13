using System;
using ECS.Core;
using UnityEngine;
using XLua;
using System.Collections.Generic;
using ECS;

namespace Game.Battle
{
    /// <summary>
    /// 经验和升级系统
    /// 职责: 
    /// 1. 处理经验累积
    /// 2. 检测升级条件
    /// 3. 触发升级并调用 Lua 处理升级逻辑
    /// </summary>
    public class ExpSystem : IExpReceiver
    {
        private static ExpSystem instance;

        public static ExpSystem Instance => instance ??= new ExpSystem();

        // 升级经验曲线配置
        private const float BASE_EXP = 100f;
        private const float EXP_GROWTH_RATE = 1.15f;

        // Lua 升级系统
        private LuaEnv luaEnv;
        private LuaTable luaUpgradeTable;
        private LuaFunction rollOptionsFunc;
        private LuaFunction applyUpgradeFunc;

        private Exp exp;

        public void Init(LuaEnv luaEnv, PlayerContext playerContext)
        {
            this.luaEnv = luaEnv;
            exp = playerContext.Exp;
            // InitializeLuaUpgradeSystem();
        }

        public void AddExp(int value)
        {
            exp.current_exp.Value += value * exp.exp_multiplier;
            while (exp.current_exp.Value >= exp.exp_to_next_level.Value)
            {
                LevelUp();
            }
        }

        private void LevelUp()
        {
            // 减去升级所需经验
            exp.current_exp.Value -= exp.exp_to_next_level.Value;

            // 升级
            exp.level.Value++;

            // 计算下一级所需经验
            exp.exp_to_next_level.Value = CalculateExpForLevel(exp.level.Value + 1);

            Debug.Log($"[ExperienceSystem] Level Up! New Level: {exp.level}");


            // 处理升级选项
            ProcessUpgrade();
        }

        private void ProcessUpgrade()
        {
        }

        private List<UpgradeOption> ParseUpgradeOptions(LuaTable optionsTable)
        {
            if (optionsTable == null) return null;

            List<UpgradeOption> options = new List<UpgradeOption>();


            return options;
        }

        private void ShowUpgradeOptions(List<UpgradeOption> options)
        {
        }

        public void ApplyUpgrade(World world, int entity, string upgradeId)
        {
        }

        private float CalculateExpForLevel(int level)
        {
            return BASE_EXP * Mathf.Pow(EXP_GROWTH_RATE, level - 1);
        }

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

    /// <summary>
    /// 升级选项数据
    /// </summary>
    [Serializable]
    public class UpgradeOption
    {
        public string id;
        public string name;
        public string description;
        public string iconKey;
    }
}