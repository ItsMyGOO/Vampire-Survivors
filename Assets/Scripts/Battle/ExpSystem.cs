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

            Debug.Log($"[ExpSystem] Level Up → {exp.level.Value}");

            RollUpgradeOptions();
        }

        // =========================
        // Upgrade
        // =========================

        private void RollUpgradeOptions()
        {
            var options = upgradeService.RollOptions(
                optionCount: 3,
                playerLevel: exp.level.Value,
                ownedWeapons: PlayerUpgradeState.GetWeapons(playerEntity),
                passiveLevels: PlayerUpgradeState.GetPassives(playerEntity)
            );

            if (options == null || options.Count == 0)
            {
                Debug.LogWarning("[ExpSystem] No upgrade options");
                return;
            }

            OnUpgradeOptionsReady?.Invoke(options);
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
    }
}