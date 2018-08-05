using System;
using TMPro;
using UnityEngine;

public class CardMovment : MonoBehaviour {

	public MeshRenderer border;
	public TextMeshPro stateText;

	[HideInInspector] public Transform defaultParent;

	[HideInInspector] public CardInfo cardInfo;
	[HideInInspector] public CardAnimator animator;

	private Vector3 basePosition;
	private Vector3 baseAngles;

	private bool mouseOver = false;
	private float overTime;
	private float overTimeLimit = 0.1f;

	public bool moving;
	public  bool selected;
	public CardState State = CardState.CLOSED;
	public enum CardState {
		HOVERED,
		HOVERED_A,
		CLOSED,
		ACTIVE,
		OPEN,
		FREEZED
	}

	private void Awake() {
		cardInfo = GetComponent<CardInfo>();
		animator = GetComponent<CardAnimator>();

		border.enabled = false;
    }
	private void Update() {
		stateText.text = Enum.GetName(typeof(CardState), State);
	}

	private void OnMouseOver() {
		if (selected || moving || (State != CardState.ACTIVE && State != CardState.OPEN))
			return;

		// Delay before Hover()
		if (!mouseOver)
			overTime = overTimeLimit;
		
		mouseOver = true;

		if (overTime > 0) {
			overTime -= Time.deltaTime;
			return;
		}

		// Hover()
		if (selected || moving)
			return;
		
		if (State == CardState.ACTIVE)
			State = CardState.HOVERED_A;
		else
			State = CardState.HOVERED;

		WriteNewPosition();
		animator.HoverCard(basePosition);
		selected = true;
    }
	private void OnMouseDown() {
		// TODO: Hover down
	}
	private void OnMouseDrag() {
		if (State != CardState.HOVERED_A || moving)
			return;

		Vector3 distanceToScreen = Camera.main.WorldToScreenPoint(transform.position);
		Vector3 posMove = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, distanceToScreen.z));

		transform.position = new Vector3(posMove.x, transform.position.y, posMove.z);

		// Если parent рука и находимся в пределах руки при перетаскивании меняем карты местами
	}
	private void OnMouseUp() {
		if (State != CardState.HOVERED_A || moving)
			return;
		
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hits = Physics.RaycastAll(ray);

		foreach (RaycastHit hit in hits) {
			DropSlot targetSlot = hit.collider.gameObject.transform.GetComponent<DropSlot>();
			if (targetSlot) {
				if (targetSlot.transform == defaultParent || targetSlot.slotParent == SlotParent.ENEMY) {
					if (State == CardState.HOVERED_A)
						State = CardState.ACTIVE;
					else if (State == CardState.HOVERED)
						State = CardState.OPEN;
					
					ResetPosition();
					return;
				}

				string targetSlotName = Enum.GetName(typeof(DropSlotType), targetSlot.dropSlotType);

				Client.Instance.OnDrop(cardInfo.selfCard, targetSlotName);
				GameManager.Instance.freezCards.Add(cardInfo);
				State = CardState.FREEZED;
				return;
			} 
		}
		ResetPosition();
    }
	private void OnMouseExit() {
		mouseOver = false;

		if (State == CardState.FREEZED || State == CardState.CLOSED || moving)
			return;

		animator.StopAllCoroutines();
		ResetPosition();
		transform.SetParent(defaultParent);

		if (State == CardState.HOVERED_A)
			State = CardState.ACTIVE;
		else if (State == CardState.HOVERED)
			State = CardState.OPEN;

		selected = false;
	}

	public void MoveTo(Vector3 targetPos, Vector3 targetAngles, float time) {
		WriteNewPosition(targetPos, targetAngles);

		if (State == CardState.HOVERED)
			return;

		animator.MoveTo(targetPos, targetAngles, time);
	}

	public void UndoDrop() {
		State = CardState.ACTIVE;
		ResetPosition();
	}
	public void AllowDrop() {
		State = CardState.OPEN;
		ResetPosition();
	}

	public void ResetPosition() {
		//Debug.Log("ResetPosition()");
        transform.position = basePosition;
        transform.eulerAngles = baseAngles;
    }
	public void WriteNewPosition(Vector3 position, Vector3 angles) {
		//Debug.Log("WriteNewPosition(" + position + ", " + angles + ")");
        basePosition = position;
        baseAngles = angles;
    }
	public void WriteNewPosition() {
		//Debug.Log("WriteNewPosition()");
        basePosition = transform.position;
        baseAngles = transform.eulerAngles;
    }
}
