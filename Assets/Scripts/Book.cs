using System.Collections;
using UnityEngine;

public class Book : MonoBehaviour {
	const float minTimeToTurn = 0.5f;
	const float maxTimeToTurn = 1f;
	const float heightBetweenPages = 0.01f;

	[Header("Drag and Drop")]
	[SerializeField] Transform pivot;
	[SerializeField] Transform pagesParent;
	[Header("If empty, filled with children of Pages Parent")]
	[SerializeField] Page[] pages;
	[Header("(0),  (nb of pages - 1) and (nb of pages) : book closed")]
	[Header("Any number between [1 ; number of pages - 2] : book open")]
	[SerializeField] int currentPage = 0;

	bool isTurning = false;
	float timeToTurn = 0f;

	void OnValidate() {
		if (currentPage < 0) {
			currentPage = 0;
		}
		/*if (Application.isPlaying) {
			Navigate();
		}*/
	}

	void Navigate() {
		if (currentPage % 2 == 1) {
			currentPage++;
		}
		if (pages != null && currentPage > pages.Length) {
			currentPage = pages.Length; // this check when you start on number of page too high
		}
		if (currentPage > 0) {
			StartCoroutine(TurnPages());
		}
	}

	void Start () {
		if (pages.Length == 0) {
			pages = pagesParent.GetComponentsInChildren<Page>();
			if (pages.Length == 0 || pages.Length % 2 != 0) {
				Debug.LogError("Number of pages null, or not even!");
				enabled = false;
				return;
			}
		}
		int i;
		for (i = 1; i < pages.Length - 1; i++) {
			var page = pages[i];
			page.name += " (" + i + ")";
			page.enabled = false;
			if (i % 2 == 1) { // left pages, glued to the back of right pages
				page.transform.SetParent(pages[i - 1].transform);
				page.transform.localPosition = new Vector3(0f, 0f, 0f);
				page.transform.localRotation = Quaternion.Euler(Vector3.forward * 180f);				
			}	else { // right pages, between the two covers
				var pageHeight = 0f; // (pages.Length - 1 - i) * heightBetweenPages;
				page.transform.localPosition = new Vector3(5f, pageHeight, 0f);
				page.transform.localRotation = Quaternion.identity;
			}
		}
		pages[i].transform.SetParent(pages[i - 1].transform);

		Navigate();
	}
	
	void Update () {
		if (!isTurning) { // no key event if the page is turning
			if (Input.GetKeyDown(KeyCode.D) && currentPage < pages.Length - 1) {
				TurnPage(true);
			} else if (Input.GetKeyDown(KeyCode.Q) && currentPage > 0) {
				TurnPage(false);
			}
		}
	}

	void TurnPage(bool toRight) {
		isTurning = true;
		timeToTurn = Random.Range(minTimeToTurn, maxTimeToTurn);

		SetPagesActive(toRight); // +- 2 depending on the side

		var pageIndex = (toRight ? currentPage - 2 : currentPage);
		if (pageIndex == pages.Length) {
			pageIndex--;
		}
		StartCoroutine(TurnPage(pages[pageIndex], toRight));
	}

	void SetPagesActive(bool toRight) {
		if (currentPage > 0) {
			SetPageActive(pages[currentPage - 1], false);
		}
		if (currentPage < pages.Length - 1) {
			SetPageActive(pages[currentPage], false);
		}

		currentPage += (toRight ? 2 : -2);

		if (currentPage > 0) {
			SetPageActive(pages[currentPage - 1], true);
		}
		if (currentPage < pages.Length - 1) {
			SetPageActive(pages[currentPage], true);
		}
	}

	void SetPageActive(Page page, bool isActive) {
		if (page) {
			page.timeToTurn = timeToTurn;
			page.enabled = isActive;
		}
	}

	IEnumerator TurnPages() {
		int targetPage = currentPage;
		currentPage = 0;
		while (currentPage < targetPage) {
			TurnPage(true);
			yield return new WaitWhile(() => isTurning);
		}
	}

	IEnumerator TurnPage(Page page, bool toLeft) {
		float inTime;
		if (timeToTurn > 0) {
			inTime = timeToTurn;
		} else {
			Debug.LogWarning("No TimeToTurn defined!");
			inTime = maxTimeToTurn;
		}

		page.transform.SetParent(pivot); // set the pivot as the parent of target

		// apply new rotation to pivot. 
		var fromAngle = pivot.localRotation;
		var rotationSpeed = 180f / inTime * (toLeft ? 1f : -1f);
		for (var t = 0f; t < 1f; t += Time.deltaTime / inTime) {
			pivot.transform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);
			yield return null;
		}

		page.transform.SetParent(pagesParent); // restore old parents
		//Debug.Log(page.transform.localPosition);
		//page.transform.localPosition = new Vector3((toLeft ? -5 : 5), -0.1f, 0f); // flip the position to secure
		//page.transform.localRotation = Quaternion.Euler(Vector3.forward * (toLeft ? 180 : 0)); // flip the rotation to secure

		isTurning = false;
	}
}
