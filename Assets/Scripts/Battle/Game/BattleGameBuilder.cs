using Battle.Player;
using Battle.Upgrade;
using Battle.View;
using Cinemachine;
using ECS;
using ECS.Core;
using ECS.SyncSystems;
using UI.Model;

namespace Battle
{
    public static class BattleGameBuilder
    {
public static BattleWorldContext Build(CinemachineVirtualCamera vCam)
        {
            GameConfigLoader.LoadAll();

            var world = new World();

            // 从会话数据读取选中的角色配置
            ConfigHandler.CharacterDef characterDef = null;
            string selectedId = Session.GameSessionData.SelectedCharacterId;
            if (!string.IsNullOrEmpty(selectedId))
            {
                bool found = ConfigHandler.CharacterConfigDB.Instance?.TryGetCharacter(selectedId, out characterDef) ?? false;
                if (!found || characterDef == null)
                    UnityEngine.Debug.LogWarning($"[BattleGameBuilder] 未找到角色配置 id='{selectedId}'，CharacterConfigDB 中不存在该 ID，将 fallback 到默认属性。请检查 CharacterRegistry 或角色 ScriptableObject 配置。");
                else
                    UnityEngine.Debug.Log($"[BattleGameBuilder] 角色配置读取成功: {characterDef.id} ({characterDef.displayName})");
            }
            else
            {
                UnityEngine.Debug.LogWarning("[BattleGameBuilder] GameSessionData 中无角色选择，将使用默认属性");
            }
            // characterDef 为 null 时 PlayerFactory 自动 fallback 到默认值
                        // 如果没有选择或读取失败，characterDef 为 null，PlayerFactory 会自动使用默认值

            // Player
            int playerId = PlayerFactory.CreatePlayer(world, characterDef);

            // SpawnController 全局实体：持有生成状态，供 EnemySpawnSystem 读写
            int spawnControllerId = world.CreateEntity();
            world.AddComponent(spawnControllerId, new ECS.SpawnStateComponent(spawnInterval: 1.0f));

            // Core Systems
            ECSSystemInstaller.Install(world);

            // Upgrade Module
            UpgradeWorldInstaller.Install(world, playerId);

            // Render
            var renderSystem = new RenderSystem(
                new SpriteProvider(),
                new RenderObjectPool()
            );

            renderSystem.OnCameraTargetChanged +=
                new CameraFollowController(vCam).SetTarget;

            var renderSync = new RenderSyncSystem(renderSystem);

            // HUD
            var hudViewModel = new HUDViewModel();
            ViewModelRegistry.Register(hudViewModel);

            world.RegisterSystem(new HUDSyncSystem(playerId, hudViewModel));

            // 构建 PlayerContext 供 Debug 工具使用
            world.TryGetService<ExpSystem>(out var expSystem);
            var playerContext = new PlayerContext(world, playerId, expSystem);

            return new BattleWorldContext(world, renderSync, hudViewModel, playerContext);
        }
    }
}