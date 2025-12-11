using UnityEngine;
using UnityEngine.Rendering;

public class BillboardScript : MonoBehaviour
{
  private void Awake()
  {
    RenderPipelineManager.beginCameraRendering += OnBeginCameraRender;
	}


  private void OnDestroy()
  {
    RenderPipelineManager.beginCameraRendering -= OnBeginCameraRender;
  }


	void OnBeginCameraRender(ScriptableRenderContext context, Camera camera)
  {
    transform.rotation = camera.transform.rotation;
	}
}
