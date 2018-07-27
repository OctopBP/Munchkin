using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum TurnStage {
	preparation,
	waiting,
	after_door,
	fight_player,
	fight_enemy,
	completion
}

public class TurnController : MonoBehaviour {

	public TextMeshProUGUI turnTimeText;
	public TextMeshProUGUI turnStageText;
	public Button endTurnButton;
	private TextMeshProUGUI buttonText;

	public bool playerTurn;
	public TurnStage currentTurnStage;

	private void StartGame() {
		currentTurnStage = TurnStage.preparation;

		buttonText = endTurnButton.GetComponentInChildren<TextMeshProUGUI>();

		buttonText.text = "Open Door";
		StartCoroutine(TurnFunc());
	}

	private IEnumerator TurnFunc() {
		int[] turnTime = { 20, 2, 15, 15, 15, 10 };
		int timeToEndTurn = turnTime[(int)currentTurnStage];
		turnTimeText.text = timeToEndTurn.ToString();
		//turnStageText.text = currentTurnStage.ToString();

		while (timeToEndTurn >= 0) {
			turnTimeText.text = timeToEndTurn.ToString();
			yield return new WaitForSeconds(1);
			timeToEndTurn--;
		}
	}

	public void ButtonOnPress() {
		//SendChangeTurn();
	}
	public void ChangeTurn(TurnStage newStage, bool isPlayerTurn) {
		StopAllCoroutines();

		playerTurn = isPlayerTurn;

		currentTurnStage = newStage;
		turnStageText.text = currentTurnStage.ToString();

		Color disableColor = Color.grey; // new Color(110, 180, 90);
		Color enableColor = Color.green; // new Color(240, 65, 70);

		bool isButtonEnable = (isPlayerTurn ^ (newStage == TurnStage.fight_enemy)) && (newStage != TurnStage.waiting);

		endTurnButton.image.color = isButtonEnable ? enableColor : disableColor;
		endTurnButton.enabled = isButtonEnable;

		StartCoroutine(TurnFunc());
	}
}