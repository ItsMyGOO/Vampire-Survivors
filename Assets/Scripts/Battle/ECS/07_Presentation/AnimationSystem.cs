using ConfigHandler;
using ECS.Core;

namespace ECS.Systems
{
    /// <summary>
    /// 动画系统
    /// 职责: 更新动画状态
    /// </summary>
    public class AnimationSystem : SystemBase
    {
        public override void Update(World world, float deltaTime)
        {
            var animDb = AnimationConfigDB.Instance;

            var animations = world.GetComponents<AnimationComponent>();

            foreach (var (entity, anim) in animations)
            {
                if (!world.HasComponent<SpriteKeyComponent>(entity))
                    continue;
                var spriteKey = world.GetComponent<SpriteKeyComponent>(entity);

                if (string.IsNullOrEmpty(anim.ClipSetName))
                    continue;

                // ---------- default state 初始化 ----------
                if (string.IsNullOrEmpty(anim.AnimName))
                {
                    anim.AnimName = anim.DefaultAnim;
                    anim.Frame = 1;
                    anim.Time = 0f;
                    anim.Playing = true;
                }

                if (!anim.Playing || string.IsNullOrEmpty(anim.AnimName))
                    continue;

                // ---------- 取动画配置 ----------
                if (!animDb.TryGetClip(
                        anim.ClipSetName,
                        anim.AnimName,
                        out var sheet,
                        out var clip))
                {
                    continue;
                }

                // ---------- 时间推进 ----------
                anim.Time += deltaTime;

                float frameTime = 1f / clip.Fps;

                while (anim.Time >= frameTime)
                {
                    anim.Time -= frameTime;
                    anim.Frame++;

                    // ---------- 播放到末尾 ----------
                    if (anim.Frame > clip.Frames.Length)
                    {
                        if (clip.Loop)
                        {
                            anim.Frame = 1;
                        }
                        else
                        {
                            anim.Playing = false;

                            // ★★★ 自动回退到 defaultState ★★★
                            if (!string.IsNullOrEmpty(anim.DefaultAnim) &&
                                anim.AnimName != anim.DefaultAnim)
                            {
                                anim.AnimName = anim.DefaultAnim;
                                anim.Frame = 1;
                                anim.Time = 0f;
                                anim.Playing = true;
                            }
                            else
                            {
                                anim.Frame = clip.Frames.Length;
                            }

                            break;
                        }
                    }
                }

                // ---------- 写入 Sprite ----------
                spriteKey.sheet = sheet.Sheet;
                spriteKey.key = clip.Frames[anim.Frame - 1]; // Lua → C# 下标修正
            }
        }
    }
}