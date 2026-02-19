namespace UI.Core
{
    /// <summary>
    /// UI面板层级枚举
    /// 用于控制面板的显示顺序
    /// </summary>
    public enum UIPanelLayer
    {
        Background = 0,     // 背景层 (最底层)
        Normal = 100,       // 普通UI层
        Popup = 200,        // 弹出窗口层
        Tips = 300,         // 提示层
        System = 400,       // 系统UI层 (暂停菜单等)
        Top = 500           // 顶层 (加载界面等)
    }

    /// <summary>
    /// UI面板类型
    /// 用于区分不同行为的面板
    /// </summary>
    public enum UIPanelType
    {
        Normal,      // 普通面板 (可被其他面板覆盖)
        Fixed,       // 固定面板 (始终显示,如HUD)
        Popup,       // 弹出面板 (显示时其他面板变暗)
        Exclusive    // 独占面板 (显示时隐藏其他所有面板)
    }
}
