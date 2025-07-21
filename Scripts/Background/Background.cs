using UnityEngine;

public class Background : MonoBehaviour
{
    private Renderer _renderer;

    private float _offsetX = 0.0f;
    private float _offsetY = 0.0f;

    public bool Horizontal;
    public bool Vertical;
    public bool InverseHorizontal;
    public bool InverseVertical;

    public float ScrollSpeed;

    private void Start()
    {
        _renderer = GetComponent<Renderer>();
    }

    private void Update()
    {
        if (Horizontal) HorizontalMove();
        if (Vertical) VerticalMove();

        BackgroundMove();
    }

    private void HorizontalMove()
    {
        _offsetX += Time.deltaTime * ScrollSpeed;
        _offsetX = Mathf.Repeat(_offsetX, 1.0f);
    }

    private void VerticalMove()
    {
        _offsetY += Time.deltaTime * ScrollSpeed;
        _offsetY = Mathf.Repeat(_offsetY, 1.0f);
    }

    private void BackgroundMove()
    {
        Vector2 offset = new Vector2(InverseHorizontal ? -_offsetX : _offsetX,
                                     InverseVertical ? -_offsetY : _offsetY);

        _renderer.sharedMaterial.SetTextureOffset("_UnlitColorMap", offset);
    }
}