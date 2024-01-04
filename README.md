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
using System.Collections.ObjectModel;
using LurkingNinja.Attributes;
using UnityEngine;

namespace DoTest
{
    public partial class DiTest : MonoBehaviour
    {
        [Get][SerializeField]
        private BoxCollider[] get_BoxColliders;
        
        [Get][SerializeField]
        private BoxCollider get_BoxCollider;
        
        [Get][field: SerializeField]
        private SphereCollider GetOnProperty_SphereCollider { get; set; }
        
        [GetByName(gameObjectName:"Directional Light")][SerializeField]
        private Light getByName_DirectionalLight;
        
        [GetByTag(tag:"MainCamera")][SerializeField]
        private Camera getByTag_MainCamera;
        
        [GetInChildren][IncludeInactive][SerializeField]
        private AudioSource getInChild_AudioSource;
        
        [GetInChildren][IncludeInactive][SerializeField]
        private AudioSource[] getInChildren_AudioSource;
               
        [GetInParent][SerializeField]
        private Rigidbody getInParent_Rigidbody;
        
        [GetInChildren][IncludeInactive][InjectInPlay]
        private Collection<Collider> getInP1arents_Colli1der;
        
        [GetInAssets("AnotherShader")][SerializeField]
        private Shader getInAssets_Shader;
#if UNITY_EDITOR 
        private void OnValidate()
        {
            // Do your own things in OnValidate
            // Then call the InjectInEditor() generated method to inject.
            InjectInEditor();
        }
#endif
        private void Awake()
        {
            // Do your own things in Awake
            // Then call InjectInPlay() generated method to inject.
            InjectInPlay();
        }
    }
}
```
And this code results in this inspector:
![Inspector with injected references](docs/inspector.png)
<br>
<br>
## API
The system is based on attributes. You decorate your fields and properties with the following attributes and
the source generator will generate the appropriate code to put the searched reference in it.
Most references fulfilled in editor time, utilizing the [OnValidate()](https://docs.unity3d.com/ScriptReference/MonoBehaviour.OnValidate.html) editor-only method, but you can opt for
in-play injection too (see [InjectInPlay](#injectinplay) attribute below).
If you do not use the in-play mode, this entire DI system is working through statically serialized references in Unity.

### Base attributes
These are generating queries in order to fill either a field or a property in your class.
It is your responsibility to provide the [SerializeField](https://docs.unity3d.com/ScriptReference/SerializeField.html) or [field:SerializeField](https://forum.unity.com/threads/c-7-3-field-serializefield-support.573988/) attribute depending on
if your field is a field or a property. It is also your responsibility to choose [InjectInPlay](#injectinplay) when your receiving
field is not serializable by Unity  with the only current exception of [Collection<T>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.objectmodel.collection-1?view=netstandard-2.1) type  which is switched
automatically to [InjectInPlay](#injectinplay) mode.
It is also your responsibility currently to call ```InjectInEditor()``` from the [OnValidate](https://docs.unity3d.com/ScriptReference/MonoBehaviour.OnValidate.html)
method and the ```InjectInPlay()``` method either from your [Awake()](https://docs.unity3d.com/ScriptReference/MonoBehaviour.Awake.html) or [Start()](https://docs.unity3d.com/ScriptReference/MonoBehaviour.Start.html) method.
<br>
<br>
#### Get
The most basic attribute, implements a [GetComponent<T>()](https://docs.unity3d.com/ScriptReference/GameObject.GetComponent.html) call and store the result in the field or property you
decorate with this attribute. It only searches on the same game object.

Available modifier attributes are: [InjectInPlay](#injectinplay) and [SkipNullCheck](#skipnullcheck).

```TODO: Allow the usage on Lists, Arrays and Collections.```
<br>
<br>
#### GetByName
This attribute generates a [GameObject.Find(gameObjectName)](https://docs.unity3d.com/ScriptReference/GameObject.Find.html).[GetComponent<T>()](https://docs.unity3d.com/ScriptReference/GameObject.GetComponent.html) line. The ```gameObjectName``` is a
```string``` parameter and should be passed in the ```GetByName``` attribute.
This will look for any game objects with the specified name and the very first will be referenced with
the ```<T>``` component. Due to Unity limitations this won't work cross-scenes. It is not recommended to use this
often in [InjectInPlay](#injectinplay) because it is a slow search. 

Available modifier attributes are: [InjectInPlay](#injectinplay) and [SkipNullCheck](#skipnullcheck).

```TODO: Allow the usage on Lists, Arrays and Collections.```
<br>
<br>
#### GetByTag
Generates a [GameObject.FindWithTag(tagName)](https://docs.unity3d.com/ScriptReference/GameObject.FindWithTag.html).[GetComponent<T>()](https://docs.unity3d.com/ScriptReference/GameObject.GetComponent.html) call. The ```tagName``` should be passed in to the 
```GetByTag``` attribute.
This can be used on [Arrays](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/arrays), [List<T>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1?view=netstandard-2.1) and [Collection<T>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.objectmodel.collection-1?view=netstandard-2.1).

Available modifier attributes are: [InjectInPlay](#injectinplay) and [SkipNullCheck](#skipnullcheck).
<br>
<br>
#### GetInAssets
***TODO***: Document how AssetDatabase Search works here. 
<br>
<br>
#### GetInChildren
Utilizes a [GameObject.GetComponentInChildren<T>(includeInactive)](https://docs.unity3d.com/ScriptReference/Component.GetComponentInChildren.html).
If it is called on arrays, collections or lists it generates [GameObject.GetComponentsInChildren](https://docs.unity3d.com/ScriptReference/Component.GetComponentsInChildren.html).
This can be used on [Arrays](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/arrays), [List<T>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1?view=netstandard-2.1) and [Collection<T>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.objectmodel.collection-1?view=netstandard-2.1).

Available modifier attributes are: [IncludeInactive](#includeinactive), [InjectInPlay](#injectinplay) and [SkipNullCheck](#skipnullcheck).
<br>
<br>
#### GetInParent
It generates a [GameObject.GetComponentInParent](https://docs.unity3d.com/ScriptReference/GameObject.GetComponentInParent.html) call.
If it is called on arrays, collections or lists it generates [GameObject.GetComponentsInParent](https://docs.unity3d.com/ScriptReference/GameObject.GetComponentsInParent.html).
This can be used on [Arrays](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/arrays), [List<T>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1?view=netstandard-2.1) and [Collection<T>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.objectmodel.collection-1?view=netstandard-2.1).
<br>
<br>

#### FindWithTag
***TODO*** It will utilize the [FindGameObjectsWithTag](https://docs.unity3d.com/ScriptReference/GameObject.FindGameObjectsWithTag.html) method.
<br>
<br>
### Modifier attributes
These have no effect alone but modifying the behavior of the base attributes or the generation process.
<br>
<br>
#### IgnoreSelf
When  it is allowed to use, it filters out the current game object from the results.
<br>
<br>
#### IncludeInactive
When it is permitted to use, the search will include inactive components or game objects as well.
<br>
<br>
#### InjectInPlay
It causes to put the generation into the ```InjectInPlay()``` instead of the ```InjectInEditor()``` method allowing you
to call it during play-mode or in build.
<br>
<br>
#### SkipNullCheck
If this attribute is added to a field or property, there won't be a null-check performed when a value is added  causing
overwrite no matter if another value is already attached or not.
<br>
<br>
## TODO
- FindWithTag
- Allow some cases the usage on Lists, Arrays and Collections
- Add more available receiving types.
- Restructure the system to be more modular to perform game object searches and component searches allowing to finda list of one component on multiple game objects or find one game object and multiple components on it or multiple game objects and multiple components on them.
- Building a more robust searching and filtering system, but staying on source generation.
- Adding non-Unity dependency injection of sorts