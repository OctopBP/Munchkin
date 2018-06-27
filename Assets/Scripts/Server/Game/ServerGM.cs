﻿using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ServerPlayer {
	public PlayerInfo info = new PlayerInfo();
	public ServerMunchkin munchkin = new ServerMunchkin();
}

public class ServerGM: MonoBehaviour {

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

		if (card == null) {
			TurnDisallowed(pNum, cardId, "no card in hand");
			return;
		}

		Dictionary<string, ThingCard.ThingType> thingMap = new Dictionary<string, ThingCard.ThingType> {
			{ "WEAPON1", ThingCard.ThingType.WEAPON },
			{ "WEAPON2", ThingCard.ThingType.WEAPON },
			{ "HEAD", ThingCard.ThingType.HEAD },
			{ "ARMOR", ThingCard.ThingType.ARMOR },
			{ "SHOES", ThingCard.ThingType.SHOES }
		};

		switch (card.cardType) {
			case Card.CardType.EXPLOSIVE:
				if (targetSlot == "WT_MONSTER" || targetSlot == "WT_PLAYER") {
					if (((turnController.currentTurnStage == TurnStage.fight_player) && (turnController.CurPlayerTurnNum == pNum)) ||
						((turnController.currentTurnStage == TurnStage.fight_enemy) && (turnController.CurPlayerTurnNum != pNum))) {

						warTable.PlayCard(card, targetSlot == "WT_PLAYER");
						TurnAllowed(pNum, card, targetSlot);
						return;
					}
				}
				break;

			case Card.CardType.LVLUP:
				if (targetSlot == "WT_MONSTER" || targetSlot == "WT_PLAYER") {
					GetCurPlayer().munchkin.LvlUp();
					TurnAllowed(pNum, card, targetSlot);
					return;
				}
				break;

			case Card.CardType.THING:
				if (thingMap[targetSlot] == (card as ThingCard).thingType) {
					GetPlayerAt(pNum).munchkin.setCardToSlot(targetSlot, card);
					TurnAllowed(pNum, card, targetSlot);
					return;
				}
				break;

			case Card.CardType.CLASS:
				if (targetSlot == "CLASS") {
					GetPlayerAt(pNum).munchkin.setCardToSlot(targetSlot, card);
					TurnAllowed(pNum, card, targetSlot);
					return;
				}
				break;

			case Card.CardType.MONSTER:
				break;
		}
		TurnDisallowed(pNum, cardId, "reason");
	}
	private void TurnAllowed(int pNum, Card card, string targetSlot) {
		GetPlayerAt(pNum).munchkin.hand.Remove(card);
		GetPlayerAt(pNum).munchkin.SetCloseId();

		Server.Instance.SendTurnAllowed(pNum, card.id, card.closeId, targetSlot);
	}
	private void TurnDisallowed(int pNum, int cardId, string reason) {
		Server.Instance.SendTurnDisllowed(pNum, cardId, reason);
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