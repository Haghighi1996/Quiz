
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using System.IO;
using UnityEngine.UI;
using System.Collections;
using System.Linq;
using System.ComponentModel;

public class QuizHandler : MonoBehaviour, IGameLogic
{
    #region peraperty
    private GameObject _optionPrefab;
    [SerializeField]
    private TextMeshProUGUI _wordTxt;
    [SerializeField]
    private ScrollRect _scrollRect;
    [SerializeField]
    private GameObject _contactParent;
    #endregion

    public delegate void OnAnswerGame(bool iscurrect);
    public event OnAnswerGame OnanswerGame;

    private List<GameObject> _optionListtmp;
    public List<Sprite> _refrenceSprites;
    internal Quiz CurrentQuiz;
    private bool _isanswered;

    public void Awake()
    {
        Init();
    }

    public void Init()
    {
        _optionListtmp = new List<GameObject>();
        _refrenceSprites = new List<Sprite>();
        _optionPrefab = DataManager.instance.GetGameData(GameDataEnum.wordtopic).subPrefabs[0];
        GetRefrenceSprites();
    }

    public void GetQuiz(Quiz _value)
    {
        CurrentQuiz = _value;
        SetupData();
    }

    public void SetupData()
    {
        _wordTxt.text = CurrentQuiz.word.ToString();
        SetupOptionsObjects((_canstart) => OnStartQuiz(_canstart));
    }

    private void SetupOptionsObjects(Action<bool> callback = null)
    {
        SuffleOptions();
        for (int i = 0; i < CurrentQuiz.optionSprite.Length; i++)
        {
            GameObject _optionObject = Instantiate(_optionPrefab, transform.position, transform.rotation);
            _optionObject.transform.parent = _contactParent.transform;
            _optionObject.transform.localScale = new Vector3(0.7f,0.7f,1);
            _optionObject.SetActive(false);
            SpriteOption _option = _optionObject.GetComponent<SpriteOption>();
            _option._spriteOption = CurrentQuiz.optionSprite[i];
            _option._btn.onClick.AddListener(delegate { OnClickOption(_option._isCurrect); });
            _optionListtmp.Add(_optionObject);
            if (i == CurrentQuiz.optionSprite.Length - 1)
            {
                callback(true);
            }
        }

    }

    private void SuffleOptions()
    {
        List<Sprite> _savelist = new List<Sprite>();
        foreach(var sprite in _refrenceSprites)
        {
            _savelist.Add(sprite);
        }

        Sprite _answerSprite = Array.Find(CurrentQuiz.optionSprite, x => x._isCurrect).sprite;
        if (_answerSprite != null)
        {
           _refrenceSprites =_refrenceSprites.Where(_x=>_x.name!=_answerSprite.name).ToList();
        }

        for (int i = 0; i < CurrentQuiz.optionSprite.Length; i++)
        {     
            if (!CurrentQuiz.optionSprite[i]._isCurrect)
            {
                OptionData temp = CurrentQuiz.optionSprite[i];
                int randomIndex = UnityEngine.Random.Range(i, (_refrenceSprites.Count - 1));
                Sprite _randomsprite = _refrenceSprites[randomIndex];
                temp.sprite = _randomsprite;
                _refrenceSprites.Remove(_randomsprite);
            }
        }

        _refrenceSprites=_savelist;
    }

    private void GetRefrenceSprites()
    {
        List<string> _refrences = new List<string>();
        _refrences = DataManager.instance.GetGameData(GameDataEnum.wordtopic).dataRefrences.ToList();
        for (int i = 0; i <_refrences.Count; i++)
        {
            Sprite _sprite = GetSprite(_refrences[i].ToString());
            _refrenceSprites.Add(_sprite);
        }
    }

    private void OnStartQuiz(bool canStart)
    {
        _wordTxt.gameObject.SetActive(canStart);
        StartCoroutine(AutoScroll(_scrollRect, 1, 0,13f));
    }

    IEnumerator AutoScroll(ScrollRect scrollRect, float startpos, float endpos, float dur)
    {
        yield return new WaitForSeconds(2f);

        _wordTxt.gameObject.SetActive(false);
        OnEnableOptions(true);

        float timefirst = 0.0f;
        while (timefirst < 1.0f && !_isanswered)
        {
            timefirst += Time.deltaTime / dur;
            scrollRect.horizontalNormalizedPosition = Mathf.Lerp(startpos, endpos, timefirst);
            scrollRect.verticalNormalizedPosition = Mathf.Lerp(startpos, endpos, timefirst);
            yield return null;
        }
        if (timefirst >= 1.0f&&!_isanswered)
        {
            StopQuizing();

            if (OnanswerGame != null)
                OnanswerGame(false);
        }
    }

    public Sprite GetSprite(string _link)
    {
        byte[] fileData = null;
        if (File.Exists(Application.dataPath + _link))
        {
            fileData = File.ReadAllBytes(Application.dataPath + _link);
        }
        Texture2D tex = new Texture2D(2, 2);
        tex.LoadImage(fileData);
        Rect rec = new Rect(0, 0, tex.width, tex.height);
        Sprite spriteToUse = Sprite.Create(tex, rec, new Vector2(0.5f, 0.5f), 100);
        spriteToUse.name = _link.Substring(19);
        return spriteToUse;
    }

    private void OnClickOption(bool _iscurrect)
    {
        StopQuizing();
        OnEnableOptions(false);
        
        if (OnanswerGame != null)
            OnanswerGame(_iscurrect);

    }

    private void OnEnableOptions(bool _isshow)
    {
        for (int i = 0; i < _optionListtmp.Count; i++)
        {
            _optionListtmp[i].SetActive(_isshow);
        }
    }

    private void StopQuizing()
    {
        _scrollRect.horizontalNormalizedPosition = 1;
        _scrollRect.verticalNormalizedPosition = 1;
        _isanswered = true;
    }

    public void ResetData()
    {
        CurrentQuiz = null;
        _isanswered = false;

        _wordTxt.gameObject.SetActive(false);

        _scrollRect.horizontalNormalizedPosition = 1;
        _scrollRect.verticalNormalizedPosition = 1;

        if (_optionListtmp != null && _optionListtmp.Count > 0)
        {
            foreach (var item in _optionListtmp)
            {
                Destroy(item);
            }
            _optionListtmp.Clear();
        }
       
    }

    public void End()
    {
        gameObject.SetActive(false);
    }
}



