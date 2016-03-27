//----------------------------------------------------------------------------------------------------------
//Copyright (c) 2016 Koen Van Den Steen (http://koenvds.com), Thomas Van Riel (http://www.thomasvanriel.com)
//All Rights Reserved
//----------------------------------------------------------------------------------------------------------
using UnityEngine;
using UnityEngine.UI;
using System.Collections;


[RequireComponent(typeof(Image))]
public class TreasureProgressBar : MonoBehaviour
{
    private Image _myImage;
    private Slider _sliderProgress;
    private Slider _sliderDanger;
   
    void Start()
    {
        _myImage = GetComponent<Image>();
        _sliderProgress = GetComponentInChildren<Slider>();
        _sliderDanger = GetComponentsInChildren<Slider>()[1];
    }
    // Update is called once per frame
    void Update()
    {
        var manager = GameManager.Instance;

        if (_myImage != null) {
        _myImage.fillAmount = (float)manager.CollectedTreasureAmount / manager.MaxTreasureAmmount;
        }

        if (_sliderProgress != null) {
            _sliderProgress.value = (float)manager.CollectedTreasureAmount / manager.MaxTreasureAmmount;
        }

        _sliderDanger.value = (float)manager.CurDangerlevel / 6;

    }
}
