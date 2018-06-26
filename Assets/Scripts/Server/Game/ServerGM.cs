using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ServerPlayer {
	public PlayerInfo info = new PlayerInfo();
	public ServerMunchkin munchkin = new ServerMunchkin();
}

public class ServerGM : MonoBehaviour {

	public static ServerGM Instance { get; set; }

	public ServerPlayer player1, player2;

	public ServerPlayer GetPlayerAt(int num) {
		return num == 0 ? player1 : player2;
	}
	public ServerPlayer GetCurPlayer() {
		return GetPlayerAt(turnController.CurPlayerTurnNum);
	}

	public List<Card> doorDeck, treasureDeck;

	public ServerTurnController turnController;
	public ServerWT warTable;

	public TextMeshProUGUI debugText;
	public TextMeshProUGUI doorDeckCountText;
	public TextMeshProUGUI treasureDeckCountText;

	private void Awake() {
		Instance = this;

		turnController = GetComponent<ServerTurnController>();

		player1 = new ServerPlayer();
		player2 = new ServerPlayer();
		warTable = new ServerWT();
	}

	private void Start() {
		doorDeck = CardManagerData.allDoorCards;
		treasureDeck = CardManagerData.allTreasureCards;

		doorDeck.Shaffle();
		treasureDeck.Shaffle();
	}

	public void StarGame() {
		GiveStartCards();
		turnController.StatFirstTurn();
	}

	public void GiveStartCards() {
		GiveHandCards(numberOfCards: 2, sp: player1, deck: doorDeck);
		GiveHandCards(numberOfCards: 2, sp: player1, deck: treasureDeck);
		GiveHandCards(numberOfCards: 2, sp: player2, deck: doorDeck);
		GiveHandCards(numberOfCards: 2, sp: player2, deck: treasureDeck);
	}
	private void GiveHandCards(int numberOfCards, ServerPlayer sp, List<Card> deck) {
		for (int i = 0; i < numberOfCards; i++)
			GiveCardToHand(sp, deck);
	}
	private void GiveCardToHand(ServerPlayer sp, List<Card> deck) {
		if (deck.Count == 0)
			return;

		Card card = deck[0];
		deck.RemoveAt(0);
		doorDeckCountText.text = doorDeck.Count + " Doors";
		treasureDeckCountText.text = treasureDeck.Count + " Treasures";
		card.closeId = sp.munchkin.hand.Count;
		sp.munchkin.hand.Add(card);

		Server.Instance.SendCardToHand(sp.info.number, card);
	}

	public void TryDropCard(int pNum, int cardId, string targetSlot) {
		Card card = GetPlayerAt(pNum).munchkin.hand.Find(c => c.id == cardId);

		if (card == null) // send NO
			return;
		
		if (targetSlot == "WT_MONSTER" && targetSlot == "WT_PLAYER") {
			if (card.cardType == Card.CardType.EXPLOSIVE) {
				if (((turnController.currentTurnStage == TurnStage.fight_player) && (turnController.CurPlayerTurnNum == pNum)) ||
					((turnController.currentTurnStage == TurnStage.fight_enemy) && (turnController.CurPlayerTurnNum != pNum))) {

					warTable.PlayCard(card, targetSlot == "WT_PLAYER");
				}
			}
			else if (card.cardType == Card.CardType.LVLUP) {
				GetCurPlayer().munchkin.LvlUp();
			}

		}
		else {
			if (card.cardType == Card.CardType.THING) {
				switch (targetSlot) {
					case "WEAPON1":
						if ((card as ThingCard).thingType == ThingCard.ThingType.WEAPON)
							GetPlayerAt(pNum).munchkin.weapon1 = card as ThingCard;
						break;

					case "WEAPON2":
						if ((card as ThingCard).thingType == ThingCard.ThingType.WEAPON)
							GetPlayerAt(pNum).munchkin.weapon2 = card as ThingCard;
						break;

					case "HEAD":
						if ((card as ThingCard).thingType == ThingCard.ThingType.HEAD)
							GetPlayerAt(pNum).munchkin.head = card as ThingCard;
						break;

					case "ARMOR":
						if ((card as ThingCard).thingType == ThingCard.ThingType.ARMOR)
							GetPlayerAt(pNum).munchkin.armor = card as ThingCard;
						break;

					case "SHOES":
						if ((card as ThingCard).thingType == ThingCard.ThingType.SHOES)
							GetPlayerAt(pNum).munchkin.shoes = card as ThingCard;
						break;
				}
			}

			if (card.cardType == Card.CardType.CLASS && targetSlot == "CLASS")
				GetPlayerAt(pNum).munchkin.munClass = card as ClassCard;
		}

	}
	private void TurnAllowed(int pNum, Card card, string targetSlot) {
		GetPlayerAt(pNum).munchkin.hand.Remove(card);
		GetPlayerAt(pNum).munchkin.SetCloseId();

		Server.Instance.SendDrop(pNum, card.id, card.closeId, targetSlot);
	}
	private void TurnDisallowed(int pNum) {

	}


	public void OpenDoor(out bool isMonster) {
		isMonster = false;

		if (doorDeck.Count == 0)
			return;

		Card card = doorDeck[0];
		doorDeck.RemoveAt(0);
		doorDeckCountText.text = doorDeck.Count + " Doors";
	
		isMonster = card.cardType == Card.CardType.MONSTER;

		if (isMonster)
			warTable.StartFight(card as MonsterCard);
		else
			warTable.OpenCard(card);

		Server.Instance.SendOpenDoor(turnController.CurPlayerTurnNum, card.id, isMonster);
	}

	public void OnPlayerWinFight() {
		GiveHandCards(warTable.GetNumberOfTreasure(), GetCurPlayer(), treasureDeck);

		GetCurPlayer().munchkin.LvlUp(1);
		warTable.ClearTable();
	}
}