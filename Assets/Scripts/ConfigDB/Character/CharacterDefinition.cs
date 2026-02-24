using UnityEngine;

namespace ConfigHandler
{
    /// <summary>
    /// Character visual definition - ScriptableObject
    ///
    /// Holds Unity asset references (Sprite, etc.) that cannot be expressed in JSON.
    /// Maps to CharacterDef (JSON data layer) via characterId.
    /// Battle logic reads only CharacterDef; UI reads both, bridged by characterId.
    ///
    /// Contains no game logic, no ECS component references.
    /// </summary>
    [CreateAssetMenu(
        fileName = "CharDef_New",
        menuName  = "Game/Character/Character Definition",
        order     = 10)]
    public class CharacterDefinition : ScriptableObject
    {
        [Header("Identity")]
        [Tooltip("Must match the id field in character_config.json exactly")]
        public string characterId;

        [Header("Display")]
        [Tooltip("Full portrait shown in character select screen")]
        public Sprite portraitSprite;

        [Tooltip("Small thumbnail for character card (falls back to portraitSprite if null)")]
        public Sprite thumbnailSprite;

        /// <summary>Returns card thumbnail; falls back to portrait if not set.</summary>
        public Sprite CardSprite => thumbnailSprite != null ? thumbnailSprite : portraitSprite;
    }
}