using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ClientTurnController : MonoBehaviour {

	public TextMeshProUGUI turnTimeText;
	public TextMeshProUGUI turnStageText;
	public Button endTurnButton;
	private TextMeshProUGUI buttonText;

	public bool playerTurn;
	public TurnStage currentTurnStage;

	void StartGame() {
		currentTurnStage = TurnStage.preparation;

		buttonText = endTurnButton.GetComponentInChildren<TextMeshProUGUI>();

		buttonText.text = "Open Door";
		StartCoroutine(TurnFunc());
	}

	IEnumerator TurnFunc() {
		int[] turnTime = { 20, 2, 20, 20, 5, 10, 5 };
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

		endTurnButton.enabled = isPlayerTurn ^ (newStage == TurnStage.fight_enemy);
		endTurnButton.image.color = (isPlayerTurn ^ (newStage == TurnStage.fight_enemy)) ? new Color(110, 180, 90) : new Color(240, 65, 70);

		StartCoroutine(TurnFunc());
	}
}