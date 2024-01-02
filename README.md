# Dependency Injection
Simplified dependency Injection for Unity utilizing SourceGeneration making it as performant as possible.
## Installation
You can choose manually installing the package or from GitHub source.
### Add package from git URL
Use the Package Manager's ```+/Add package from git URL``` function.
The URL you should use is this:
```
https://github.com/LurkingNinja/com.lurking-ninja.dependency.git?path=Packages/com.lurking-ninja.dependency
```
### Manual install
1. Download the latest ```.zip``` package from the [Release](https://github.com/LurkingNinja/com.lurking-ninja.dependency/releases) section.
2. Unpack the ```.zip``` file into your project's ```Packages``` folder.
3. Open your project and check if it is imported properly.
## Usage
```csharp
using LurkingNinja.DependencyInjection;
using UnityEngine;

public partial class TestDependencyReceiver : MonoBehaviour
{
    [Get][SerializeField]
    private Collider firstColliderOnThisGameObject;
#if UNITY_EDITOR
    private static void OnValidate()
    {
        // Do your things here OnValidate.
        InjectDependencies();
    }
#endif
}
```
