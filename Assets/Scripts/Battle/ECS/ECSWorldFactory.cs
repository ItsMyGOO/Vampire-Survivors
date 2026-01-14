using Cinemachine;
using ECS;
using ECS.Core;
using ECS.Systems;
using UnityEngine;

namespace Battle
{
    public static class ECSWorldFactory
    {
        private static RenderSyncSystem renderSystem;

        public static World Create(CinemachineVirtualCamera camera)
        {
            var world = new World();

            var spriteProvider = new SpriteProvider();
            renderSystem = new RenderSyncSystem(new RenderSystem(spriteProvider, new RenderObjectPool(), camera));

            return world;
        }

        public static void Render(World world)
        {
            renderSystem?.Update(world);
        }
    }
}