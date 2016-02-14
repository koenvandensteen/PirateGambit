using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Clock : MonoBehaviour {

    public RectTransform SecondHand;
    public RectTransform MinuteHand;
    public Text TimerText;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        float time = GameManager.ThisManager.GameTime;

        float angle = (Mathf.Floor(time) / 60.0f) * 360;
        SecondHand.rotation = Quaternion.Euler(0, 0, -angle);

        angle = (Mathf.Floor(time / 60.0f) / 60.0f) * 360;
        MinuteHand.rotation = Quaternion.Euler(0, 0, -angle);

        TimerText.text = string.Format("{0}:{1}", Mathf.FloorToInt(time) / 60, (Mathf.FloorToInt(time) % 60).ToString("00"));
    }
}
