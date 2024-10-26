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
using LurkingNinja.Attributes;
using UnityEngine;

namespace DoTest
{
    [GenerateOnValidate]
    public sealed partial class DiTest : MonoBehaviour
    {
        [Get][SerializeField]
        private BoxCollider[] get_BoxColliders;
        
        [Get][field: SerializeField]
        private SphereCollider GetOnProperty_SphereCollider { get; set; }
        
        [FindWithTag("MainCamera")][SerializeField]
        private Camera getByTag_MainCamera;
        
        [GetInChildren][IncludeInactive][SerializeField]
        private AudioSource getInChild_AudioSource;
        
        [GetInChildren][IncludeInactive][IgnoreSelf][SerializeField]
        private AudioSource[] getInChildren_AudioSource;
    }
}
```
And this code results in this inspector:
![Inspector with injected references](docs/inspector.png)

## API
The system is based on attributes. You decorate your fields and properties with the following attributes and the source generator will generate the appropriate code to put the searched reference in it.
Most references fulfilled in editor time, utilizing the [OnValidate()](https://docs.unity3d.com/ScriptReference/MonoBehaviour.OnValidate.html) editor-only method, but you can opt for runtime injection too (see [InjectInRuntime](#InjectInRuntime) attribute below).
If you do not use the runtime mode, this entire DI system is working through statically serialized references in Unity.

### Basics
Example:
```csharp
[Get][SerializeField]
private BoxCollider getBoxCollider;

[Get][field:SerializeField]
private BoxCollider[] getBoxCollider { get; set; };
```
You can use both fields or properties. When I say ```field``` moving forward I always mean both fields and properties.
If you want to use the editor-time injection, you need make sure your fields and properties are serializeable in Unity. In these examples I always use ```[SerializeField]``` as it is highly recommended as opposed to leave the fields as ```public```. No need to be able to be serialized if you are opting for the run-time injection ([see below](#injectinruntime)).
The type of your field will be used to generate the query. It will only return the type you specify here. If you use array (```GameObject[]```) or ```List<T>``` (```List<BoxCollider>```) then the system tries to generate a broader query (like using ```GetComponents<T>``` call instead of ```GetComponent<T>```) whenever possible.

### [Base attributes](./docs/BaseAttributes.md)
- [Get](./docs/BaseAttributes.md#get)
- [Add](./docs/BaseAttributes.md#add)
- [Find](./docs/BaseAttributes.md#find)
- [FindWithTag](./docs/BaseAttributes.md#findwithtag)
- [GetInChildren](./docs/BaseAttributes.md#getinchildren)
- [GetInParent](./docs/BaseAttributes.md#getinparent)

### [Modifier attributes](./docs/ModifierAttributes.md)
- [GenerateAwake](./docs/ModifierAttributes.md#generateawake)
- [GenerateInitializers](./docs/ModifierAttributes.md#generateinitializers)
- [GenerateOnValidate](./docs/ModifierAttributes.md#generateonvalidate)
- [IgnoreSelf](./docs/ModifierAttributes.md#ignoreself)
- [IncludeInactive](./docs/ModifierAttributes.md#includeinactive)
- [InjectInEditor](./docs/ModifierAttributes.md#injectineditor)
- [InjectInRuntime](./docs/ModifierAttributes.md#injectinruntime)
- [SkipNullCheck](./docs/ModifierAttributes.md#skipnullcheck)
- [StableSort](./docs/ModifierAttributes.md#stablesort)

## TODO
- Reintroduce asset referencing (GetInAssets)
- Develop filtering
- Add C# class (```[Depends]``` - ```[Provides]```) dependency injection
- Check if we can have a [Require] attribute for entries (throwing warning or error in console).