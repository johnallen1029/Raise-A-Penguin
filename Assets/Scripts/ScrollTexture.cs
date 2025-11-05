using UnityEngine;

public class ScrollTexture : MonoBehaviour
{
    public float scrollSpeedX = 0.1f;
    public float scrollSpeedY = 0.1f;

    private Renderer objectRenderer;
    private Vector2 offset;

    void Start()
    {
        objectRenderer = GetComponent<Renderer>();
        offset = Vector2.zero;
    }

    public void Update()
    {
        offset.x += scrollSpeedX * Time.deltaTime;
        offset.y += scrollSpeedY * Time.deltaTime;

        objectRenderer.material.SetTextureOffset("_BaseMap", offset);
        objectRenderer.material.SetTextureOffset("_BumpMap", offset);
    }
}
