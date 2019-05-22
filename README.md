# BuildTool
A utility replacing Unity's scene management system.

Introduces the concept of a 'level' as a named collection of scenes with a single entry scene.
Likewise defines a build target as a named collection of levels with a single entry level.

Build data (build name, level names and their scene dependencies) is built into asset bundles.
The asset bundles can be enabled in the editor, and will be built and copied to builds.

All classes are stored in the Djn (Pronounced "Jihn" as in Djinn) namespace.

## Usage
1) Create a Level asset somewhere that makes sense for your scene collection.
2) Add scenes from your collection to the Level asset.
3) The `Djn.Scene` class can then be used to store serialized scene references. Intra-level loading can then be handled by accessing the Djn.Scene.Path member.
4) Create Build targets in the Build window (Window->Build)
5) Add your Level asset references to your build target (a single Level asset can be used on multiple targets)
6a (editor)) Enable your build target by clicking "Enable Target" in the build window. The editor's Scene list will populate and AssetBundles will be generated.
6b (player)) Click build and select a destination for your build
7) Call `Application.Init()`, and then access build data through `UnityEngine.SceneManagement.SceneManager.LoadScene` and `Application.BuildData.Levels[int].Data.MainScene.Path`*
   * this is verbose and I'm looking for way to shorten it. Perhaps with `Application.BuildData.Levels[int].Load()` ?
