using System.Collections.Generic;
using UnityEngine;

namespace Tests.TestScenes
{
    using LurkingNinja.Dependency.Attributes;

    [DiTestHelpers]
    public partial class FindTestRun : MonoBehaviour
    {
        [Find("TestChild")][IgnoreSelf][SerializeField]
        private GameObject findDirectionalLight;

        [Find("TestChild")][IgnoreSelf][SerializeField]
        private GameObject[] findArrayByName;

        [Find("TestChild")][IncludeInactive][SerializeField]
        private List<GameObject> findListByName;
        
        [Get][SerializeField]
        private BoxCollider getBoxCollider;

        [Get][SerializeField]
        private BoxCollider[] getManyBoxColliders;

        [Add][SerializeField]
        private HingeJoint noHingeJointUntilAdded;

        [Add][SerializeField]
        private SphereCollider[] noSphereCollidersUntilAdded;
        
        [Find("TestChild")][Get][SerializeField]
        private BoxCollider findFirstBoxColliderOnManyGameObject;
        
        [Find("TestChild")][Get][SerializeField]
        private BoxCollider[] findManyBoxColliderOnManyGameObject;
        
        [FindWithTag("MainCamera")][SerializeField]
        private GameObject findMainCameraWithTag;
        
        [FindWithTag("EditorOnly")][SerializeField]
        private GameObject[] findEditorOnlyGameObjects;
        
        [FindWithTag("EditorOnly")][Get][SerializeField]
        private MeshRenderer findFirstMeshRendererGameObjectsWithTag;
        
        [FindWithTag("EditorOnly")][Get][SerializeField]
        private MeshRenderer[] findManyMeshRendererGameObjectsWithTag;
        
        [GetInChildren][SerializeField]
        private Rigidbody findFirstRigidBodyInChildren;
        
        [GetInChildren][IgnoreSelf][SerializeField]
        private Rigidbody findFirstRigidBodyInChildrenExcludeSelf;
        
        [GetInChildren][SerializeField]
        private Rigidbody[] findRigidBodiesInChildren;
        
        [GetInParent][IgnoreSelf][SerializeField]
        private Rigidbody findFirstRigidBodyInParentExcludeSelf;
        
        [GetInParent][SerializeField]
        private Rigidbody[] findRigidBodiesInParent;
    }
}
