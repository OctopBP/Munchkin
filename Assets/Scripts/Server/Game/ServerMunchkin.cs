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
}
