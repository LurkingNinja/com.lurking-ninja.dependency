using System.Collections.Generic;
using LurkingNinja.Attributes;
using UnityEngine;

namespace Tests.TestScenes
{
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
