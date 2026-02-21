namespace Session
{
    /// <summary>
    /// 游戏会话数据 - 跨场景传递选中角色
    /// 轻量级静态持有，生命周期与进程相同
    /// 仅存储菜单→战斗过渡期间的选择结果，不承载运行时状态
    /// </summary>
    public static class GameSessionData
    {
        /// <summary>
        /// 选中的角色 ID（对应 CharacterDef.id）
        /// 在进入战斗场景前由选人界面写入
        /// </summary>
        public static string SelectedCharacterId { get; private set; } = string.Empty;

        /// <summary>
        /// 由选人界面调用，确认角色选择
        /// </summary>
        public static void SelectCharacter(string characterId)
        {
            SelectedCharacterId = characterId ?? string.Empty;
        }

        /// <summary>
        /// 是否已完成角色选择
        /// </summary>
        public static bool HasSelection => !string.IsNullOrEmpty(SelectedCharacterId);

        /// <summary>
        /// 重置会话（用于返回主菜单时清理状态）
        /// </summary>
        public static void Reset()
        {
            SelectedCharacterId = string.Empty;
        }
    }
}
