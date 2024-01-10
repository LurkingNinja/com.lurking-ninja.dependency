using System.Collections.Generic;
using System.Linq;
using LurkingNinja.Attributes;
using UnityEngine;

namespace DoTest
{
    // [GenerateOnValidate]
    public partial class DiTest : MonoBehaviour
    {
        // // Receiver can be Property or plain field.
        //
        // // [GenerateOnValidate]
        // // class level - generates OnValidate() instead of InjectInEditor()
        //
        // // [GenerateAwake]
        // // class level - generates Awake() instead of InjectInRuntime()
        //
        // // [InjectInRuntime] - Injection happens in run time (play mode and build)
        // // It can be private and non-serialized or even [NonSerialized]
        //
        // // [InjectInEditor]
        // // Only can happen in Editor if the receiving field or property is serializable.
        // // If the member is not public it has to have [SerializeField] on it.
        //
        // // [SkipNullCheck]
        // // skips null check, overwriting any manual reference
        //
        // // [IgnoreSelf]
        // // In GO searches & in GetInParent & GetInChildren ignores the current GO
        //
        // // [IncludeInactive]
        // // when possible include inactive GOs in searches
        //
        // Find the first GameObject
        // If the TYPE is GameObject you can't have [Get] here
        // GameObject.Find(gameObjectName).First();
        // [Find("Main Camera")][SerializeField]
        // private GameObject oneGameObject;
        // Find many GameObjects
        // If the TYPE is GameObject you can't have [Get] here
        // GameObject.Find(gameObjectName).All();
        // Can use T[]. List<T>
        // [Find("ChildWithAudioSource")][SerializeField]
        // private GameObject[] manyGameObjects;
        // // Find one Component<BoxCollider> on the current GO.
        // // GetComponent<BoxCollider>();
        // [Get][SerializeField]
        // private BoxCollider getBoxCollider;
        // // Find many Component<BoxCollider> on  the current  GO
        // // GetComponentsInChildren<BoxCollider>() filtered on current.
        // [Get][SerializeField]
        // private BoxCollider[] getManyBoxColliders;
        // // Find many GameObject by name and find one component<BoxCollider>
        // // GameObject.Find(gameObjectName).each().GetComponent<BoxCollider>().First();
        // [Find("name")][Get][SerializeField]
        // private BoxCollider findFirstBoxColliderOnManyGameObject;
        // // Find many GameObject by name and find many components<BoxCollider>
        // // GameObject.Find(gameObjectName).each().GetComponent<BoxCollider>().All();
        // // Can use T[]. List<T>
        // [Find("name")][Get][SerializeField]
        // private BoxCollider[] findManyBoxCollidersOnManyGameObjects;
        // // Find the first GameObject by Tag
        // // If the TYPE is GameObject you can't have [Get] here
        // // GameObject.FindWithTag(tagName).First();
        // [FindByTag("tag")][SerializeField]
        // private GameObject oneGameObjectByTag;
        // // Find many GameObjects by Tag
        // // If the TYPE is GameObject you can't have [Get] here
        // // GameObject.FindWithTag(tagName).All();
        // // Can use T[]. List<T>
        // [FindByTag("tag")][SerializeField]
        // private GameObject[] manyGameObjectsByTag;
        // // Find many GameObject by tag and find one Component<BoxCollider>
        // // GameObject.FindWithTag(tagName).each().GetComponent<BoxCollider>().First()
        // [FindByTag("tag")][Get][SerializeField]
        // private BoxCollider findFirstBoxColliderOnManyGameObjectByTag;
        // // Find many GameObject by tag and find many Components<BoxCollider>
        // // GameObject.FindWithTag(gameObjectName).each().GetComponent<BoxCollider>().All()
        // // Can use T[]. List<T>
        // [FindByTag("tag")][Get][SerializeField]
        // private BoxCollider[] findManyBoxCollidersOnManyGameObjectsByTag;
        // // Find first Component<BoxCollider> on all child GOs
        // // GetComponent{s}InChildren<BoxCollider>(includeInactive).First()
        // [GetInChildren][SerializeField]
        // private BoxCollider getOneBoxColliderOnAnyChild;
        // // Find all Component<BoxCollider> on all child GOs
        // // GetComponent{s}InChildren<BoxCollider>(includeInactive).All()
        // // Can use T[]. List<T>
        // [GetInChildren][SerializeField]
        // private BoxCollider[] getManyBoxCollildersOnAnyChild;
        // // Find first Component<BoxCollider> on all parent GOs
        // // GetComponent{s}InParent<BoxCollider>(includeInactive).First()
        // [GetInParent][SerializeField]
        // private BoxCollider getOneBoxColliderOnManyParent;
        // // Find all Component<BoxCollider> on all parent GOs
        // // GetComponent{s}InParent<BoxCollider>(includeInactive).All()
        // // Can use T[]. List<T>
        // [GetInChildren][SerializeField]
        // private BoxCollider[] getManyBoxCollidersOnManyParents;

        private void Awake()
        {
        }
    }
}