# Base attributes
These are generating queries in order to fill either a field or a property in your class.
It is your responsibility to provide the [SerializeField](https://docs.unity3d.com/ScriptReference/SerializeField.html) or [field:SerializeField](https://forum.unity.com/threads/c-7-3-field-serializefield-support.573988/) attribute depending on
if your field is a field or a property. It is also your responsibility to choose [InjectInRuntime](#InjectInRuntime) when your receiving field is not serializable by Unity.
It is also your responsibility currently to call ```InitializeInEditor()``` from the [OnValidate](https://docs.unity3d.com/ScriptReference/MonoBehaviour.OnValidate.html)
method and/or the ```InitializeInRuntime()``` method either from your [Awake()](https://docs.unity3d.com/ScriptReference/MonoBehaviour.Awake.html) or [Start()](https://docs.unity3d.com/ScriptReference/MonoBehaviour.Start.html) method.
You can opt-in for auto-initialization if you decorate your ```class``` with [[GenerateAwake]](#generateawake), [[GenerateOnValidate]](#generateonvalidate) or [[GenerateInitializers]](#generateinitializers).

[< back](../README.md)

## Get
The most basic attribute, implements a [GetComponent\<T\>()](https://docs.unity3d.com/ScriptReference/GameObject.GetComponent.html) call and store the result in the field or property you
decorate with this attribute. It only searches on the same game object. It can be used with [[Find]](#find) or [[FindWithTag]](#findwithtag) in order to access components on the resulting set of game objects.

Available modifier attributes are: [InjectInRuntime](#InjectInRuntime) and [SkipNullCheck](#skipnullcheck).

Examples:
```csharp
// Find the first BoxCollider component on the current GameObject.
// Also known as GetComponent<BoxCollider>().
[Get][SerializeField]
private BoxCollider getBoxCollider;

// Find all BoxCollider components on the current GameObject.
// Also known as GetComponents<BoxCollider>().
[Get][SerializeField]
private BoxCollider[] getBoxColliders;

// Find all GameObjects named "TestChild" and find the first BoxCollider component on any of them.
[Find("TestChild")][Get][SerializeField]
private BoxCollider getFirstBoxColliderOnGameObjects;

// Find all GameObjects named "TestChild" and find all BoxCollider components on all of them.
[Find("TestChild")][Get][SerializeField]
private BoxCollider[] getAllBoxCollidersOnGameObjects;

// Find all GameObjects tagged with "EditorOnly" and find the first BoxCollider on any of them.
[FindWithTag("EditorOnly")][Get][SerializeField]
private BoxCollider getFirstBoxColliderOnAnyTaggedGameObject;

// Find all GameObjects tagged with "EditorOnly" and find all BoxColliders on all of them.
[FindWithTag("EditorOnly")][Get][SerializeField]
private BoxCollider[] getAllBoxCollidersOnTaggedGameObjects;
```

## Add
In most aspects ```[Add]``` is similar to [[Get]](#get) so please see that entry too, everything is written there applies to ```[Add]``` as well.
```[Add]``` will query the Component as Get but if it finds none, then will add one Component to the game object.

Examples:
```csharp
// Find the first BoxCollider component on the current GameObject.
// If there is none, add one.
// Also known as GetComponent<BoxCollider>().
[Add][SerializeField]
private BoxCollider getBoxCollider;

// Find all BoxCollider components on the current GameObject.
// If there is none, add one.
// Also known as GetComponents<BoxCollider>().
[Add][SerializeField]
private BoxCollider[] getBoxColliders;
```

## Find
This attribute generates a [GameObject.Find(gameObjectName)](https://docs.unity3d.com/ScriptReference/GameObject.Find.html). The ```gameObjectName``` is a
```string``` parameter and should be passed in the ```Find``` attribute.
This will look for any game objects with the specified name. If you use it with [[Get]](#get), you can address the first or all game object in the set which has the correct ```Component``` on it.

Available modifier attributes are: [InjectInRuntime](#injectinruntime) and [SkipNullCheck](#skipnullcheck).

Examples:
```csharp
// Find the first GameObject with the name "TestChild", but ignore the current GameObject.
[Find("TestChild")][IgnoreSelf][SerializeField]
private GameObject findDirectionalLight;

// Find all GameObjects with the name "TestChild", including the current one.
// Results in an array.
[Find("TestChild")][SerializeField]
private GameObject[] findArrayByName;

// Find all GameObjects with the name "TestChild", including the current one and the inactive ones.
// Results in a List<GameObject>.
[Find("TestChild")][IncludeInactive][SerializeField]
private List<GameObject> findListByName;

// Find all GameObjects with the name "TestChild" and return the first BoxCollider component from any of them.
[Find("TestChild")][Get][SerializeField]
private BoxCollider findFirstBoxColliderOnManyGameObject;

// Find all GameObjects with the name "TestChild" and return all the BoxCollider components from all of them.
[Find("TestChild")][Get][SerializeField]
private BoxCollider[] findManyBoxColliderOnManyGameObject;
```

## FindWithTag
Generates a [GameObject.FindWithTag(tagName)](https://docs.unity3d.com/ScriptReference/GameObject.FindWithTag.html). The ```tagName``` should be passed in to the ```FindWithTag``` attribute.

Available modifier attributes are: [InjectInRuntime](#injectinruntime) and [SkipNullCheck](#skipnullcheck).

Examples:
```csharp
// Find all GameObjects tagged with "MainCamera" and return with the first one.
// Also known as GameObject.FindWithTag() call.
[FindWithTag("MainCamera")][SerializeField]
private GameObject theCameraGameObject;

// Find all GameObjects tagged with "EditorOnly" tag.
[FindWithTag("EditorOnly")][SerializeField]
private GameObject[] nonPlayModeGameObjects;

// Find all GameObjects tagged with "Player" and return with the first MeshRenderer on any of them.
[FindWithTag("Player")][Get][SerializeField]
private MeshRenderer playerRenderer;

// Find all GameObjects tagged with "Obstacles" and return with all the BoxColliders on all of them.
[FindWithTag("Obstacles")][Get][SerializeField]
private BoxCollider[] allObstacleColliders;
```

## GetInChildren
Utilizes a [GetComponentInChildren\<T\>(includeInactive)](https://docs.unity3d.com/ScriptReference/Component.GetComponentInChildren.html).
If it is called on [arrays](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/arrays), or [List<T>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1?view=netstandard-2.1) it generates [GameObject.GetComponentsInChildren\<T\>](https://docs.unity3d.com/ScriptReference/Component.GetComponentsInChildren.html).

Available modifier attributes are: [IncludeInactive](#includeinactive), [InjectInRuntime](#injectinruntime) and [SkipNullCheck](#skipnullcheck).

Examples:
```csharp
// Find the first Rigidbody component on this game object or any children.
[GetInChildren][SerializeField]
private Rigidbody rigidBodyInChildren;

// Find the first Rigidbody component on any of the children but excluding the current GameObject.
[GetInChildren][IgnoreSelf][SerializeField]
private Rigidbody rigidBodyOnlyInChildren;

// Find all Rigidbody components on all of the children game objects and on the current one as well.
[GetInChildren][SerializeField]
private Rigidbody[] findRigidBodiesInChildren;
```

## GetInParent
It generates a [GetComponentInParent\<T\>()](https://docs.unity3d.com/ScriptReference/GameObject.GetComponentInParent.html) call.
If it is called on [arrays](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/arrays), or [List<T>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1?view=netstandard-2.1) it generates [GameObject.GetComponentsInChildren\<T\>](https://docs.unity3d.com/ScriptReference/Component.GetComponentsInChildren.html).

Available modifier attributes are: [IncludeInactive](#includeinactive), [InjectInRuntime](#injectinruntime) and [SkipNullCheck](#skipnullcheck).

Examples:
```csharp
// Find the first Rigidbody component on any of the parents but excluding the current GameObject.
[GetInParent][IgnoreSelf][SerializeField]
private Rigidbody firstRigidbodyOnParents;

// Find all Rigidbody components on all of the parent game objects and on the current one as well.
[GetInParent][SerializeField]
private Rigidbody[] findRigidBodiesOnParents;
```