using System.Collections;
using UnityEngine;

public class PopUpElement : MonoBehaviour {
	const float activateAngle = -90f;
	const float deactivateAngle = 0f;

	[HideInInspector] public float timeToPop;

	private void OnEnable() {
		StopAllCoroutines();
		StartCoroutine(PopUp(true));
	}

	private void OnDisable() {
		StopAllCoroutines();
		if (gameObject.activeInHierarchy) { // avoid error when exiting
			StartCoroutine(PopUp(false));
		}
	}

	IEnumerator PopUp(bool isActive) {
		float inTime;
		if (timeToPop > 0) {
			inTime = timeToPop;
		} else {
			Debug.LogWarning("No TimeToPop defined!");
			inTime = 0.5f;
		}

		var fromAngle = transform.localRotation;
		var toAngle = Quaternion.Euler(Vector3.right * (isActive ? activateAngle : deactivateAngle));
		for (var t = 0f; t < 1f; t += Time.deltaTime / inTime) {
			transform.localRotation = Quaternion.Lerp(fromAngle, toAngle, t);
			yield return null;
		}

		transform.localRotation = toAngle;
	}
}
