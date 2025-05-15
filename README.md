# RuntimeScriptableObject

[![Unity Version](https://img.shields.io/badge/Unity-2020.1%2B-blueviolet?logo=unity)](https://unity.com/releases/editor/archive)

This package provides a class called `RuntimeScriptableObject`, or RSO for short. RSO is the basis of a simple, idiomatic Dependency Injection framework.

* Much lighter weight than all other Unity DI frameworks
* No custom menus, no code generation steps
* Feels like using a built-in Unity framework, not like forcing ASP.NET or Spring Boot into Unity

# Install

See official instructions for how to [Install a Package from a Git URL](https://docs.unity3d.com/Manual/upm-ui-giturl.html). The URL is

`https://github.com/mikerochip/unity-rso.git`

# Usage

Create a subclass of `RuntimeScriptableObject`

```CSharp
using MikeSchweitzer.Rso;

class ThingManager : RuntimeScriptableObject
{
}
```

Enable creation, just like any other `ScriptableObject` subclass

```CSharp
[CreateAssetMenu(fileName = nameof(ThingManager), menuName = "MyGame/Managers/" + nameof(ThingManager))]
class ThingManager : RuntimeScriptableObject
{
}
```

Add serialized fields and `Runtime` lifecycle methods, similar to `MonoBehaviour`

```CSharp
[CreateAssetMenu(fileName = nameof(ThingManager), menuName = "MyGame/Managers/" + nameof(ThingManager))]
class ThingManager : RuntimeScriptableObject
{
    public string ApiUrl;

    public override void RuntimeAwake()
    {
        Debug.Log($"{name}.ApiUrl = {ApiUrl}");
    }
}
```

Now you can create multiple `ThingManager` classes that contain different data for different use cases.

* `MockThingManager` for writing tests
* `LobbyThingManager` for a lobby system
* `ArtTestThingManager` for your artists' test scenes
* etc...
