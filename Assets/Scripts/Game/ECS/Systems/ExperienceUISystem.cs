using ECS.Core;
using UnityEngine;
using UnityEngine.UI;

namespace ECS.Systems
{
    /// <summary>
    /// 经验UI系统
    /// 职责: 更新经验条和等级显示
    /// </summary>
    public class ExperienceUISystem : SystemBase
    {
        // UI引用（需要在Inspector中设置）
        private Slider expBar;
        private Text levelText;
        private Text expText;
        private GameObject levelUpPanel;

        private float levelUpPanelTimer = 0f;
        private const float LEVEL_UP_DISPLAY_TIME = 3f;

        public void Initialize(Slider expBar, Text levelText, Text expText, GameObject levelUpPanel = null)
        {
            this.expBar = expBar;
            this.levelText = levelText;
            this.expText = expText;
            this.levelUpPanel = levelUpPanel;

            if (levelUpPanel != null)
            {
                levelUpPanel.SetActive(false);
            }
        }

        public override void Update(World world, float deltaTime)
        {
            // 更新升级面板计时器
            if (levelUpPanelTimer > 0f)
            {
                levelUpPanelTimer -= deltaTime;
                if (levelUpPanelTimer <= 0f && levelUpPanel != null)
                {
                    levelUpPanel.SetActive(false);
                }
            }

            // 找到玩家
            int playerId = FindPlayer(world);
            if (playerId == -1) return;

            if (!world.HasComponent<ExperienceComponent>(playerId)) return;

            var exp = world.GetComponent<ExperienceComponent>(playerId);

            // 更新经验条
            if (expBar != null)
            {
                expBar.value = exp.current_exp / exp.exp_to_next_level;
            }

            // 更新等级文本
            if (levelText != null)
            {
                levelText.text = $"Lv.{exp.level}";
            }

            // 更新经验文本
            if (expText != null)
            {
                expText.text = $"{Mathf.FloorToInt(exp.current_exp)}/{Mathf.FloorToInt(exp.exp_to_next_level)}";
            }

            // 检查升级事件
            if (world.HasComponent<LevelUpEventComponent>(playerId))
            {
                var levelUpEvent = world.GetComponent<LevelUpEventComponent>(playerId);
                if (!levelUpEvent.processed)
                {
                    ShowLevelUpNotification(levelUpEvent.new_level);
                    levelUpEvent.processed = true;
                }
            }
        }

        private void ShowLevelUpNotification(int newLevel)
        {
            if (levelUpPanel != null)
            {
                levelUpPanel.SetActive(true);
                levelUpPanelTimer = LEVEL_UP_DISPLAY_TIME;

                // 更新升级面板文本
                var levelUpText = levelUpPanel.GetComponentInChildren<Text>();
                if (levelUpText != null)
                {
                    levelUpText.text = $"LEVEL UP!\n{newLevel}";
                }
            }

            Debug.Log($"=== LEVEL UP TO {newLevel} ===");
        }

        private int FindPlayer(World world)
        {
            foreach (var (entity, _) in world.GetComponents<PlayerTagComponent>())
            {
                return entity;
            }
            return -1;
        }
    }
}
