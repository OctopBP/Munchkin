using UnityEngine;

public enum SlotParent {
	SELF,
	ENEMY,
	WARTABLE
}
public enum DropSlotType {
	// Things
	W1,
	W2,
	HE,
	AR,
	SH,

	// Class
	CL,

	// Hand
	HA,

	// War table
	WTM,
	WTP
}

public class DropSlot: MonoBehaviour {
	public SlotParent slotParent;
	public DropSlotType dropSlotType;
}