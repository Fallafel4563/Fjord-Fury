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
    transform.LookAt(camera.transform.position, camera.transform.up);
	}
}
