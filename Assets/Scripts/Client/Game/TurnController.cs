using System.Collections;
using TMPro;
using UnityEngine;

public enum TurnStage {
	preparation,
	waiting,
	after_door,
	fight_player,
	fight_enemy,
	select_cards,
	completion
}

public class TurnController : MonoBehaviour {
	
	public TextMeshPro turnTimeText;
	public TextMeshPro turnStageText;
	public MyButton endTurnButton;

	public bool playerTurn;
	public TurnStage currentTurnStage;

	private void StartGame() {
		currentTurnStage = TurnStage.preparation;

		endTurnButton.textMesh.text = "Open Door";
		StartCoroutine(TurnFunc(20));
	}

	private IEnumerator TurnFunc(int time) {
		//int[] turnTime = { 20, 3, 15, 15, 15, 20, 10 };
		int timeToEndTurn = time;//turnTime[(int)currentTurnStage];
		turnTimeText.text = timeToEndTurn.ToString();

		while (timeToEndTurn >= 0) {
			turnTimeText.text = timeToEndTurn.ToString();
			yield return new WaitForSeconds(1);
			timeToEndTurn--;
		}
	}

	public void ButtonOnPress() {
		//SendChangeTurn();
	}
	public void ChangeTurn(TurnStage newStage, bool isPlayerTurn, int time) {
		StopAllCoroutines();

		playerTurn = isPlayerTurn;

		currentTurnStage = newStage;
		turnStageText.text = currentTurnStage.ToString();

		bool isButtonEnable = (isPlayerTurn ^ (newStage == TurnStage.fight_enemy)) && (newStage != TurnStage.waiting);
		endTurnButton.SetActive(isButtonEnable);

		StartCoroutine(TurnFunc(time));
	}
}