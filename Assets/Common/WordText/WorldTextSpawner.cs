using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WorldTextSpawner : MonoBehaviour
{
    public static WorldTextSpawner instance;

    public int poolAmount = 30;
    public GameObject worldTextPrefab;
    public AnimationCurve defaultSizeCurve;

    private int poolIndex = 0;
    private List<Transform> textPool = new();

    private List<WorldTextInstanceInfo> activeTextInstances = new();


    private void Awake()
    {
        instance = this;
    }


    private void Start()
    {
        // Spawn all the objects at the start of the game
        for (int i = 0; i < poolAmount; i++)
        {
            GameObject worldTextObject = Instantiate(worldTextPrefab, Vector3.zero, Quaternion.identity);
            textPool.Add(worldTextObject.transform);
            worldTextObject.SetActive(false);
        }
    }



    private void Update()
    {
        for (int i = 0; i < activeTextInstances.Count; i++)
        {
            WorldTextInstanceInfo textInstanceInfo = activeTextInstances[i];
            // Scale the text instance using the animation curve
            float scale = textInstanceInfo.animationCurve.Evaluate(Time.time - textInstanceInfo.spawnTime);
            textInstanceInfo.transform.localScale = Vector3.one * scale;

            // Disable text instance when the time animation curve has reached its end
            if (Time.time - textInstanceInfo.spawnTime > textInstanceInfo.animationCurve.keys.Last().time)
            {
                activeTextInstances.Remove(textInstanceInfo);
                textInstanceInfo.transform.SetParent(this.transform);
                textInstanceInfo.transform.gameObject.SetActive(false);
            }
        }
    }


    public void SpawnText(string text, Vector3 position, Color color, Transform parent = null, AnimationCurve sizeCurve = null)
    {
        Transform textInstance = textPool[poolIndex];
        poolIndex++;
        poolIndex %= poolAmount;

        textInstance.gameObject.SetActive(true);
        textInstance.GetComponent<WorldText>().SetUpText(text, color);
        textInstance.position = position;
        if (parent == null)
            textInstance.SetParent(this.transform);
        else
            textInstance.SetParent(parent);

        textInstance.localScale = Vector3.zero;
        AnimationCurve curve = sizeCurve != null ? sizeCurve : defaultSizeCurve;

        WorldTextInstanceInfo textInstanceInfo = new();
        textInstanceInfo.spawnTime = Time.time;
        textInstanceInfo.transform = textInstance;
        textInstanceInfo.animationCurve = curve;

        activeTextInstances.Add(textInstanceInfo);
    }
}


public class WorldTextInstanceInfo
{
    public float spawnTime;
    public Transform transform;
    public AnimationCurve animationCurve;
}
