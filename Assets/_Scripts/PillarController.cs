using Assets.Scripts.Singleton;
using System.Collections;
using UnityEngine;

public class PillarController : Singleton<PillarController> {

    [Header("Pillar Segments")]
    [SerializeField] private Transform topSegment;
    [SerializeField] private Transform middleSegment;
    [SerializeField] private Transform bottomSegment;
    [SerializeField] private Transform topSeprationSegment;
    [SerializeField] private Transform bottomSeprationSegment;

    [SerializeField] private Vector2 seperationRotationSpeedMinMax = new Vector2(2f, 4f);
    [SerializeField] private Vector2 rotationSpeedMinMax = new Vector2(32f, 35f);

    [SerializeField] private AnimationEventHandler animationEventHandler;
    private enum EnumRunes { None, Cross, Triangle, Circle, Blank }
    [System.Serializable] public enum EnumSegment { None, Top, Middle, Bottom }

    private int rotationCost = 60;

    private EnumRunes currentTopRune;
    private EnumRunes currentMiddleRune;
    private EnumRunes currentBottomRune;

    private Coroutine currentTopRotationCoroutine, currentMiddleRotationCoroutine, currentBottomRotationCoroutine;

    private void Start() {
        if (this.animationEventHandler == null) {
            this.animationEventHandler = this.transform.root.GetComponentInChildren<AnimationEventHandler>();
        }

        float topSeparationRotationSpeed = Random.Range(seperationRotationSpeedMinMax.x, seperationRotationSpeedMinMax.y);
        float bottomSeparationRotationSpeed = Random.Range(seperationRotationSpeedMinMax.x, seperationRotationSpeedMinMax.y);

        StartCoroutine(IdleRotate(topSeprationSegment, topSeparationRotationSpeed));
        StartCoroutine(IdleRotate(bottomSeprationSegment, bottomSeparationRotationSpeed));

        this.currentTopRune = EnumRunes.Circle;
        this.currentMiddleRune = EnumRunes.Cross;
        this.currentBottomRune = EnumRunes.Circle;
    }

    [ContextMenu("Debug Middle Side")] public void DebugMiddleSide() => Debug.Log($"Current Middle Rune: {GetRune(EnumSegment.Middle)}");
    [ContextMenu("Rotate Middle Left")] public void RotateMiddleLeft() => RotateSegment(EnumSegment.Middle, true);
    [ContextMenu("Rotate Middle Right")] public void RotateMiddleRight() => RotateSegment(EnumSegment.Middle, false);

    [ContextMenu("Debug front all segments")] public void DebugFrontAllSegments() {
        string debugText = $"Current Top Rune: {GetRune(EnumSegment.Top)}\n"
                           + $"Current Middle Rune: {GetRune(EnumSegment.Middle)}\n"
                           + $"Current Bottom Rune: {GetRune(EnumSegment.Bottom)}";
        Debug.Log(debugText);
    }

    [ContextMenu("Check win condition")] public void CheckWinConditionContext() => CheckWinCondition();

    public void RotateLeftByEnumIndex(int segmentIndex) => RotateSegment((EnumSegment)segmentIndex, true);
    public void RotateRightByEnumIndex(int segmentIndex) => RotateSegment((EnumSegment)segmentIndex, false);

    private IEnumerator IdleRotate(Transform targetTransform, float rotationSpeed) {
        float maxRotation = this.rotationCost - 5f;

        while (true) {
            float amountRotated = 0f;

            // Rotate until we reach our target amount
            while (amountRotated < maxRotation) {
                // Calculate rotation step for this frame
                float step = rotationSpeed * Time.deltaTime;

                targetTransform.Rotate(Vector3.up, step);
                amountRotated += Mathf.Abs(step);

                yield return null;
            }

            // Reverse direction after reaching the target angle
            rotationSpeed = -rotationSpeed;
        }
    }

    public void RotateSegment(EnumSegment segment, bool isLeft) {
        float randomRotationSpeed = Random.Range(rotationSpeedMinMax.x, rotationSpeedMinMax.y);
        if (!isLeft) randomRotationSpeed *= -1; // Reverse direction for right rotation
        switch (segment) {
            case EnumSegment.Top:
                if (this.currentTopRotationCoroutine != null) break; // Prevent starting a new rotation if one is already in progress
                this.currentTopRotationCoroutine = StartCoroutine(RotateSegment(this.topSegment, EnumSegment.Top, randomRotationSpeed));
                break;
            case EnumSegment.Middle:
                if (this.currentMiddleRotationCoroutine != null) break; // Prevent starting a new rotation if one is already in progress
                this.currentMiddleRotationCoroutine = StartCoroutine(RotateSegment(this.middleSegment, EnumSegment.Middle, randomRotationSpeed));
                break;
            case EnumSegment.Bottom:
                if (this.currentBottomRotationCoroutine != null) break; // Prevent starting a new rotation if one is already in progress
                this.currentBottomRotationCoroutine = StartCoroutine(RotateSegment(this.bottomSegment, EnumSegment.Bottom, randomRotationSpeed));
                break;
        }
    }

    private IEnumerator RotateSegment(Transform transform, EnumSegment segment, float rotationSpeed) {
        if (segment == EnumSegment.None) yield break; // Invalid segment, exit coroutine
        float amountRotated = 0f; // Track how much we've rotated to ensure we don't exceed the rotation cost

        while (amountRotated <= this.rotationCost) {

            float step = rotationSpeed * Time.deltaTime; // Calculate rotation step for this frame
            transform.Rotate(Vector3.up, step); // Rotate the segment

            amountRotated += Mathf.Abs(step);

            yield return null; // Wait for the next frame
        }

        // After rotation is complete, snap to the nearest valid angle to ensure we end up in a correct position.
        SnapToValidAngle(transform, out int validAngle);

        if (segment == EnumSegment.Top) this.currentTopRotationCoroutine = null;
        else if (segment == EnumSegment.Middle) this.currentMiddleRotationCoroutine = null;
        else if (segment == EnumSegment.Bottom) this.currentBottomRotationCoroutine = null;

        StartCoroutine(DelayedCheckWinCondition());
    }

    private IEnumerator DelayedCheckWinCondition() {
        yield return new WaitForSeconds(0.25f);
        CheckWinCondition();
    }

    private bool SnapToValidAngle(Transform transform, out int validAngle) {
        int angle = Mathf.RoundToInt(transform.localEulerAngles.y); // Using localEulerAngles is safer in case the parent is rotated.
        bool result = IsValidAngle(angle, out validAngle);

        if (result) {
            Vector3 newEulerAngles = transform.localEulerAngles;
            newEulerAngles.y = validAngle;
            transform.localEulerAngles = newEulerAngles;
        }

        return result;
    }

    private bool IsValidAngle(int angle, out int validAngle) {
        int remainder = angle % this.rotationCost;
        int distanceToNearest = Mathf.Min(remainder, this.rotationCost - remainder);

        if (distanceToNearest > 2) {
            validAngle = angle;
            return false;
        }

        validAngle = (angle + distanceToNearest) % 360;
        if (validAngle % this.rotationCost != 0) {
            validAngle = (angle - distanceToNearest) % 360;
        }

        return true;
    }

    private EnumRunes GetRune(EnumSegment segment) {
        EnumRunes result = EnumRunes.None; // Default value if no valid rune is found

        // Get the transform reference of the specified segment. Using localEulerAngles is safer in case the parent is rotated.
        Transform targetTransform = segment switch {
            EnumSegment.Top => this.topSegment,
            EnumSegment.Middle => this.middleSegment,
            EnumSegment.Bottom => this.bottomSegment,
            _ => null
        };

        if (targetTransform == null) {
            Debug.LogWarning($"GetRune is not implemented for {segment}. Returning None.");
            return EnumRunes.None;
        }

        // Use a small tolerance for floating point inaccuracies since object transformations can result in slight drifting
        if (!SnapToValidAngle(targetTransform, out int validAngle)) {
            return EnumRunes.None;
        }

        switch (segment) {
            case EnumSegment.Top:
                if (validAngle == 0) {
                    result = EnumRunes.Circle;
                } else if (validAngle == this.rotationCost * 2) {
                    result = EnumRunes.Triangle;
                } else if (validAngle == this.rotationCost * 4) {
                    result = EnumRunes.Cross;
                }

                if (result != this.currentTopRune) {
                    this.currentTopRune = result;
                }
                break;
            case EnumSegment.Middle:
                if (validAngle == 0) {
                    result = EnumRunes.Cross;
                } else if (validAngle == this.rotationCost * 2) {
                    result = EnumRunes.Triangle;
                } else if (validAngle == this.rotationCost * 4) {
                    result = EnumRunes.Circle;
                } else {
                    result = EnumRunes.Blank;
                }

                // Update current middle rune if it has changed
                if (result != this.currentMiddleRune) {
                    this.currentMiddleRune = result;
                }
                break;
            case EnumSegment.Bottom:
                if (validAngle == 0) {
                    result = EnumRunes.Circle;
                } else if (validAngle == this.rotationCost * 2) {
                    result = EnumRunes.Cross;
                } else if (validAngle == this.rotationCost * 4) {
                    result = EnumRunes.Triangle;
                }

                if (result != this.currentBottomRune) {
                    this.currentBottomRune = result;
                }
                break;
        }

        Debug.Log($"GetRune: Segment={segment}, ValidAngle={validAngle}, Result={result}");
        return result;
    }

    private void CheckWinCondition() {
        GetRune(EnumSegment.Top);
        GetRune(EnumSegment.Middle);
        GetRune(EnumSegment.Bottom);

        bool front = this.currentTopRune == EnumRunes.Circle && this.currentMiddleRune == EnumRunes.Circle && this.currentBottomRune == EnumRunes.Triangle;
        bool leftFront = this.currentTopRune == EnumRunes.Cross && this.currentMiddleRune == EnumRunes.Triangle && this.currentBottomRune == EnumRunes.Circle;
        bool rightFront = this.currentTopRune == EnumRunes.Triangle && this.currentMiddleRune == EnumRunes.Cross && this.currentBottomRune == EnumRunes.Cross;
        if (front || leftFront || rightFront) {
            Debug.Log("Win Condition Met!");
            StartCoroutine(GameManager.Instance.GameWin(this.animationEventHandler));
        }
    }

    public void DisableModels() {
        this.topSegment.gameObject.SetActive(false);
        this.middleSegment.gameObject.SetActive(false);
        this.bottomSegment.gameObject.SetActive(false);
        this.topSeprationSegment.gameObject.SetActive(false);
        this.bottomSeprationSegment.gameObject.SetActive(false);
    }
}
