using NUnit.Framework;
using Tests.TestScenes;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Tests.Editor
{
    public class DependencyTests
    {
        private FindTestRun refFindTestRun;
        private Scene scene;
        
        [SetUp]
        public void Init()
        {
            scene = EditorSceneManager
                .OpenScene("Assets/Tests/TestScenes/FindTestScene.unity", OpenSceneMode.Additive);
            SceneManager.SetActiveScene(scene);
            refFindTestRun = GameObject.Find("TestChild").GetComponent<FindTestRun>();
        }
        
        // ---------------------------- Tests Begin
        
        [Test]
        public void Find()
        {
            refFindTestRun.GetfindDirectionalLight = null;
            refFindTestRun.InitializeInEditor();
            Assert.IsNotNull(refFindTestRun.GetfindDirectionalLight);
            Assert.AreNotSame(refFindTestRun.gameObject, refFindTestRun.GetfindDirectionalLight);
        }

        [Test]
        public void FindArray_IgnoreSelf()
        {
            refFindTestRun.GetfindArrayByName = null;
            refFindTestRun.InitializeInEditor();
            Assert.IsNotNull(refFindTestRun.GetfindArrayByName);
            Assert.AreEqual(3, refFindTestRun.GetfindArrayByName.Length);
        }

        [Test]
        public void FindList_IncludeInactive()
        {
            refFindTestRun.GetfindListByName = null;
            refFindTestRun.InitializeInEditor();
            Assert.IsNotNull(refFindTestRun.GetfindListByName);
            Assert.AreEqual(5, refFindTestRun.GetfindListByName.Count);
        }

        [Test]
        public void GetComponent()
        {
            refFindTestRun.GetgetBoxCollider = null;
            refFindTestRun.InitializeInEditor();
            Assert.IsNotNull(refFindTestRun.GetgetBoxCollider);
            Assert.IsInstanceOf<BoxCollider>(refFindTestRun.GetgetBoxCollider);
        }

        [Test]
        public void AddComponent()
        {
            refFindTestRun.GetnoHingeJointUntilAdded = null;
            refFindTestRun.InitializeInEditor();
            Assert.IsNotNull(refFindTestRun.GetnoHingeJointUntilAdded);
            Assert.IsInstanceOf<HingeJoint>(refFindTestRun.GetnoHingeJointUntilAdded);
        }

        [Test]
        public void GetManyComponents()
        {
            refFindTestRun.GetgetManyBoxColliders = null;
            refFindTestRun.InitializeInEditor();
            Assert.IsNotNull(refFindTestRun.GetgetManyBoxColliders);
            Assert.AreEqual(2, refFindTestRun.GetgetManyBoxColliders.Length);
        }
        
        [Test]
        public void AddManyComponents()
        {
            refFindTestRun.GetnoSphereCollidersUntilAdded = null;
            refFindTestRun.InitializeInEditor();
            Assert.IsNotNull(refFindTestRun.GetnoSphereCollidersUntilAdded);
            Assert.AreEqual(1, refFindTestRun.GetnoSphereCollidersUntilAdded.Length);
        }
        
        [Test]
        public void GetComponentOnManyGameObjects()
        {
            refFindTestRun.GetfindFirstBoxColliderOnManyGameObject = null;
            refFindTestRun.InitializeInEditor();
            Assert.IsNotNull(refFindTestRun.GetfindFirstBoxColliderOnManyGameObject);
            Assert.IsInstanceOf<BoxCollider>(refFindTestRun.GetfindFirstBoxColliderOnManyGameObject);
        }
        
        [Test]
        public void GetManyComponentsOnManyGameObjects()
        {
            refFindTestRun.GetfindManyBoxColliderOnManyGameObject = null;
            refFindTestRun.InitializeInEditor();
            Assert.IsNotNull(refFindTestRun.GetfindManyBoxColliderOnManyGameObject);
            Assert.AreEqual(3, refFindTestRun.GetfindManyBoxColliderOnManyGameObject.Length);
        }
        
        [Test]
        public void FindWithTag()
        {
            refFindTestRun.GetfindMainCameraWithTag = null;
            refFindTestRun.InitializeInEditor();
            Assert.IsNotNull(refFindTestRun.GetfindMainCameraWithTag);
            if (Camera.main != null)
                Assert.AreSame(Camera.main.gameObject, refFindTestRun.GetfindMainCameraWithTag);
        }
        
        [Test]
        public void FindGameObjectsWithTag()
        {
            refFindTestRun.GetfindEditorOnlyGameObjects = null;
            refFindTestRun.InitializeInEditor();
            Assert.IsNotNull(refFindTestRun.GetfindEditorOnlyGameObjects);
            Assert.AreEqual(3, refFindTestRun.GetfindEditorOnlyGameObjects.Length);
        }
        
        [Test]
        public void FindComponentWithTag()
        {
            refFindTestRun.GetfindFirstMeshRendererGameObjectsWithTag = null;
            refFindTestRun.InitializeInEditor();
            Assert.IsNotNull(refFindTestRun.GetfindFirstMeshRendererGameObjectsWithTag);
            Assert.IsInstanceOf<MeshRenderer>(refFindTestRun.GetfindFirstMeshRendererGameObjectsWithTag);
        }
        
        [Test]
        public void FindManyComponentsWithTag()
        {
            refFindTestRun.GetfindManyMeshRendererGameObjectsWithTag = null;
            refFindTestRun.InitializeInEditor();
            Assert.IsNotNull(refFindTestRun.GetfindManyMeshRendererGameObjectsWithTag);
            Assert.AreEqual(2, refFindTestRun.GetfindManyMeshRendererGameObjectsWithTag.Length);
        }
        
        [Test]
        public void FindFirstComponentInChildren()
        {
            refFindTestRun.GetfindFirstRigidBodyInChildren = null;
            refFindTestRun.InitializeInEditor();
            Assert.IsNotNull(refFindTestRun.GetfindFirstRigidBodyInChildren);
            Assert.IsInstanceOf<Rigidbody>(refFindTestRun.GetfindFirstRigidBodyInChildren);
            Assert.AreSame(
                refFindTestRun.gameObject, refFindTestRun.GetfindFirstRigidBodyInChildren.gameObject);
        }
        
        [Test]
        public void FindFirstComponentInChildrenExcludeSelf()
        {
            refFindTestRun.GetfindFirstRigidBodyInChildrenExcludeSelf = null;
            refFindTestRun.InitializeInEditor();
            Assert.IsNotNull(refFindTestRun.GetfindFirstRigidBodyInChildrenExcludeSelf);
            Assert.IsInstanceOf<Rigidbody>(refFindTestRun.GetfindFirstRigidBodyInChildrenExcludeSelf);
            Assert.AreNotSame(refFindTestRun.gameObject,
                refFindTestRun.GetfindFirstRigidBodyInChildrenExcludeSelf.gameObject);
        }
                
        [Test]
        public void FindManyComponentInChildren()
        {
            refFindTestRun.GetfindRigidBodiesInChildren = null;
            refFindTestRun.InitializeInEditor();
            Assert.IsNotNull(refFindTestRun.GetfindRigidBodiesInChildren);
            Assert.AreEqual(2, refFindTestRun.GetfindRigidBodiesInChildren.Length);
        }
        
        [Test]
        public void FindFirstComponentInParent()
        {
            refFindTestRun.GetfindFirstRigidBodyInParentExcludeSelf = null;
            refFindTestRun.InitializeInEditor();
            Assert.IsNotNull(refFindTestRun.GetfindFirstRigidBodyInParentExcludeSelf);
            Assert.IsInstanceOf<Rigidbody>(refFindTestRun.GetfindFirstRigidBodyInParentExcludeSelf);
            Assert.AreNotSame(refFindTestRun.gameObject,
                refFindTestRun.GetfindFirstRigidBodyInParentExcludeSelf.gameObject);
        }
                
        [Test]
        public void FindManyComponentInParent()
        {
            refFindTestRun.GetfindRigidBodiesInParent = null;
            refFindTestRun.InitializeInEditor();
            Assert.IsNotNull(refFindTestRun.GetfindRigidBodiesInParent);
            Assert.AreEqual(2, refFindTestRun.GetfindRigidBodiesInParent.Length);
        }
        
        // ---------------------------- Tests End
        
        [TearDown]
        public void DeInit()
        {
            EditorSceneManager.CloseScene(scene, true);
            refFindTestRun = null;
            scene = default;
        }
    }
}
