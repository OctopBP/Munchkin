using UnityEngine;
using TMPro;

public class MyButton : MonoBehaviour {

	public TextMeshPro textMesh;
	public bool active;

	private MeshRenderer mesh;

	public Color enableColor;
	public Color disableColor;

	private void Start() {
		mesh = GetComponent<MeshRenderer>();
	}

	private void OnMouseDown() {
		if (active)
			GameManager.Instance.EndTurn();
	}

	public void SetActive(bool a) {
		active = a;
		mesh.material.color = a ? enableColor : disableColor;
	}
}