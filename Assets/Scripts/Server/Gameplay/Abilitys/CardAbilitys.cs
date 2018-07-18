using UnityEngine;

public class CardAbilitys: MonoBehaviour {

	public static CardAbilitys Instance { get; set; }

	//public CardAbilitys() {
	//	if (Instance == null)
	//		Instance = this;
	//}

	private void Awake() {
		Instance = this;
	}

	// id: 21
	public void _TrapPoisoning() {
		Debug.Log("INVOKING");
		ServerGM.Instance.GetCurPlayer().munchkin.LvlUp(-1);
		// send that lvl change
	}

	// id: 26
	public void _TrapRadioactivityPuddles() {
		Debug.Log("INVOKING");

		if (ServerGM.Instance.GetCurPlayer().munchkin.shoes == null)
			return;
		
		Server.Instance.SendRemoveCard(ServerGM.Instance.GetCurPlayer().info.number, "SHOES");
		ServerGM.Instance.GetCurPlayer().munchkin.shoes = null;
	}

	// id: 28
	public void _TrapConcussion() {
		Debug.Log("INVOKING");

		if (ServerGM.Instance.GetCurPlayer().munchkin.munClass != null) {
			Server.Instance.SendRemoveCard(ServerGM.Instance.GetCurPlayer().info.number, "CLASS");
			ServerGM.Instance.GetCurPlayer().munchkin.munClass = null;
		}
		else {
			ServerGM.Instance.GetCurPlayer().munchkin.LvlUp(-1);
			// send that lvl change
		}
	}

}