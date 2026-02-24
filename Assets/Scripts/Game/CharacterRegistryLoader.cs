using ConfigHandler;
using UnityEngine;

namespace Game
{
    /// <summary>
    /// CharacterRegistry initializer - MonoBehaviour
    ///
    /// Attach to a persistent GameObject (e.g. GameSceneManager or a dedicated
    /// initializer) so the Registry is ready before any UI Panel calls GetDefinition.
    ///
    /// Single responsibility: call Registry.Initialize() and expose the instance.
    /// No game logic, no ECS references.
    /// </summary>
    public class CharacterRegistryLoader : MonoBehaviour
    {
        [SerializeField] private CharacterRegistry _registry;

        public static CharacterRegistry Instance { get; private set; }

        private void Awake()
        {
            if (_registry == null)
            {
                Debug.LogError("[CharacterRegistryLoader] CharacterRegistry asset not assigned!");
                return;
            }

            _registry.Initialize();
            Instance = _registry;
            Debug.Log("[CharacterRegistryLoader] CharacterRegistry ready.");
        }
    }
}