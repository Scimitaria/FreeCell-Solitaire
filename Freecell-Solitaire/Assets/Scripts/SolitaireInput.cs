using UnityEngine;
using UnityEngine.InputSystem;

public class SolitaireInput : MonoBehaviour
{
    private bool playAnimation;
    private Solitaire solitaire;
    private GameObject selectedCard,animCard = null;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        solitaire = FindAnyObjectByType<Solitaire>();
        playAnimation=false;
    }

    void Update()
    {
        if (playAnimation&&animCard!=null)
        {
            animCard.GetComponent<Animator>().SetTrigger("wiggle");
            playAnimation=false;
            animCard=null;
        }
    }

    void OnBurst(InputValue value)
    {
        //Debug.Log("Burst");
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, 0f));
        //Debug.Log(worldPosition);
        Collider2D hit = Physics2D.OverlapPoint(worldPosition);

        if (hit != null)
        {
            if (hit.CompareTag("Card"))
            {
                //Debug.Log("Card clicked: " + hit.name);
                if (selectedCard != null)
                {
                    if (selectedCard == hit.gameObject)
                    {
                        selectedCard.GetComponent<SpriteRenderer>().color = Color.white;
                        selectedCard = null;
                        return;
                    }
                    if (solitaire.IsValidMove(selectedCard, hit.gameObject))
                    {
                        //Debug.Log("valid move from " + selectedCard + " to " + hit.gameObject.name);
                        solitaire.PlaceCard(selectedCard, hit.gameObject);
                    }
                    selectedCard.GetComponent<SpriteRenderer>().color = Color.white;
                    selectedCard = null;
                    return;
                }
                if(solitaire.IsBlocked(hit.gameObject))return;
                //Debug.Log("Card selected: " + hit.name);
                selectedCard = hit.gameObject;
                selectedCard.GetComponent<SpriteRenderer>().color = Color.gray;
            }
            else if (hit.CompareTag("Tableau"))
            {
                //Debug.Log("Tableau clicked: " + hit.name);
                if (solitaire.IsValidMove(selectedCard, hit.gameObject))
                {
                    //Debug.Log("valid move from " + selectedCard + " to " + hit.gameObject.name);
                    solitaire.PlaceCard(selectedCard, hit.gameObject);
                    selectedCard.GetComponent<SpriteRenderer>().color = Color.white;
                    selectedCard = null;
                    return;
                }
            }
            else if (hit.CompareTag("Freecell"))
            {
                //Debug.Log("Free cell clicked: " + hit.name);
                if(selectedCard==null)return;
                if (solitaire.IsValidMove(selectedCard, hit.gameObject))
                {
                    //Debug.Log("valid move from " + selectedCard + " to " + hit.gameObject.name);
                    solitaire.PlaceCard(selectedCard, hit.gameObject);
                    selectedCard.GetComponent<SpriteRenderer>().color = Color.white;
                    selectedCard = null;
                    return;
                } else
                {
                    selectedCard.GetComponent<SpriteRenderer>().color = Color.white;
                    selectedCard = null;
                    return;
                }
            }
            else if (hit.CompareTag("Foundation"))
            {
                //Debug.Log("Foundation clicked: " + hit.name);
                if (solitaire.IsValidMove(selectedCard, hit.gameObject))
                {
                    //Debug.Log("valid move from " + selectedCard + " to " + hit.gameObject.name);
                    solitaire.PlaceCard(selectedCard, hit.gameObject);
                    playAnimation=true;
                    animCard=selectedCard;
                    selectedCard.GetComponent<SpriteRenderer>().color = Color.white;
                    selectedCard = null;
                    return;
                }
            }
            else print("no clicky");
        }
    }
}
