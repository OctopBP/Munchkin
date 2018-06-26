using UnityEngine;
using TMPro;

public class ClientGM: MonoBehaviour {

	public static ClientGM Instance { get; set; }

	public Munchkin player = new Munchkin();
	public Munchkin enemy = new Munchkin();

	public Munchkin GetMunchkin(int num) {
		return num == player.info.number ? player : enemy;
	}

	public WarTable warTable;

    public GameObject cardPrefab;
	public ClientTurnController turnController;

	public GameObject cnnCanvas;
	public GameObject cnnCnnGroup;
	public GameObject cnnWaitGroup;
	public TextMeshProUGUI enemyName;
	public TextMeshProUGUI playerName;

	private void Awake() {
		Instance = this;

		turnController = GetComponent<ClientTurnController>();
    }

	public CardInfo CreateCard(HidenCard.DeckType deckType) {
		HidenCard hidenCard = new HidenCard();;
		hidenCard.deckType = deckType;

		Vector3 deckPos = new Vector3(6.6f, 0.8f, 0);
		Quaternion deckQuat = Quaternion.identity;
		deckQuat.eulerAngles = new Vector3(0, 90, 180);

		GameObject cardGO = Instantiate(cardPrefab, deckPos, deckQuat);
		cardGO.name = hidenCard.deckType.ToString().ToLower() + " card";

		CardInfo cardInfo = cardGO.GetComponent<CardInfo>();
		cardInfo.BuildCard(hidenCard);

		return cardInfo;
	}
	public CardInfo CreateCard(int cardId) {
		Card card = CardManagerData.allCard.Find(c => c.id == cardId);

		Vector3 deckPos = new Vector3(6.6f, 0.8f, 0);
		Quaternion deckQuat = Quaternion.identity;
		deckQuat.eulerAngles = new Vector3(0, 90, 180);

		GameObject cardGO = Instantiate(cardPrefab, deckPos, deckQuat);
		cardGO.name = card.cardType.ToString().ToLower() + "Card (" + card.name + ")";

		CardInfo cardInfo = cardGO.GetComponent<CardInfo>();
		cardInfo.BuildCard(card);

		return cardInfo;
	}

	public void EndTurn() {
		Client.Instance.EndTurn();
	}

	public void Drop(int pNum, int cardId, int closId, string targetSlot) {
		CardInfo cardInfo = GetMunchkin(pNum).hand.GetCard(closId);
		Card card = GetCard(cardId);
		cardInfo.OpenCard(card);
		cardInfo.cardMovment.cardActive = false;

		if (card.cardType == Card.CardType.LVLUP && targetSlot.StartsWith("WT_", System.StringComparison.CurrentCulture)) {
			GetMunchkin(pNum).hand.RemoveCard(cardInfo);
			Destroy(cardInfo.gameObject);
			return;
		}

		switch (targetSlot) {
			case "WEAPON1":		GetMunchkin(pNum).weapon1.AddCard(cardInfo); 	break;
			case "WEAPON2":		GetMunchkin(pNum).weapon2.AddCard(cardInfo);	break;
			case "HEAD":		GetMunchkin(pNum).head.AddCard(cardInfo);		break;
			case "ARMOR":		GetMunchkin(pNum).armor.AddCard(cardInfo); 		break;
			case "SHOES": 		GetMunchkin(pNum).shoes.AddCard(cardInfo); 		break;
			case "CLASS":		GetMunchkin(pNum).munClass.AddCard(cardInfo);	break;
			case "WT_MONSTER":	warTable.AddCard(cardInfo, false);				break;
			case "WT_PLAYER":	warTable.AddCard(cardInfo, true);				break;
		}

		GetMunchkin(pNum).hand.RemoveCard(cardInfo);
	}
	public void OpenDoor(int cardId, bool isMonster) {
		CardInfo cardInfo = CreateCard(cardId);
	
		if (isMonster)
			warTable.StartFight(cardInfo);
		else
			warTable.AddCard(cardInfo, true);
	}

	public Card GetCard(int cardId) {
		return CardManagerData.allCard.Find(c => c.id == cardId);
	}
}