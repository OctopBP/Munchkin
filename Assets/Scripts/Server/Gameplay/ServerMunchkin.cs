using System.Collections.Generic;
using UnityEngine;

public class ServerMunchkin : MonoBehaviour {

	public List<Card> hand = new List<Card>();
	public ThingCard weapon1;
	public ThingCard weapon2;
	public ThingCard head;
	public ThingCard armor;
	public ThingCard shoes;
	public ClassCard munClass;

	public int lvl = 1;
	public int Damage {
		get {
			int dmg = lvl;

			dmg += weapon1 != null ? weapon1.bonus : 0;
			dmg += weapon2 != null ? weapon2.bonus : 0;
			dmg += head != null ? head.bonus : 0;
			dmg += armor != null ? armor.bonus : 0;
			dmg += shoes != null ? shoes.bonus : 0;

			return dmg;
		}
	}
	public void LvlUp(int lvls = 1) {
		lvl += lvls;
	}

	public void SetCloseId() {
		for (int i = 0; i < hand.Count; i++)
			hand[i].closeId = i;
	}

	public void setCardToSlot(string slotName, Card card) {
		switch (slotName) {
			case "WEAPON1": weapon1 	= card as ThingCard;	break;
			case "WEAPON2": weapon2 	= card as ThingCard;	break;
			case "HEAD":	head 		= card as ThingCard;	break;
			case "ARMOR":	armor		= card as ThingCard;	break;
			case "SHOES":	shoes		= card as ThingCard;	break;
			case "CLASS":	munClass	= card as ClassCard;	break;
		}
	}
}
