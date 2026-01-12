using System.Collections.Generic;
using XLua;

namespace ConfigHandler
{
    public sealed class AnimationConfigDB
    {
        static AnimationConfigDB _instance;
        public static AnimationConfigDB Instance => _instance;

        public static void Initialize(AnimationConfigDB db)
        {
            _instance = db;
        }

        readonly Dictionary<string, AnimationSetDef> _sets = new();

        internal void AddSet(string setId, AnimationSetDef set)
        {
            _sets[setId] = set;
        }

        public bool TryGetSet(string setId, out AnimationSetDef set)
        {
            return _sets.TryGetValue(setId, out set);
        }

        public bool TryGetClip(
            string setId,
            string clipId,
            out AnimationSetDef set,
            out AnimationClipDef clip)
        {
            clip = null;
            if (!_sets.TryGetValue(setId, out set))
                return false;

            return set.Clips.TryGetValue(clipId, out clip);
        }

        /// <summary>
        /// ECS / Render 直接用的接口
        /// </summary>
        public bool TryGetFrame(
            string setId,
            string clipId,
            int frameIndex,
            out string sheet,
            out string key)
        {
            sheet = null;
            key = null;

            if (!TryGetClip(setId, clipId, out var set, out var clip))
                return false;

            if (frameIndex < 0 || frameIndex >= clip.Frames.Length)
                return false;

            sheet = set.Sheet;
            key = clip.Frames[frameIndex];
            return true;
        }
    }

    public static class AnimationConfigLoader
    {
        public static AnimationConfigDB LoadAll(LuaEnv env)
        {
            var table =
                env.DoString("return require 'Data.animation_db'")[0] as LuaTable;

            var db = new AnimationConfigDB();

            foreach (var setKey in table.GetKeys())
            {
                string setId = setKey.ToString();
                var setTable = table.Get<LuaTable>(setKey);

                var setDef = new AnimationSetDef
                {
                    Sheet = setTable.Get<string>("sheet")
                };

                foreach (var clipKey in setTable.GetKeys())
                {
                    string clipId = clipKey.ToString();
                    if (clipId == "sheet")
                        continue;

                    var clipTable = setTable.Get<LuaTable>(clipKey);
                    var clipDef = BuildClip(clipTable);

                    setDef.Clips.Add(clipId, clipDef);
                }

                db.AddSet(setId, setDef);
            }

            return db;
        }

        static AnimationClipDef BuildClip(LuaTable table)
        {
            string name = table.Get<string>("name");
            int frameCount = table.Get<int>("frame_count");
            float fps = table.Get<float>("fps");
            bool loop = table.Get<bool>("loop");

            return new AnimationClipDef
            {
                Name = name,
                Fps = fps,
                Loop = loop,
                Frames = BuildFrames(name, frameCount)
            };
        }

        static string[] BuildFrames(string name, int frameCount)
        {
            var frames = new string[frameCount];

            if (frameCount == 1)
            {
                frames[0] = name;
            }
            else
            {
                for (int i = 0; i < frameCount; i++)
                    frames[i] = $"{name} {i}";
            }

            return frames;
        }
    }

    public sealed class AnimationSetDef
    {
        public string Sheet;
        public Dictionary<string, AnimationClipDef> Clips = new();
    }

    public sealed class AnimationClipDef
    {
        public string Name;
        public float Fps;
        public bool Loop;
        public string[] Frames;

        public int FrameCount => Frames.Length;
    }
}