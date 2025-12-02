using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Solitaire : MonoBehaviour
{
    public string[] suits = { "C", "D", "H", "S" };
    public string[] ranks = { "A", "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K" };
    public Sprite[] cardFaces;
    public Sprite cardBack, emptyPlace;
    public GameObject[] foundationPositions, tableauPositions, freecellPositions;
    public GameObject cardPrefab;
    public List<string> deck;
    public List<string>[] foundations, tableaus, freecells;
    public List<string> freecell0, freecell1, freecell2, freecell3 = new List<string>();
    public List<string> foundation0, foundation1, foundation2, foundation3 = new List<string>();
    public List<string> tableau0, tableau1, tableau2, tableau3, tableau4, tableau5, tableau6, tableau7 = new List<string>();
    private System.Random rng = new System.Random();
    private Vector3 cardOffset = new Vector3(0f, -.3f, -0.1f);
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        tableaus = new List<string>[] { tableau0, tableau1, tableau2, tableau3, tableau4, tableau5, tableau6, tableau7 };
        foundations = new List<string>[] { foundation0, foundation1, foundation2, foundation3 };
        freecells = new List<string>[] { freecell0, freecell1, freecell2, freecell3 };
        PlayGame();
    }

    void PlayGame()
    {
        deck = GenerateDeck();
        //foreach (string card in deck)Debug.Log(card);
        Deal();
    }

    List<string> GenerateDeck()
    {
        List<string> newDeck = new List<string>();
        foreach (string suit in suits) foreach (string rank in ranks) newDeck.Add(suit + rank);
        //shuffle
        newDeck = newDeck.OrderBy(x => rng.Next()).ToList();
        return newDeck;
    }

    void Deal()
    {
        //Debug.Log("Dealing cards...");
        int tabIndex = 0;
        List<string> currentTab;
        for (int i = deck.Count - 1; i >= 0; i--)
        {
            if (tabIndex > 7) break;
            string card = deck[i];
            currentTab=tableaus[tabIndex];
            

            deck.RemoveAt(i);
            currentTab.Add(card);

            //blursed af logic, do not touch
            if(tabIndex < 4) {if(currentTab.Count >= 7) tabIndex++;}
            else if(currentTab.Count >= 6) tabIndex++;
        }

        foreach (GameObject tabPosition in tableauPositions)
        {
            //Debug.Log("Dealing to tableau position " + tabPosition.name);
            int index = Array.IndexOf(tableauPositions, tabPosition);
            Vector3 currentPosition = tabPosition.transform.position + new Vector3(0, 0, -.1f);
            foreach (string card in tableaus[index])
            {
                //Debug.Log("Dealing card " + card + " to tableau " + index);
                // create card
                CreateCard(card, currentPosition, tabPosition.transform);
                currentPosition += cardOffset;
            }
        }
    }

    void CreateCard(string cardName, Vector3 position, Transform parent)
    {
        //Debug.Log("Creating card " + cardName + " at " + position);
        GameObject newCard = Instantiate(cardPrefab, position, Quaternion.identity, parent);
        newCard.name = cardName;
        Sprite cardFace = cardFaces.FirstOrDefault(s => GetName(s.name) == cardName) ?? throw new ArgumentNullException(cardName + " is missing a face");
        newCard.GetComponent<CardSprite>().cardFace = cardFace;
    }

    string GetName(string card_name)
    {
        string newName = "";
        string num = "";
        int val;

        if(card_name.Contains("Diamonds"))newName+='D';
        else if(card_name.Contains("Clubs"))newName+='C';
        else if(card_name.Contains("Hearts"))newName+='H';
        else if(card_name.Contains("Spades"))newName+='S';
        else throw new ArgumentNullException("suite not found");

        for(int i=0; i< card_name.Length-1; i++) if(Char.IsDigit(card_name[i])) num += card_name[i];
        if(num.Length>0)val = int.Parse(num);
        else throw new ArgumentNullException("number not found");
        newName+=ranks[val-1];

        return newName;
    }

    public bool IsValidMove(GameObject cardObject, GameObject targetObject)
    {
        if (cardObject == targetObject || cardObject == null || targetObject == null) return false;
        ResolveTarget(targetObject, out GameObject clickedTag, out int foundationIndex, out int tabIndex);

        // free cell -> tab/foundation
        if (cardObject.transform.parent.CompareTag("Freecell"))
        {
            if (clickedTag.transform.CompareTag("Tableau") && tabIndex >= 0)
            {
                //Debug.Log("can place on tab: " + CanPlaceOnTableau(cardObject.name, tabIndex));
                return CanPlaceOnTableau(cardObject.name, tabIndex);
            }
            if (clickedTag.transform.CompareTag("Foundation") && foundationIndex >= 0)
            {
                //Debug.Log("can place on found: " + CanPlaceOnFoundation(cardObject.name, foundationIndex));
                return CanPlaceOnFoundation(cardObject.name, foundationIndex);
            }
            if (clickedTag.transform.CompareTag("Freecell"))return CanPlaceOnFreecell(cardObject);
            return false;
        }

        // foundation -> tab
        if (cardObject.transform.parent.CompareTag("Foundation"))
        {
            if (clickedTag.transform.CompareTag("Tableau") && tabIndex >= 0)
            {
                //Debug.Log("can place on tab from found: " + CanPlaceOnTableau(cardObject.name, tabIndex));
                return CanPlaceOnTableau(cardObject.name, tabIndex);
            }
            if (clickedTag.transform.CompareTag("Freecell"))return CanPlaceOnFreecell(cardObject);
            //Debug.Log("bad found to tab click");
            return false;
        }

        // tab -> tab/foundation
        if (cardObject.transform.parent.CompareTag("Tableau"))
        {
            if (clickedTag.transform.CompareTag("Tableau") && tabIndex >= 0)
            {
                //Debug.Log("can place on tab from tab: " + CanPlaceOnTableau(cardObject.name, tabIndex));
                return CanPlaceOnTableau(cardObject.name, tabIndex);
            }
            if (clickedTag.transform.CompareTag("Foundation") && foundationIndex >= 0)
            {
                if (IsBlocked(cardObject))
                {
                    //Debug.Log("Blocked from tab->tab/foundation");
                    return false;
                }
                //Debug.Log("can place on found from tab: " + CanPlaceOnFoundation(cardObject.name, foundationIndex));
                return CanPlaceOnFoundation(cardObject.name, foundationIndex);
            }
            if (clickedTag.transform.CompareTag("Freecell"))return CanPlaceOnFreecell(cardObject);
            //Debug.Log("Bad tab to tab/found click");
            return false;
        }
        //Debug.Log("nothing matched. returning false");
        return false;
    }

    public void MoveCardsAbove(GameObject origParent, int originalTabIndex, int destTabIndex, int cardsToMoveCount, GameObject clickedTag, GameObject cardObject)
    {
        if (originalTabIndex == -1 || cardsToMoveCount <= 1) return;
        List<string> origTab = tableaus[originalTabIndex];
        int origCount = origTab.Count;
        int origIndex = origCount - cardsToMoveCount + 1;
        for (int i = 0; i < cardsToMoveCount - 1; i++)
        {
            string movingCardName = origTab[origIndex];
            origTab.RemoveAt(origIndex);
            tableaus[destTabIndex].Add(movingCardName);
            GameObject movingCardObj = null;
            foreach (Transform child in origParent.transform)
            {
                if (child.gameObject.name == movingCardName)
                {
                    movingCardObj = child.gameObject;
                    break;
                }
            }
            if (movingCardObj != null)
            {
                movingCardObj.transform.parent = clickedTag.transform;
                movingCardObj.transform.position = cardObject.transform.position + (cardOffset * (i + 1));
            }
        }
    }

    public void PlaceCard(GameObject cardObject, GameObject targetObject)
    {
        if (cardObject == targetObject || cardObject == null || targetObject == null) return;
        int originalTabIndex = -1;
        int cardsToMoveCount = 1;
        ResolveTarget(targetObject, out GameObject clickedTag, out int foundationIndex, out int tabIndex);
        GameObject originalParent = cardObject.transform.parent.gameObject;
        // if coming from tab, need to remove card and all cards on top of it from their original tab
        if (cardObject.transform.parent.CompareTag("Tableau"))
        {
            foreach (List<string> tableau in tableaus)
            {
                if (tableau.Contains(cardObject.name))
                {
                    originalTabIndex = System.Array.IndexOf(tableaus, tableau);
                    cardsToMoveCount = tableau.Count - tableau.IndexOf(cardObject.name);
                    tableau.Remove(cardObject.name);
                    break;
                }
            }
        }
        // if coming from foundation, remove card from correct foundation
        if (cardObject.transform.parent.CompareTag("Foundation"))
        {
            foreach (List<string> foundation in foundations)
            {
                if (foundation.Contains(cardObject.name))
                {
                    foundation.Remove(cardObject.name);
                }
            }
        }
        if (cardObject.transform.parent.CompareTag("Freecell"))
        {
            foreach(List<string> freecell in freecells)
            {
                if(freecell.Contains(cardObject.name)) freecell.Remove(cardObject.name);
            }
        }

        // if moving to tab, add the card to the correct tab
        if (clickedTag.transform.CompareTag("Tableau"))
        {
            // add it to the right tab
            int tableauIndex = System.Array.IndexOf(tableauPositions, clickedTag);
            tableaus[tableauIndex].Add(cardObject.name);
            // move the card position
            if (tableaus[tableauIndex].Count == 1)
                cardObject.transform.position = targetObject.transform.position + new Vector3(0f, 0f, -.03f);
            else
                cardObject.transform.position = targetObject.transform.position + cardOffset;
            // update parent
            cardObject.transform.parent = clickedTag.transform;
            // move all other cards on top of the original cardObject (probably put this in a helper function)
            MoveCardsAbove(originalParent, originalTabIndex, tableauIndex, cardsToMoveCount, clickedTag, cardObject);
        }
        // if moving to foundation, add card to correct foundation
        if (clickedTag.transform.CompareTag("Foundation"))
        {
            int fIndex = System.Array.IndexOf(foundationPositions, clickedTag);
            foundations[fIndex].Add(cardObject.name);
            cardObject.transform.position = targetObject.transform.position + new Vector3(0f, 0f, -.03f);
            cardObject.transform.parent = clickedTag.transform;
        }
        if (clickedTag.transform.CompareTag("Freecell"))
        {
            int cIndex = System.Array.IndexOf(freecellPositions, clickedTag);
            freecells[cIndex].Add(cardObject.name);
            cardObject.transform.position = targetObject.transform.position + new Vector3(0f, 0f, -.03f);
            cardObject.transform.parent = clickedTag.transform;
        }
    }

    public bool IsLastInTab(GameObject cardObject)
    {
        foreach (List<string> tab in tableaus)
        {
            if (tab.Count > 0 && tab.Last() == cardObject.name)
            {
                return true;
            }
        }
        return false;
    }

    public bool IsBlocked(GameObject cardObject)
    {
        foreach (Transform child in cardObject.transform.parent)
        {
            if (child.gameObject != cardObject && child.position.z < cardObject.transform.position.z)
            {
                return true;
            }
        }
        return false;
    }

    public bool IsAlternatingColor(string card1, string card2)
    {
        if (card1 == null || card2 == null) return false;
        char suit1 = card1[0];
        char suit2 = card2[0];
        bool isRed1 = (suit1 == 'D' || suit1 == 'H');
        bool isRed2 = (suit2 == 'D' || suit2 == 'H');
        return isRed1 != isRed2;
    }

    public bool IsSameSuit(string card1, string card2)
    {
        if (card1 == null || card2 == null) return false;
        return card1[0] == card2[0];
    }

    public bool IsOneRankHigher(string card1, string card2)
    {
        if (card1 == null || card2 == null) return false;
        int rank1 = Array.IndexOf(ranks, card1.Substring(1));
        int rank2 = Array.IndexOf(ranks, card2.Substring(1));
        Debug.Log("rank1: " + rank1);
        Debug.Log("rank2: " + rank2);
        return rank1 == (rank2 + 1) % ranks.Length;
    }

    public bool IsOneRankLower(string card1, string card2)
    {
        if (card1 == null || card2 == null) return false;
        int rank1 = Array.IndexOf(ranks, card1.Substring(1));
        int rank2 = Array.IndexOf(ranks, card2.Substring(1));
        return (rank1 + 1) % ranks.Length == rank2;
    }

    public bool CanPlaceOnFoundation(string card, int foundationIndex)
    {
        if (foundations[foundationIndex].Count == 0)
        {
            return card.Substring(1) == "A";
        }
        string topCard = foundations[foundationIndex].Last();
        Debug.Log("topCard: " + topCard + ", card: " + card);
        Debug.Log("IsSameSuit: " + IsSameSuit(card, topCard));
        Debug.Log("IsOneRankHigher: " + IsOneRankHigher(card, topCard));
        return IsSameSuit(card, topCard) && IsOneRankHigher(card, topCard);
    }

    public bool CanPlaceOnTableau(string card, int tableauIndex)
    {
        if (tableaus[tableauIndex].Count == 0)
        {
            return card.Substring(1) == "K";
        }
        string topCard = tableaus[tableauIndex].Last();
        return IsAlternatingColor(card, topCard) && IsOneRankLower(card, topCard);
    }

    public bool CanPlaceOnFreecell(GameObject card)
    {
        return !IsBlocked(card);
    }

    void ResolveTarget(GameObject toLocation, out GameObject clickedTag, out int foundationIndex, out int tableauIndex)
    {
        clickedTag = toLocation.transform.CompareTag("Card") ? toLocation.transform.parent.gameObject : toLocation;
        foundationIndex = -1;
        tableauIndex = -1;
        if (clickedTag.transform.CompareTag("Foundation"))
            foundationIndex = System.Array.IndexOf(foundationPositions, clickedTag);
        else if (clickedTag.transform.CompareTag("Tableau"))
            tableauIndex = System.Array.IndexOf(tableauPositions, clickedTag);
    }
}
