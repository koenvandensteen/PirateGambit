using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LabelUpdater : MonoBehaviour {

    public enum Target {
        Treasure,
        Krakens
    }

    public Target TargetType;

    // Use this for initialization
    void Awake() {
        switch (TargetType) {
            case Target.Treasure:
                GameManager.Instance.TreasureChangedImplementation += UpdateText;
                break;
            case Target.Krakens:
                GameManager.Instance.CurKrakenAmmountChangedImplementation += UpdateText;
                break;
            default:
                break;
        }

    }

    void UpdateText(int value) {

        int max = 0;

        switch (TargetType) {
            case Target.Treasure:
                max = GameManager.Instance.MaxTreasureAmmount;
                break;
            case Target.Krakens:
                max = GameManager.Instance.MaxKrakenAmmount;
                break;
            default:
                break;
        }


        GetComponent<Text>().text = string.Format("{0}/{1}", value, max);
    }
}
