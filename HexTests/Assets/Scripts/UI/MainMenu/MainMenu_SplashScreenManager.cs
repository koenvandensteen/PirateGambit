//----------------------------------------------------------------------------------------------------------
//Copyright (c) 2016 Koen Van Den Steen (http://koenvds.com), Thomas Van Riel (http://www.thomasvanriel.com)
//All Rights Reserved
//----------------------------------------------------------------------------------------------------------
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MainMenu_SplashScreenManager : MonoBehaviour {

    [System.Serializable]
    public class SplashScreen {
        [Tooltip("-1 for final screen")]
        public float DisplayTime = 1.0f;
        [HideInInspector]
        public float Counter;
        public GameObject Canvas;
        public float FadeTime = 1.0f;
        private bool _isFading = false;
        public bool IsFading { get { return _isFading; } }
        private bool _isFullyTransparent = false;
        public bool IsFullyTransparent { get { return _isFullyTransparent; } }

        private float _opacity = 1.0f;

        public IEnumerator Fade() {
            _isFading = true;
            while (_opacity > 0) {
                if (FadeTime < Mathf.Epsilon)
                    _opacity = 0.0f;
                else
                    _opacity -= Time.deltaTime / FadeTime;
                foreach (var item in this.Canvas.GetComponentsInChildren<Graphic>()) {
                    Color color = item.color;
                    color.a = _opacity;
                    item.color = color;
                }
                yield return new WaitForEndOfFrame();
            }
            _isFullyTransparent = true;
        }
    }

    public SplashScreen[] SplashScreens;
    private int _index = 0;

    // Use this for initialization
    void Awake() {
        foreach (var item in SplashScreens) {
            item.Counter = item.DisplayTime;
            item.Canvas.SetActive(false);
        }

        SplashScreens[0].Canvas.SetActive(true);

        if (PlayerPrefs.GetInt("Splash") == 0) {

            for (int i = 0; i < SplashScreens.Length - 1; i++) {
                SplashScreens[i].Canvas.SetActive(false);
            }

            SplashScreens[SplashScreens.Length - 1].Canvas.SetActive(true);
            AudioManager.Instance.StartMusic(0);
            this.enabled = false;
        }
    }

    // Update is called once per frame
    void Update() {
        var curScreen = SplashScreens[_index];
        if (curScreen.Counter < 0) {
            if (!curScreen.IsFading) {
                StartCoroutine(curScreen.Fade());
                if (_index + 1 < SplashScreens.Length) {
                    SplashScreens[_index + 1].Canvas.SetActive(true);
                }
                if (_index + 1 == SplashScreens.Length - 1) {
                    AudioManager.Instance.StartMusic(curScreen.FadeTime);

                }
            }
        } else {
            curScreen.Counter -= Time.deltaTime;
        }

        if (curScreen.IsFullyTransparent) {
            curScreen.Canvas.SetActive(false);
            if (++_index < SplashScreens.Length) {
                if (SplashScreens[_index].DisplayTime == -1) {
                    this.enabled = false;
                }
            }
        }


    }
}
