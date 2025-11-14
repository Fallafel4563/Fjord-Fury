using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class SpriteDraw : MonoBehaviour {
	public float nearPlaneMultiplier;
	public float farPlaneMultiplier;
	public RenderTexture z_buffer;
	public bool isMultiSprite; //used for pseudo-3d cars
	public SpriteRenderer spr;
	public Sprite[] frames; //used for either animation or multi-view.
	public int orderInLayer;

	int[] LUT = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };

	MaterialPropertyBlock spritePriSwitch; //this is set to actually set draw order.

	void Update () {
		spritePriSwitch = new MaterialPropertyBlock ();
		spr.GetPropertyBlock (spritePriSwitch);
		spritePriSwitch.SetFloat ("_Order", orderInLayer);
		spr.SetPropertyBlock (spritePriSwitch);
	}
	void OnWillRenderObject () {
		if (spr != null) {
			if (isMultiSprite) {
				float globalAng = Camera.current.transform.rotation.eulerAngles.y;	
				float localAng = 360-transform.parent.rotation.eulerAngles.y;
				float totalAng = Mathf.Repeat ((360-globalAng) - localAng + 11.25f, 360) / 360;
				spr.sprite = frames [Mathf.Clamp(Mathf.FloorToInt(totalAng * 16), 0, 15)];
			}
			transform.rotation = Camera.current.transform.rotation;
		}
		else {
			spr = GetComponent<SpriteRenderer> ();
		}
	}
}
