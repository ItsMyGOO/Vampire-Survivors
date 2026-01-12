using System;
using System.Collections.Generic;
using Framework.Config;

namespace ConfigHandler
{
    // =========================
    // Animation Config DB
    // =========================
    public sealed class AnimationConfigDB : SingletonConfigDB<AnimationConfigDB, string, AnimationSetDef>
    {
        public const string ConfigFileName = "animation_config.json";

        public bool TryGetClip(
            string setId,
            string clipId,
            out AnimationSetDef set,
            out AnimationClipDef clip)
        {
            clip = null;
            if (!TryGet(setId, out set))
                return false;

            return set.Clips.TryGetValue(clipId, out clip);
        }

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

        public static AnimationConfigDB Load(string fileName = ConfigFileName)
        {
            var configData = JsonConfigLoader.Load<Dictionary<string, AnimationSetData>>(fileName);
            if (configData == null)
                return null;

            var db = new AnimationConfigDB();

            foreach (var kvp in configData)
            {
                var setId = kvp.Key;
                var setData = kvp.Value;

                var setDef = new AnimationSetDef
                {
                    Sheet = setData.sheet
                };

                foreach (var clipKvp in setData.clips)
                {
                    var clipId = clipKvp.Key;
                    var clipData = clipKvp.Value;

                    var clipDef = new AnimationClipDef
                    {
                        Name = clipData.name,
                        Fps = clipData.fps,
                        Loop = clipData.loop,
                        Frames = BuildFrames(clipData.name, clipData.frameCount)
                    };

                    setDef.Clips.Add(clipId, clipDef);
                }

                db.Add(setId, setDef);
            }

            return db;
        }

        private static string[] BuildFrames(string name, int frameCount)
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

    // =========================
    // JSON Data Classes
    // =========================
    [Serializable]
    public class AnimationSetData
    {
        public string sheet;
        public Dictionary<string, AnimationClipData> clips;
    }

    [Serializable]
    public class AnimationClipData
    {
        public string name;
        public int frameCount;
        public float fps;
        public bool loop;
    }

    // =========================
    // Runtime Definitions
    // =========================
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
