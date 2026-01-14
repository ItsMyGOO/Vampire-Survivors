using System;
using System.Collections.Generic;
using Battle;
using ECS;
using ECS.Core;
using UnityEngine;
using XLua;

namespace Game.Battle
{
    /// <summary>
    /// 经验 & 升级系统（重构后）
    /// 只负责：经验 → 升级 → 请求 UpgradeService
    /// </summary>
    public sealed class ExpSystem : IExpReceiver, IDisposable
    {
        public static ExpSystem Instance { get; } = new();

        private const float BASE_EXP = 10f;
        private const float EXP_GROWTH_RATE = 1.15f;

        private Exp exp;
        private World world;
        private int playerEntity;

        private LuaEnv luaEnv;
        private LuaTable luaUpgradeFlow;
        private Action<LuaTable> luaOnUpgradeOptions;

        private UpgradeService upgradeService;
        
        // 测试模式开关
        [System.NonSerialized]
        public bool testMode = true; // 设为 true 启用自动随机升级测试

        /// <summary>
        /// 升级选项可用事件（UI / Lua 监听）
        /// </summary>
        public event Action<List<UpgradeOption>> OnUpgradeOptionsReady;

        public void Init(LuaEnv luaEnv, PlayerContext ctx, UpgradeService upgradeService)
        {
            this.luaEnv = luaEnv;
            this.upgradeService = upgradeService;
            
            exp = ctx.Exp;
            world = ctx.World;
            playerEntity = ctx.PlayerEntity;

            InitLuaUpgradeFlow();
        }

        private void InitLuaUpgradeFlow()
        {
            var ret = luaEnv.DoString(@"
        local flow = require('battle.upgrade_flow')
        return flow
    ");

            luaUpgradeFlow = ret[0] as LuaTable;
            luaOnUpgradeOptions =
                luaUpgradeFlow.Get<Action<LuaTable>>("OnUpgradeOptions");
        }
        // =========================
        // Exp
        // =========================

        public void AddExp(int value)
        {
            exp.current_exp.Value += value * exp.exp_multiplier;

            while (exp.current_exp.Value >= exp.exp_to_next_level.Value)
                LevelUp();
        }

        private void LevelUp()
        {
            exp.current_exp.Value -= exp.exp_to_next_level.Value;
            exp.level.Value++;

            exp.exp_to_next_level.Value =
                CalculateExpForLevel(exp.level.Value + 1);

            Debug.Log($"[ExpSystem] ========== Level Up → {exp.level.Value} ==========");

            RollUpgradeOptions();
        }

        // =========================
        // Upgrade
        // =========================

        private void RollUpgradeOptions()
        {
            var options = upgradeService.RollOptions(
                optionCount: 3,
                playerLevel: exp.level.Value
            );

            if (options == null || options.Count == 0)
            {
                Debug.LogWarning("[ExpSystem] 没有可用的升级选项");
                return;
            }

            Debug.Log($"[ExpSystem] 生成了 {options.Count} 个升级选项:");
            for (int i = 0; i < options.Count; i++)
            {
                var opt = options[i];
                Debug.Log($"  [{i}] {opt.type} - {opt.name} (ID: {opt.id})");
            }

            // 触发事件
            OnUpgradeOptionsReady?.Invoke(options);

            // 测试模式：自动随机选择一个武器升级选项
            if (testMode)
            {
                AutoSelectUpgradeForTest(options);
            }
        }

        /// <summary>
        /// 测试模式：自动随机选择一个升级选项
        /// </summary>
        private void AutoSelectUpgradeForTest(List<UpgradeOption> options)
        {
            // 优先选择武器类型的选项
            var weaponOptions = options.FindAll(opt => opt.type == UpgradeOptionType.Weapon);
            
            UpgradeOption selected;
            if (weaponOptions.Count > 0)
            {
                // 随机选择一个武器选项
                int randomIndex = UnityEngine.Random.Range(0, weaponOptions.Count);
                selected = weaponOptions[randomIndex];
                Debug.Log($"[ExpSystem] 测试模式 - 随机选择武器: {selected.name} (ID: {selected.id})");
            }
            else if (options.Count > 0)
            {
                // 如果没有武器选项，随机选择任意选项
                int randomIndex = UnityEngine.Random.Range(0, options.Count);
                selected = options[randomIndex];
                Debug.Log($"[ExpSystem] 测试模式 - 随机选择选项: {selected.name} (ID: {selected.id})");
            }
            else
            {
                Debug.LogWarning("[ExpSystem] 测试模式 - 没有可选择的选项");
                return;
            }

            // 应用选择的升级
            ApplyUpgradeOption(selected);
        }

        /// <summary>
        /// 应用升级选项（可由外部UI或测试代码调用）
        /// </summary>
        public void ApplyUpgradeOption(UpgradeOption option)
        {
            if (option == null)
            {
                Debug.LogError("[ExpSystem] 升级选项为空");
                return;
            }

            Debug.Log($"[ExpSystem] 应用升级选项: {option.type} - {option.name} (ID: {option.id})");
            
            try
            {
                UpgradeApplyService.Apply(option);
                Debug.Log($"[ExpSystem] ========== 升级应用完成 ==========\n");
            }
            catch (Exception e)
            {
                Debug.LogError($"[ExpSystem] 应用升级选项时出错: {e.Message}\n{e.StackTrace}");
            }
        }

        // =========================
        // Utils
        // =========================

        private static float CalculateExpForLevel(int level)
        {
            return BASE_EXP * Mathf.Pow(EXP_GROWTH_RATE, level - 1);
        }

        public void Dispose()
        {
            OnUpgradeOptionsReady = null;
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
            Debug.Log($"  等级: {exp.level.Value}");
            Debug.Log($"  当前经验: {exp.current_exp.Value:F1}");
            Debug.Log($"  升级所需: {exp.exp_to_next_level.Value:F1}");
            
            var upgradeState = PlayerContext.Instance.UpgradeState;
            Debug.Log($"  拥有武器数: {upgradeState.weapons.Count}");
            foreach (var weapon in upgradeState.weapons)
            {
                Debug.Log($"    - {weapon.Key}: Lv.{weapon.Value}");
            }
        }
    }
}
