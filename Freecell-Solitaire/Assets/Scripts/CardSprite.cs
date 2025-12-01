using UnityEngine;

public class CardSprite : MonoBehaviour
{
    public Sprite cardFace;
    private SpriteRenderer spriteRenderer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = cardFace;
    }
}
