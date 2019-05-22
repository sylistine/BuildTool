using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Djn.Builds {
    public class BuildData : ScriptableObject {
        [SerializeField] private string _name = "NewBuildTarget";
        [SerializeField] private Level _startupLevel;
        [SerializeField] private LevelDataList _levelDataList;

        public string Name => _name;
        public Level StartupLevel => _startupLevel;
        public LevelDataList LevelDatas => _levelDataList;

        public bool IsValid {
            get {
                // TODO: Check for empty strings for build or level names?
                if(string.IsNullOrEmpty(StartupLevel.MainScene.Path)) return false;
                foreach(var scene in StartupLevel.SubScenes) if(string.IsNullOrEmpty(scene.Path))
                    return false;

                foreach(var level in LevelDatas) {
                    if(string.IsNullOrEmpty(level.Data.MainScene.Path)) return false;
                    foreach(var scene in level.Data.SubScenes) if(string.IsNullOrEmpty(scene.Path))
                        return false;
                }

                return true;
            }
        }

        public List<Scene> CompleteSceneList {
            get {
                // compile a scene list from build data.
                var sceneList = new List<Scene>();
                sceneList.Add(StartupLevel.MainScene);
                foreach(var scene in StartupLevel.SubScenes) {
                    if(!sceneList.Contains(scene)) sceneList.Add(scene);
                }

                foreach(var level in LevelDatas) {
                    if(!sceneList.Contains(level.Data.MainScene)) sceneList.Add(level.Data.MainScene);
                    foreach(var scene in level.Data.SubScenes) if(!sceneList.Contains(scene))
                            sceneList.Add(scene);
                }
                return sceneList;
            }
        }

        [Serializable] public class LevelDataList : IEnumerable<LevelData> {
            [SerializeField] private LevelData[] _data;

            public int Length => _data.Length;
            public LevelData this[int i] => _data[i];

            public IEnumerator<LevelData> GetEnumerator() {
                return new LevelEnumerator(_data);
            }

            IEnumerator IEnumerable.GetEnumerator() {
                return GetEnumerator();
            }

            public class LevelEnumerator : IEnumerator<LevelData> {
                private LevelData[] _levels;
                private int currentIdx = -1;

                public LevelEnumerator(LevelData[] levels) {
                    _levels = levels;
                }

                ~LevelEnumerator() {
                    Dispose(false);
                }

                // IEnumerator implementation.
                public LevelData Current => _levels[currentIdx];

                object IEnumerator.Current => _levels[currentIdx];

                public bool MoveNext() {
                    currentIdx++;
                    return currentIdx < _levels.Length;
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
                    _disposed = true;
                }
            }
        }
    }
}
