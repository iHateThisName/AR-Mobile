using UnityEngine;

public class TappController : MonoBehaviour {

    [Header("Model References")]
    [SerializeField] private Transform modelTappOne;
    [SerializeField] private Transform modelTappTwo;
    [SerializeField] private Transform modelPuzzel;

    private Transform model;

    [Header("Tapp Settings")]
    [SerializeField] private int tappNumber;


    private void Awake() {
        this.model = this.modelPuzzel.parent.transform;
        for (int i = 0; i < model.childCount; i++) {
            model.GetChild(i).gameObject.SetActive(false);
        }
    }

    private void Start() {
        this.tappNumber = GameManager.Instance.CurrentTaps;

        if (this.tappNumber == 3) {
            this.modelPuzzel.gameObject.SetActive(true);

        } else if (this.tappNumber == 2) {
            this.modelTappTwo.gameObject.SetActive(true);

        } else if (this.tappNumber == 1) {
            this.modelTappOne.gameObject.SetActive(true);
        }
    }
}

