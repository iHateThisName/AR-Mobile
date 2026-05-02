using System;
using UnityEngine;
using UnityEngine.Events;

public class TappController : MonoBehaviour {

    [Header("Model References")]
    [SerializeField] private Transform modelTappOne;
    [SerializeField] private Transform modelTappTwo;
    [SerializeField] private Transform modelPuzzel;

    [Header("Tapp Settings")]
    [SerializeField] private int tappNumber;
    [SerializeField] private UnityEvent OnPuzzelSpawn;


    private void Awake() {
        Transform model = this.modelPuzzel.parent.transform;
        for (int i = 0; i < model.childCount; i++) {
            model.GetChild(i).gameObject.SetActive(false);
        }
    }

    private void Start() {
        this.tappNumber = GameManager.Instance.CurrentTaps;

        if (this.tappNumber == 3) {
            this.modelPuzzel.gameObject.SetActive(true);
            this.modelTappTwo.gameObject.SetActive(false);
            this.modelTappOne.gameObject.SetActive(false);
            this.OnPuzzelSpawn.Invoke();

        } else if (this.tappNumber == 2) {
            this.modelTappTwo.gameObject.SetActive(true);
            this.modelTappOne.gameObject.SetActive(false);

        } else if (this.tappNumber == 1) {
            this.modelTappOne.gameObject.SetActive(true);
        }
    }
}

