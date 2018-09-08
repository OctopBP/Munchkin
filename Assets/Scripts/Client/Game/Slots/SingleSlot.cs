using System;
using UnityEngine;

public class SingleSlot : DropSlot {
	private CardInfo card;

	public CardInfo GetCard() {
		return card;
	}

	public void AddCard(CardInfo newCard) {
		RemoveCard();

		card = newCard;
		newCard.transform.parent = transform;

		card.selfCard.slotId = Enum.GetName(typeof(DropSlotType), dropSlotType);

		PlaceCard();
	}
	public void RemoveCard() {
		if (card != null) {
			card.cardMovment.animator.CardToPile();
			card = null;
		}
	}

	private void PlaceCard() {
		Vector3 newPos = transform.position;
		Vector3 newAngl = new Vector3(0f, 0, 0f);

		card.cardMovment.MoveTo(newPos, newAngl, 1f);
	}
}
