# BuildTool
A utility replacing Unity's scene management system.

## Concepts
Level: a named collection of scenes with a single entry scene.
Build Target: a named collection of levels with a single entry level.

## Design
Build data helps Unity deal with your application.

It tells Unity what scenes are necessary for a build, then builds that data into asset bundles, exposing useful meta data to your application, such as the names of levels and entrypoint scene paths*.

All classes are stored in the Djn (Pronounced "Jihn" as in Djinn) namespace.

## Usage

### In Editor

Builds targets can be enabled in the editor, and should then behave exactly as it might in a build.

Enabling build targets currently does 2 things:
1. Aggregates scene dependencies and sets them in the Editor Build Scenes list (so that scenes can be loaded in script in-editor).
2. It builds the current target's Build Data into an asset bundle that the editor can use, and saves that bundle to a place where the editor can load it.

### In Builds

Building a target via the build tool generates the same scene list and asset bundle. The bundle is moved to the correct location such that the Unity APIs that search for it behave as expected, with 0 extra work from you.

### Development Steps
0. The build window creates build data assets automatically, and will also autogen a startup level to use as the default for new targets.

After creating a new build target...
1. In the Project window, create a Level asset somewhere that makes sense for your scene collection. This should not be in a Resources folder.
2. Add scenes from your collection to the Level asset. It's okay if your scene exists in some other Level asset.
3. The `Djn.Scene` class can then be used to store serialized scene references. Unity's SceneManager can load a `Djn.Scene` automatically, so use these for **intra-level** scene loading.
4. Add your Level asset references to your build target (a single Level asset can be used on multiple targets)
5a (editor). Enable your build target by clicking "Enable Target" in the build window. The editor's Scene list will populate and AssetBundles will be generated
5b (player). Click build and select a destination for your build
  * build meta data is referenced by loading an asset bundle at the `Application.StreamingAssets` path. The path is different for editor and for builds, but the tool handles this automatically.
6. Call `Application.Init()`, and then access build data through `UnityEngine.SceneManagement.SceneManager.LoadScene` and `Application.BuildData.Levels[int].MainScene.`.

Commandline build commands should be created on a per-app basis.
