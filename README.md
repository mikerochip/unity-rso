# RuntimeScriptableObject

[![Unity Version](https://img.shields.io/badge/Unity-2020.1%2B-blueviolet?logo=unity)](https://unity.com/releases/editor/archive)

This package provides a class called `RuntimeScriptableObject`, or RSO for short.

RSO is the base class for a simple Dependency Injection framework.

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
