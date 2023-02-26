using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class SpriteOption : MonoBehaviour
{
    public Image Img;
    public Button _btn;
    public bool _isCurrect;
    private OptionData _option;

    private void SetupData()
    {
        _isCurrect = _option._isCurrect;
        Img.sprite = _option.sprite;
    }

    public OptionData _spriteOption
    {
        get { return _option; }
        set
        {
            _option = value;
            SetupData();

        }
    }
}

[Serializable]

public class OptionData
{
    public bool _isCurrect;
    public Sprite sprite;
}