using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Djn.Builds {
    [Serializable]
    public class Level {
        [SerializeField] private string _name;
        [SerializeField] private Scene _mainScene;
        [SerializeField] private SceneList _subScenes;

        public string Name => _name;
        public Scene MainScene => _mainScene;
        public SceneList SubScenes => _subScenes;

        [Serializable] public class SceneList : IEnumerable<Scene> {
            [SerializeField] private Scene[] _scenes;

            public int Length => _scenes.Length;
            public Scene this[int i] => _scenes[i];

            public IEnumerator<Scene> GetEnumerator() {
                return new SceneEnumerator(_scenes);
            }

            IEnumerator IEnumerable.GetEnumerator() {
                return GetEnumerator();
            }

            public class SceneEnumerator : IEnumerator<Scene> {
                private Scene[] _scenes;
                private int currentIdx = -1;

                public SceneEnumerator(Scene[] scenes) {
                    _scenes = scenes;
                }

                ~SceneEnumerator() {
                    Dispose(false);
                }

                // IEnumerator implementation.
                public Scene Current => _scenes[currentIdx];

                object IEnumerator.Current => _scenes[currentIdx];

                public bool MoveNext() {
                    currentIdx++;
                    return currentIdx < _scenes.Length;
                }

                public void Reset() {
                    currentIdx = -1;
                }

                // IDisposable implementation.
                private bool _disposed;

                public void Dispose() {
                    Dispose(true);
                    GC.SuppressFinalize(this);
                }

                private void Dispose(bool disposing) {
                    if(!_disposed) { }
                    _disposed = true;
                }
            }
        }
    }
}
