using TMPro;
using UnityEngine;

[RequireComponent(typeof(PlayerInfo))]
public class Munchkin : MonoBehaviour {

	public PlayerInfo info;

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


	void Awake() {
		info = GetComponent<PlayerInfo>();
	}

	public void SetDmgAndLvl(int dmg, int lvl) {
		dmgText.text = "at: " + dmg;
		lvlText.text = "lvl: " + lvl;
	}

	public void SetInfo(string name, int number, int cnnId) {
		info.playerNameText.text = name;

		info.playerName = name;
		info.number = number;
		info.connectionId = cnnId;
	}
}
