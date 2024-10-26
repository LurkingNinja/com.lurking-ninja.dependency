using UnityEngine;

namespace DoTest
{
    using LurkingNinja.Dependency.Attributes;

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