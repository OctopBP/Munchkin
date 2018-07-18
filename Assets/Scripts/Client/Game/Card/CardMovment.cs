using System;
using System.Collections;
using UnityEngine;

public class CardMovment : MonoBehaviour {

	public MeshRenderer border;

	[HideInInspector] public Transform defaultParent;

	[HideInInspector] public CardInfo cardInfo;
	[HideInInspector] public CardAnimator animator;

	private Vector3 basePosition;
	private Vector3 baseAngles;

	//private Vector3 targetPosition;
	//private Vector3 targetAngles;

	public bool cardSelected;
	public bool cardActive;

	private bool cardFreezed;
	private bool isYourCard;

	private void Awake() {
		cardInfo = GetComponent<CardInfo>();
		animator = GetComponent<CardAnimator>();

		border.enabled = false;
		cardActive = true;
		cardFreezed = false;
    }

	private void OnMouseOver() {
		if (cardFreezed)
			return;
		
		defaultParent = transform.parent;

		bool canHover = defaultParent.GetComponent<DropSlot>().slotParent != SlotParent.ENEMY || defaultParent.GetComponent<DropSlot>().dropSlotType != DropSlotType.HAND;
		isYourCard = defaultParent.GetComponent<DropSlot>().slotParent == SlotParent.SELF;

		if (!canHover) {
			WriteNewPosition();
			return;
		}

		if (!cardSelected) {
            WriteNewPosition();
			HoverCard();

            cardSelected = true;
        }
    }
	private void OnMouseDown() {
		// TODO: Hover down
	}
	private void OnMouseDrag() {
		if (!isYourCard || !cardActive || cardFreezed)
			return;

		Vector3 distanceToScreen = Camera.main.WorldToScreenPoint(transform.position);
		Vector3 posMove = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, distanceToScreen.z));

		transform.position = new Vector3(posMove.x, transform.position.y, posMove.z);

		// Если parent рука и находимся в пределах руки при перетаскивании меняем карты местами
	}
	private void OnMouseUp() {
		if (!isYourCard || !cardActive || cardFreezed)
			return;
		
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hits = Physics.RaycastAll(ray);

		foreach (RaycastHit hit in hits) {
		
			DropSlot targetSlot = hit.collider.gameObject.transform.GetComponent<DropSlot>();
			if (targetSlot) {
				if (targetSlot.transform == defaultParent) {
					ResetPosition();
					return;
				}

				if (targetSlot.slotParent != SlotParent.ENEMY) {
					string targetSlotName = Enum.GetName(typeof(DropSlotType), targetSlot.dropSlotType);

					Client.Instance.OnDrop(cardInfo.selfCard, targetSlotName);
					ClientGM.Instance.freezCards.Add(cardInfo);
					cardFreezed = true;
					return;
				}
			}
		}
		ResetPosition();
    }
	private void OnMouseExit() {
		animator.StopAllCoroutines();
		//StopAllCoroutines();

		if (cardFreezed)
			return;
		
		ResetPosition();
		cardSelected = false;
		transform.SetParent(defaultParent);
	}

	private void HoverCard() {
		float zLimit = 1;
		float xLimit = 4;
		float distanceToCamera = 3;

		float k = distanceToCamera / (Camera.main.transform.position.y - basePosition.y);

		Vector3 hoverPosition;
		hoverPosition.x = Mathf.Min(Mathf.Max(k * basePosition.x, -xLimit), xLimit);
		hoverPosition.y = Camera.main.transform.position.y - distanceToCamera;
		hoverPosition.z = Mathf.Min(Mathf.Max(k * transform.position.z, -zLimit), zLimit);

		animator.Animate(hoverPosition, Vector3.zero, 0.1f);
	}

	public void UndoDrop() {
		cardFreezed = false;
		ResetPosition();
	}
	public void AllowDrop() {
		cardFreezed = false;
		cardActive = false;
		//WriteNewPosition(targetSlot.transform.position, targetSlot.transform.eulerAngles);
		//defaultParent = targetSlot.transform;
		ResetPosition();
	}

	public void ResetPosition() {
        transform.position = basePosition;
        transform.eulerAngles = baseAngles;
    }
	public void WriteNewPosition(Vector3 position, Vector3 angles) {
        basePosition = position;
        baseAngles = angles;
    }
	public void WriteNewPosition() {
        basePosition = transform.position;
        baseAngles = transform.eulerAngles;
    }
}
