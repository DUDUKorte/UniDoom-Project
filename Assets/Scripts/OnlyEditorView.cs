using System;
using UnityEngine;

public class OnlyEditorView : MonoBehaviour
{
    public Mesh objectMeshes;
    
    private void OnDrawGizmos()
    {
        Gizmos.DrawMesh(objectMeshes);
    }
}
