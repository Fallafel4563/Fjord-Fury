using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class BillboardScript : MonoBehaviour
{
    [SerializeField] SpriteRenderer sprite;
    void OnWillRenderObject()
    {
        if (sprite != null && Camera.main != null) 
        /*if (SceneView.currentDrawingSceneView)
        {    
          transform.rotation = Camera.current.transform.rotation;
        }
        else*/
        {
          transform.rotation = Camera.main.transform.rotation;
        }
    }
}

