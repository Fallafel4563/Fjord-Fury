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
		if (camera.transform.parent == null) // Removing the SceneCamera from the equation
			return;
		
		// This will not work, as there is no way to change the state of the transform between each camera's rendering
    	transform.LookAt(camera.transform.position, camera.transform.up);
	}
}
