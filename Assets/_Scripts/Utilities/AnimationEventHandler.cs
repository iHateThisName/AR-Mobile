using UnityEngine;

public class AnimationEventHandler : MonoBehaviour {

    [SerializeField] private GameObject chainModel;
    private Animator animator;
    private void Start() {
        if (this.chainModel == null) Debug.LogError("Chain Model is not assigned.");
        this.chainModel.SetActive(false);
        animator = GetComponent<Animator>();
    }

    public void OnAnimationEvent(string eventName) {
        Debug.Log($"Animation Event Triggered: {eventName}");
        // You can add additional logic here to handle specific events based on the eventName
    }

    public void OnObeliskAppearingEnd() {
        Debug.Log("Obelisk Appearing Animation Ended");
        // You can add additional logic here to handle the end of the obelisk appearing animation
        CanvasController.Instance.StartNextConversation(EnumDialogueType.Obelisk);
        this.chainModel.SetActive(true);

    }
    public void TurnOffAnimator() {
        this.animator.enabled = false;
    }

    public void OnSolved() {
        this.animator.SetBool("IsSolved", true);
        this.animator.enabled = true;
    }
}
