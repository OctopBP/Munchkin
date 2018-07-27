using TMPro;
using UnityEngine;

public class PlayerInfo {
	public string name;
	public int number;
	public int connectionId;
}

public class Munchkin : MonoBehaviour {

	public PlayerInfo info = new PlayerInfo();

	[Space(16)]
	public Hand hand;
	public ThingSlot weapon1;
	public ThingSlot weapon2;
	public ThingSlot head;
	public ThingSlot armor;
	public ThingSlot shoes;
	public ClassSlot munClass;

	[Space(16)]
	public TextMeshPro lvlText;
	public TextMeshPro dmgText;

	public int lvl;
	public int damage;

	void Awake() {
		lvl = 1;
		damage = 1;
	}

	public void LvlUp(int lvls) {
		lvl += lvls;
		//lvlText.text = "lvl: " + lvl;
	}

	public void SetDmgAndLvl(int dmg, int lvl) {
		dmgText.text = "at: " + dmg;
		lvlText.text = "lvl: " + lvl;
	}
}
