using UnityEngine;

public class CardMovment : MonoBehaviour {

	public MeshRenderer border;

	[HideInInspector] public Transform defaultParent;

	[HideInInspector] public CardInfo cardInfo;
	//[HideInInspector] public CardAnimator animator;

    Vector3 basePosition;
	Vector3 baseAngles;

	public bool cardSelected;
	public bool cardActive;
	bool isDraggable;

    void Awake() {
		cardInfo = GetComponent<CardInfo>();
		//animator = GetComponent<CardAnimator>();

		border.enabled = false;
		cardActive = true;
    }



	void OnMouseOver() {
		defaultParent = transform.parent;

		bool canHover = defaultParent.GetComponent<DropSlot>().slotParent != SlotParent.ENEMY || defaultParent.GetComponent<DropSlot>().dropSlotType != DropSlotType.HAND;
		isDraggable = defaultParent.GetComponent<DropSlot>().slotParent == SlotParent.SELF;

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

	void OnMouseDown() {
		// TODO: Hover down
	}

	void OnMouseDrag() {
		if (!isDraggable || !cardActive)
			return;

		Vector3 distanceToScreen = Camera.main.WorldToScreenPoint(transform.position);
		Vector3 posMove = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, distanceToScreen.z));

		transform.position = new Vector3(posMove.x, transform.position.y, posMove.z);

		// Если parent рука и находимся в пределах руки при перетаскивании меняем карты местами
	}

	void OnMouseUp() {
		if (!isDraggable || !cardActive)
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

				if (targetSlot.slotParent == SlotParent.SELF || targetSlot.slotParent == SlotParent.WARTABLE) {
					
					targetSlot.OnDrop(cardInfo);
					//WriteNewPosition();
					//cardActive = false;

					return;
				}
				if (targetSlot.slotParent == SlotParent.WARTABLE) {
					// 
				}
			}
		}
		ResetPosition();
    }

	void OnMouseExit() {
		ResetPosition();
		cardSelected = false;
		transform.SetParent(defaultParent);
	}



	void HoverCard() {
		float zLimit = 1;
		float xLimit = 4;
		float distanceToCamera = 3;

		float k = distanceToCamera / (Camera.main.transform.position.y - basePosition.y);

		Vector3 hoverPosition;
		//hoverPosition.x = k * basePosition.x;
		hoverPosition.x = Mathf.Min(Mathf.Max(k * basePosition.x, -xLimit), xLimit);
		hoverPosition.y = Camera.main.transform.position.y - distanceToCamera;
		hoverPosition.z = Mathf.Min(Mathf.Max(k * transform.position.z, -zLimit), zLimit);

		transform.position = hoverPosition;
		transform.eulerAngles = new Vector3(0, 0, 0);
	}



	public void ResetPosition() {
        transform.position = basePosition;
        transform.eulerAngles = baseAngles;
		//Debug.Log("ResetPos()");
    }

	public void WriteNewPosition(Vector3 position, Vector3 angles) {
        basePosition = position;
        baseAngles = angles;
		//Debug.Log("WriteNewPos(" + position + ", " + angles + ")");
    }

	public void WriteNewPosition() {
        basePosition = transform.position;
        baseAngles = transform.eulerAngles;
		//Debug.Log("WriteNewPos()");
    }

	public void Animate(Vector3 targetPos, Vector3 targetAngles) {
		WriteNewPosition(targetPos, targetAngles);
		transform.position = targetPos;
		transform.eulerAngles = targetAngles;
		//animator.MoveTo(transform.position, targetPos, transform.eulerAngles, targetAngles);
		//WriteNewPosition(targetPos, targetAngles);
	}
}
