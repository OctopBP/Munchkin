using UnityEngine;

public enum SlotParent {
	SELF,
	ENEMY,
	WARTABLE
}
public enum DropSlotType {
	WEAPON1,
	WEAPON2,
	HEAD,
	ARMOR,
	SHOES,
	HAND,
	CLASS,

	WT_MONSTER,
	WT_PLAYER
}

public class DropSlot: MonoBehaviour {

	public SlotParent slotParent;
	public DropSlotType dropSlotType;

	//public void OnDrop(CardInfo card) {
		/*
		if (slotParent == SlotParent.ENEMY)
			return;

		// TODO: Remove
		if (slotParent == SlotParent.WARTABLE) {
			if (card.selfCard.cardType == Card.CardType.EXPLOSIVE) {
				if (((ClientGM.Instance.turnController.currentTurnStage == TurnStage.fight_player) && ClientGM.Instance.turnController.playerTurn) ||
			    	((ClientGM.Instance.turnController.currentTurnStage == TurnStage.fight_enemy) && !ClientGM.Instance.turnController.playerTurn)) {

					ClientGM.Instance.OnDrop(card.selfCard, Enum.GetName(typeof(DropSlotType), dropSlotType));
				}
			}

			if (card.selfCard.cardType == Card.CardType.LVLUP) {
				ClientGM.Instance.OnDrop(card.selfCard, Enum.GetName(typeof(DropSlotType), dropSlotType));
			}

			return;	
		}

		if (slotParent == SlotParent.SELF) {
			if (card.selfCard.cardType == Card.CardType.THING) {
				if (((card.selfCard as ThingCard).thingType == ThingCard.ThingType.WEAPON && dropSlotType == DropSlotType.WEAPON1)
					|| ((card.selfCard as ThingCard).thingType == ThingCard.ThingType.WEAPON && dropSlotType == DropSlotType.WEAPON2)
					|| ((card.selfCard as ThingCard).thingType == ThingCard.ThingType.HEAD && dropSlotType == DropSlotType.HEAD)
					|| ((card.selfCard as ThingCard).thingType == ThingCard.ThingType.ARMOR && dropSlotType == DropSlotType.ARMOR)
					|| ((card.selfCard as ThingCard).thingType == ThingCard.ThingType.SHOES && dropSlotType == DropSlotType.SHOES)) {

					ClientGM.Instance.OnDrop(card.selfCard, Enum.GetName(typeof(DropSlotType), dropSlotType));

					return;
				}
			}

			if (card.selfCard.cardType == Card.CardType.CLASS) {
				if (dropSlotType == DropSlotType.CLASS) {
					ClientGM.Instance.OnDrop(card.selfCard, Enum.GetName(typeof(DropSlotType), dropSlotType));
				}
			}
		}
		*/
	//}

	/*
	public void OnDrop(CardInfo card) {
		
		switch (dropSlotType) {
			case DropSlotType.HAND: break;

			case DropSlotType.WT_PLAYER:
			case DropSlotType.WT_MONSTER:
				if (card.typeIs(Card.CardType.LVLUP) || card.typeIs(Card.CardType.MONSTER) || card.typeIs(Card.CardType.EXPLOSIVE)) {
					WarTable warTable = GetComponent<WarTableSide>().warTable;
					warTable.PlayCard(card, dropSlotType == DropSlotType.WT_PLAYER);

					break;
				}
				card.cardMovment.ResetPosition();
				return;
				
			case DropSlotType.WEAPON:
			case DropSlotType.HEAD:
			case DropSlotType.ARMOR:
			case DropSlotType.SHOES:
				if (card.selfCard.cardType == Card.CardType.THING) {
					ThingSlot thingSlot = GetComponent<ThingSlot>();
					if ((card.selfCard as ThingCard).thingType == thingSlot.thingType) {
						thingSlot.AddCard(card);
						card.transform.parent = thingSlot.transform;
						break;
					}
				}
				card.cardMovment.ResetPosition();
				return;

			case DropSlotType.CLASS:
				if (card.selfCard.cardType != Card.CardType.CLASS) {
					card.cardMovment.ResetPosition();
					return;
				}

				ClassSlot classSlot = GetComponent<ClassSlot>();
				classSlot.AddCard(card);
				card.transform.parent = classSlot.transform;

				break;
		}

		DropSlot parentSlot = card.cardMovment.defaultParent.GetComponent<DropSlot>();
		parentSlot.RemoveCard(card);
	}

	public void RemoveCard(CardInfo card) {
		switch (dropSlotType) {
			case DropSlotType.HAND:
				GetComponent<Hand>().RemoveCard(card);
				break;

			//case DropSlotType.WARTABLE: break;

			case DropSlotType.WEAPON:
			case DropSlotType.HEAD:
			case DropSlotType.ARMOR:
			case DropSlotType.SHOES:
			case DropSlotType.CLASS: break;
		}
	}

*/
}