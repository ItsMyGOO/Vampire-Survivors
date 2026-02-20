namespace ECS
{
    /// <summary>
    /// 属性脏标记组件 - Tag Component
    /// 当修改器集合发生变化时，添加此组件标记需要重新计算属性
    /// AttributeCalculationSystem 会在下一帧处理并移除此组件
    /// 
    /// 设计模式：脏标记模式（Dirty Flag Pattern）
    /// 优点：避免每帧对所有实体重复计算属性，大幅提升性能
    /// </summary>
    public struct AttributeDirtyComponent
    {
        // Tag Component 无需字段
        // 仅用于标记实体状态
    }
}