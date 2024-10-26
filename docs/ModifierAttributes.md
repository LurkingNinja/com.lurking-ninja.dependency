# Modifier attributes
These have no effect alone but modifying the behavior of the base attributes or the generation process.

[< back](../README.md)

## GenerateAwake
If you decorate your ```partial class``` with the ```GenerateAwake```  attribute, the system will automatically generate a
```csharp
private void Awake() => InitializeInRuntime();
```
method with this call in order to automatically resolve runtime dependencies. Recommended if you do not use the ```Awake``` method yourself, if you do, place the ```InitializeInRuntime();``` inside your own method.

## GenerateInitializers
If you decorate your ```partial class``` with the ```GenerateInitializers```  attribute, the system will automatically generate both an [Awake](#generateawake) and a [OnValidate](#generateonvalidate) call. See details in their respective section.

#### GenerateOnValidate
If you decorate your ```partial class``` with the ```GenerateOnValidate```  attribute, the system will automatically generate a
```csharp
private void OnValidate() => InitializeInEditor();
```
method with this call in order to automatically resolve editor time dependencies. Recommended if you do not use the ```OnValidate``` method yourself, if you do, place the ```InitializeInEditor();``` inside your own method.

## IgnoreSelf
When  it is allowed to use, it filters out the current game object from the results.
Allowed to use in conjunction with: [Find](#find), [GetInChildren](#getinchildren), [GetInParent](#getinparent).

## IncludeInactive
When it is permitted to use, the search will include inactive components or game objects as well.
Allowed to use in conjunction with: [Find](#find), [GetInChildren](#getinchildren), [GetInParent](#getinparent).

## InjectInEditor
It causes to put the generation into the ```InjectInEditor()``` instead of the ```InjectInRuntime()``` method allowing you
to call it in the Editor (from ```OnValidate``` for example).

## InjectInRuntime
It causes to put the generation into the ```InjectInRuntime()``` instead of the ```InjectInEditor()``` method allowing you
to call it during play-mode or in build.

## SkipNullCheck
If this attribute is added to a field or property, there won't be a null-check performed when a value is added  causing
overwrite no matter if another value is already attached or not.

## StableSort
This attribute only usable with [Find](#find) and causing the found game objects sorted by [InstanceId](https://docs.unity3d.com/ScriptReference/FindObjectsSortMode.InstanceID.html) before return with the result.