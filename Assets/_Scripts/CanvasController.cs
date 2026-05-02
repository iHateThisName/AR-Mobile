using System;
using UnityEngine;
using UnityEngine.UI;

public class CanvasController : MonoBehaviour {
    [SerializeField] private Button OneButton;
    [SerializeField] private Button TwoButton;
    [SerializeField] private Button ThreeButton;

    [SerializeField] private Button LeftButton;
    [SerializeField] private Button RightButton;

    private PillarController.EnumSegment currentSegment = PillarController.EnumSegment.None;

    private void OnEnable() {
        this.OneButton.onClick.AddListener(OnOneButton);
        this.TwoButton.onClick.AddListener(OnTwoButton);
        this.ThreeButton.onClick.AddListener(OnThreeButton);

        this.LeftButton.onClick.AddListener(OnLeftButton);
        this.RightButton.onClick.AddListener(OnRightButton);
        
        GameManager.TapDetected += OnTap;
    }

    private void OnDisable() {
        this.OneButton.onClick.RemoveListener(OnOneButton);
        this.TwoButton.onClick.RemoveListener(OnTwoButton);
        this.ThreeButton.onClick.RemoveListener(OnThreeButton);

        this.LeftButton.onClick.RemoveListener(OnLeftButton);
        this.RightButton.onClick.RemoveListener(OnRightButton);

        GameManager.TapDetected -= OnTap;
    }


    private void Start() {
        DisableAllButtons();
    }

    private void OnOneButton() => this.currentSegment = PillarController.EnumSegment.Top;
    private void OnTwoButton() => this.currentSegment = PillarController.EnumSegment.Middle;
    private void OnThreeButton() => this.currentSegment = PillarController.EnumSegment.Bottom;

    private void OnLeftButton() {
        if (GameManager.Instance.CurrentTaps != 3) return;
        if (this.currentSegment == PillarController.EnumSegment.None) return;
        GameManager.Instance.MoveSegment(this.currentSegment, true);
    }

    private void OnRightButton() {
        if (GameManager.Instance.CurrentTaps != 3) return;
        if (this.currentSegment == PillarController.EnumSegment.None) return;
        GameManager.Instance.MoveSegment(this.currentSegment, false);
    }
    private void OnTap() {
        if (GameManager.Instance.CurrentTaps == GameManager.Instance.MaxTaps) {
            EnableAllButtons();
        }
    }
    private void EnableAllButtons() {
        this.OneButton.gameObject.SetActive(true);
        this.TwoButton.gameObject.SetActive(true);
        this.ThreeButton.gameObject.SetActive(true);
        this.LeftButton.gameObject.SetActive(true);
        this.RightButton.gameObject.SetActive(true);
    }
    private void DisableAllButtons() {
        this.OneButton.gameObject.SetActive(false);
        this.TwoButton.gameObject.SetActive(false);
        this.ThreeButton.gameObject.SetActive(false);
        this.LeftButton.gameObject.SetActive(false);
        this.RightButton.gameObject.SetActive(false);
    }

}
