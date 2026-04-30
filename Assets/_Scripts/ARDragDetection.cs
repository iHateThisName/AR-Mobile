using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(XRSimpleInteractable))]
public class ARDragDetection : MonoBehaviour {
    // Unity Events
    [Header("Drag Events")]
    [SerializeField] private UnityEvent OnLeftDrag;
    [SerializeField] private UnityEvent OnRightDrag;

    private XRSimpleInteractable interactable;
    private Vector2 initialTouchPosition;
    private Vector2 currentTouchPosition;
    private bool isDragging;

    private void Awake() {
        this.interactable = GetComponent<XRSimpleInteractable>();
    }

    private void OnEnable() {
        // Subscribe to interaction events
        this.interactable.selectEntered.AddListener(OnSelectEntered);
        this.interactable.selectExited.AddListener(OnSelectExited);
    }

    private void OnDisable() {
        // Unsubscribe from interaction events
        this.interactable.selectEntered.RemoveListener(OnSelectEntered);
        this.interactable.selectExited.RemoveListener(OnSelectExited);
    }

    private void Update() {
        if (!this.isDragging || Input.touchCount == 0) return;
        this.currentTouchPosition = Input.GetTouch(0).position;
    }
    private void OnSelectEntered(SelectEnterEventArgs arg0) {
        if (Input.touchCount > 0) {
            this.initialTouchPosition = Input.GetTouch(0).position;
            this.isDragging = true;
        }
    }
    private void OnSelectExited(SelectExitEventArgs arg0) {
        this.isDragging = false;

        Vector2 deltaDrag = this.currentTouchPosition - this.initialTouchPosition;
        HandleDrag(deltaDrag);
    }


    private void HandleDrag(Vector2 delta) {
        if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y)) {
            if (delta.x > 0) this.OnRightDrag?.Invoke();
            else this.OnLeftDrag?.Invoke();
        }
    }
}
