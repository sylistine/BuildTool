using UnityEngine;

namespace Djn.Builds {
    [CreateAssetMenu(fileName = "LevelData", menuName = "Level", order = 10000)]
    public class LevelData : ScriptableObject {
        [SerializeField] private Level _level;

        public Level Data => _level;
    }
}
