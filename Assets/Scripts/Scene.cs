using System;
using UnityEngine;

namespace Djn.Builds {
    [Serializable]
    public class Scene {
        [SerializeField] private string _guid;
        [SerializeField] private string _path;

        public string GUID => _guid;
        public string Path => _path;

        public static implicit operator string(Scene s) {
            return s.Path;
        }
    }
}
