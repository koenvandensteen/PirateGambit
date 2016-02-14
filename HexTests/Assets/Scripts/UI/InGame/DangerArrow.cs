using UnityEngine;
using System.Collections;

public class DangerArrow : MonoBehaviour {

    public float MinAngle;
    public float MaxAngle;

    void Awake() {
        GameManager.ThisManager.DangerLevelChangedImplementation += UpdateAngle;
    }

    void UpdateAngle(int value) {

        float delta = value / 5.0f;
        GetComponent<RectTransform>().rotation = Quaternion.Euler(0, 0, Mathf.Lerp(MinAngle, MaxAngle, delta));
    }
}
