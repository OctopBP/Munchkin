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
		GiveHandCards(numberOfCards: 1, sp: player1, deck: doorDeck);
		GiveHandCards(numberOfCards: 4, sp: player1, deck: treasureDeck);
		GiveHandCards(numberOfCards: 1, sp: player2, deck: doorDeck);
		GiveHandCards(numberOfCards: 4, sp: player2, deck: treasureDeck);
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
		card.closeId = sp.munchkin.hand.Count;
		sp.munchkin.hand.Add(card);

		Server.Instance.SendCardToHand(sp.info.number, card);
	}

	/*
	public void OnDrop(int pNum, int cardId, string targetSlot) {
		Card card = GetPlayerAt(pNum).munchkin.hand.Find(c => c.id == cardId);

		if (card == null)
			return;

		switch (targetSlot) {
			case "WEAPON1": GetPlayerAt(pNum).munchkin.weapon1 = card as ThingCard; break;
			case "WEAPON2": GetPlayerAt(pNum).munchkin.weapon2 = card as ThingCard; break;
			case "HEAD": 	GetPlayerAt(pNum).munchkin.head = card as ThingCard;	break;
			case "ARMOR":	GetPlayerAt(pNum).munchkin.armor = card as ThingCard;	break;
			case "SHOES":	GetPlayerAt(pNum).munchkin.shoes = card as ThingCard;	break;
		}

		GetPlayerAt(pNum).munchkin.hand.Remove(card);
		GetPlayerAt(pNum).munchkin.SetCloseId();

		string msg = SendNames.dropcard + "|" + pNum + "|" + cardId + "|" + card.closeId + "|" + targetSlot;
		Send(msg, reliableChannel);
	}
	*/

	public void OpenDoor(out bool isMonster) {
		isMonster = false;

		if (doorDeck.Count == 0)
			return;

		Card card = doorDeck[0];
		doorDeck.RemoveAt(0);
	
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