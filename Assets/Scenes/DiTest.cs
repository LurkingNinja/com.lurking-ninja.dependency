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
        private void OnValidate() => InjectInEditor();
#endif
        private void Awake() => InjectInPlay();
    }
}