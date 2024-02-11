using System.Collections;
using System.Collections.Generic;
using LurkingNinja.Attributes;
using UnityEngine;

namespace TestNameSpace;

[GenerateOnValidate]
public partial class FileScopedNamespaceTest : MonoBehaviour
{
    [Get] public Transform Trs;
}