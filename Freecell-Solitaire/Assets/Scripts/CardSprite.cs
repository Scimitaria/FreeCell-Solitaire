using UnityEngine;

public class CardSprite : MonoBehaviour
{
    public Sprite cardFace;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GetComponent<SpriteRenderer>().sprite = cardFace;
    }
}
