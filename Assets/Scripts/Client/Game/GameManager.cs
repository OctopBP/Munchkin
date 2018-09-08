using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System;

public class GameManager: MonoBehaviour {

	public static GameManager Instance { get; set; }

	public Munchkin player;
	public Munchkin enemy;

	public Munchkin GetMunchkin(int num) {
		return num == player.info.number ? player : enemy;
	}

	public WarTable warTable;

	public List<CardInfo> freezCards = new List<CardInfo>();

    public GameObject cardPrefab;
	public TurnController turnController;

	public GameObject cnnCanvas;
	public GameObject cnnCnnGroup;
	public GameObject cnnWaitGroup;

	public GameObject blackPlane;
	public GameObject okButton;

	public GameObject enemyDropCardsText;

	private void Awake() {
		Instance = this;

		turnController = GetComponent<TurnController>();
    }

	private Vector3 deckPos = new Vector3(6.6f, 0.8f, 0);
	public CardInfo CreateCard(HidenCard.DeckType deckType) {
		HidenCard hidenCard = new HidenCard();;
		hidenCard.deckType = deckType;

		Quaternion deckQuat = Quaternion.identity;
		deckQuat.eulerAngles = new Vector3(0, 90, 180);

		GameObject cardGO = Instantiate(cardPrefab, deckPos, deckQuat);
		cardGO.name = hidenCard.deckType.ToString().ToLower() + " card";

		CardInfo cardInfo = cardGO.GetComponent<CardInfo>();
		cardInfo.BuildCard(hidenCard);

		return cardInfo;
	}
	public CardInfo CreateCard(int cardId) {
		Card card = CardManagerData.allCards.Find(c => c.id == cardId);

		Quaternion deckQuat = Quaternion.identity;
		deckQuat.eulerAngles = new Vector3(0, 90, 180);

		GameObject cardGO = Instantiate(cardPrefab, deckPos, deckQuat);
		cardGO.name = card.cardType.ToString().ToLower() + "Card (" + card.name + ")";

		CardInfo cardInfo = cardGO.GetComponent<CardInfo>();
		cardInfo.BuildCard(card);

		return cardInfo;
	}

	// Button functions
	public void EndTurn() {
		Client.Instance.EndTurn();
	}
	public void SelectCards() {
		List<string> cardIdToDrop = new List<string>();

		foreach (KeyValuePair<string, CardInfo> card in cardToDropList) {
			if (card.Value.cardMovment.selectedToDrop)
				cardIdToDrop.Add(card.Key);
		}

		if (cardIdToDrop.Count != numberOfCardsToDrop)
			return;

		foreach (KeyValuePair<string, CardInfo> card in cardToDropList) {
			card.Value.cardMovment.x_mark.enabled = false;
			card.Value.cardMovment.selectedToDrop = false;
			card.Value.cardMovment.ResetPosition();
		}

		Client.Instance.SendCardToDrop(cardIdToDrop);

		blackPlane.SetActive(false);
		okButton.SetActive(false);
	}

	public void Drop(int pNum, string parentSlotId, string targetSlotId, int cardId) {
		CardInfo cardInfo = GetMunchkin(pNum).hand.GetSlot(parentSlotId).GetCard(); // NOT TARGET
		Card card = CardManagerData.allCards.Find(c => c.id == cardId);

		if (!cardInfo.cardIsOpen)
			cardInfo.OpenCard(card);

		if (targetSlotId.StartsWith("WT", StringComparison.CurrentCulture)) {
			if (card.cardType == Card.CardType.LVLUP) {
				GetMunchkin(pNum).hand.RemoveSlot(parentSlotId);
				freezCards.Remove(cardInfo);

				if (pNum == player.info.number) {
					Destroy(cardInfo.gameObject);
				}
				else {
					cardInfo.OpenCard(card);
					cardInfo.cardMovment.animator.PlayCard();
				}

				return;
			}
		}

		if (targetSlotId == "WTM")
			warTable.AddCard(cardInfo, false);
		else if (targetSlotId == "WTP")
			warTable.AddCard(cardInfo, true);
		else
			GetMunchkin(pNum).GetSlot(targetSlotId).AddCard(cardInfo);

		GetMunchkin(pNum).hand.RemoveSlot(parentSlotId);

		cardInfo.cardMovment.AllowDrop();
		freezCards.Remove(cardInfo);
	}
	public void DropDisallowed(string slotId, string reason) {
		freezCards.Find(c => c.selfCard.slotId == slotId).cardMovment.UndoDrop();
	}
	public void RemoveCard(int pNum, string slotId) {
		if (slotId.StartsWith("HA", StringComparison.CurrentCulture))
			GetMunchkin(pNum).hand.RemoveCard(slotId);
		else
			GetMunchkin(pNum).GetSlot(slotId).RemoveCard();
	}
	public void OpenDoor(int cardId, bool isMonster) {
		CardInfo cardInfo = CreateCard(cardId);
		cardInfo.cardMovment.State = CardMovment.CardState.OPEN;

		if (isMonster)
			warTable.StartFight(cardInfo);
		else
			warTable.AddCard(cardInfo, true);
	}

	private readonly Dictionary<string, CardInfo> cardToDropList = new Dictionary<string, CardInfo>();
	private int numberOfCardsToDrop = 0;
	public void SelectionCards(List<string> slots, int numberOfCards) {
		numberOfCardsToDrop = numberOfCards;

		blackPlane.SetActive(true);
		okButton.SetActive(true);

		int i = 0;
		foreach (string slotId in slots) {
			float x = (1.4f * i) - ((float)(slots.Count - 1) / 2) * 1.4f;
			float z = 0.45f; // numberOfCards < 6 ? 0.45f : 1.8f * (i % 2) - 0.7f;
			i++;

			CardInfo cardInfo = null;

			if (slotId.StartsWith("HA", StringComparison.Ordinal))
				cardInfo = player.hand.GetSlot(slotId).GetCard();
			else
				cardInfo = player.GetSlot(slotId).GetCard();

			cardInfo.cardMovment.HoverToSelection(x, z);
			cardToDropList.Add(slotId, cardInfo);
		}
	}
}