using UnityEngine;

public class CardInfo : MonoBehaviour {
	
	public bool cardIsOpen;
	public HidenCard selfHidenCard;
	public Card selfCard;

	[HideInInspector] public CardMovment cardMovment;
	public MeshRenderer face;
	public MeshRenderer back;

	void Awake() {
		cardMovment = GetComponent<CardMovment>();
	}

	public void BuildCard(HidenCard hidenCard) {
		cardIsOpen = false;
		cardMovment.State = CardMovment.CardState.CLOSED;
		selfHidenCard = hidenCard;
		SetTextures();
	}
	public void BuildCard(Card card) {
		cardIsOpen = true;
		cardMovment.State = CardMovment.CardState.ACTIVE;
		selfCard = card;
		selfHidenCard = card;
		SetTextures();
	}

	private void SetTextures() {
		string facePath = "CardFaces/";
		string backPath = "CardBack/";

		bool isDoor = selfHidenCard.deckType == HidenCard.DeckType.DOOR;

		facePath += cardIsOpen ? selfCard.texName : isDoor ? "DoorClose" : "TreasureClose";
		backPath += isDoor ? "DoorBack" : "TreasureBack";

		face.material.mainTexture = Resources.Load<Texture>(facePath);
		back.material.mainTexture = Resources.Load<Texture>(backPath);
	}

	public void OpenCard(Card card) {
		cardIsOpen = true;
		selfCard = card;
		SetTextures();
	}

	public bool typeIs(Card.CardType cardType) {
		if (!cardIsOpen)
			return false;
		
		return selfCard.cardType == cardType;
	}
}
