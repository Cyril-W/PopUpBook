using UnityEngine;

public class Page : MonoBehaviour {
	const float TurnPopUpRation = 2 / 3f;

	[HideInInspector] public float timeToTurn = 0f;

	PopUpElement[] popUpElements;

	private void Awake() {
		popUpElements = GetComponentsInChildren<PopUpElement>();
		SetPopUpElementsActive(false);
	}

	private void OnEnable() {
		SetPopUpElementsActive(true);
	}

	private void OnDisable() {
		SetPopUpElementsActive(false);
	}

	void SetPopUpElementsActive (bool isActive) {
		foreach (var popUpElement in popUpElements) {
			popUpElement.timeToPop = timeToTurn * TurnPopUpRation;
			popUpElement.enabled = isActive;
		}
	}
}
