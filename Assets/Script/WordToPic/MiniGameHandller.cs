using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;
using UnityEngine.UI;
using TMPro;
using System;
using System.Threading.Tasks;
using UnityEngine.Tilemaps;
using Newtonsoft.Json.Linq;
using Unity.Mathematics;

public class MiniGameHandller : MonoBehaviour, IGameLogic
{
    #region peraperty
    public QuizHandler _quizHandler;
    [SerializeField]
    private int _gameRounds;
    [SerializeField]
    private GameObject _answerpage;
    [SerializeField]
    private TextMeshProUGUI pointtxt;
    [SerializeField]
    private GameObject _feadBackGameTxt;
    [SerializeField]
    private GameObject _showResultPnl;
    [SerializeField]
    private GameObject _contentResultParent;
    #endregion

    private int _currentRound;
    private int _currectAnswerCount;
    private string m_JsonPath = "/Resources/JsonFile/Data.json";
    private string m_Json;
    private List<UserAsnwer> _answerdata = new List<UserAsnwer>();

    public Quiz[] _quizList;

    public void Start()
    {
        Init();
    }

    public void OnEnable()
    {
        _quizHandler.OnanswerGame += OnShowAnswer;
    }

    public async void Init()
    {
        await GetData();
        NextQuiz();
    }

    //IF USE APIS NEED TO USE ASYNCE AND THIS SAMPLE
    private async Task<string> GetData()
    {

        m_Json = File.ReadAllText(Application.dataPath + m_JsonPath).ToString();
        QuizList _tmpList = JsonConvert.DeserializeObject<QuizList>(m_Json);
        if (_tmpList != null)
        {
            for (int i = 0; i < _tmpList.quizes.Length; i++)
            {
                SetupQuiz(_tmpList.quizes[i]);
            }
            _quizList = ShuffleQuizList(_tmpList.quizes);

        }
        return "completed";
    }

    private void SetupQuiz(Quiz _quizdata)
    {
        List<OptionData> _spritesoption = new List<OptionData>();
        foreach (var _tmp in _quizdata.options)
        {
            OptionData _tmpOption = new OptionData();

            _tmpOption.sprite = _quizHandler.GetSprite(_tmp.Value);

            if (_tmp.Key == _quizdata.answer.ToString())
                _tmpOption._isCurrect = true;

            _spritesoption.Add(_tmpOption);
        }
        _quizdata.optionSprite = _spritesoption.ToArray();
    }

    private void NextQuiz()
    {
        _currentRound++;

        _quizHandler.ResetData();

        if (!(_currentRound > _gameRounds))
        {
            _quizHandler.GetQuiz(_quizList[_currentRound - 1]);
        }
        else
        {
            StartCoroutine(CheckPointsResult());
        }

    }

    private Quiz[] ShuffleQuizList(Quiz[] list)
    {
        for (int i = 0; i < list.Length; i++)
        {
            Quiz temp = list[i];
            int randomIndex = UnityEngine.Random.Range(i, list.Length);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
        return list;
    }

    private IEnumerator CheckPointsResult()
    {
        yield return new WaitForSeconds(1f);

        if ((_currentRound > _gameRounds))
        {
            _quizHandler.ResetData();
            _answerpage.SetActive(false);
            if (_currectAnswerCount >= 4)
            {
                ShowResult(Color.green, "Awesome");
            }
            else if (_currectAnswerCount == 3)
            {
                ShowResult(Color.yellow, "GoodJob");
            }
            else if (_currectAnswerCount <= 2)
            {
                ShowResult(Color.red, "Terrible");
            }

            UWRManager.instance.SaveData(_answerdata, (_result) =>
            {
                //End();
                OnShowResultPoints(_result);
            });
        }
        else
        {
            pointtxt.text = _currectAnswerCount.ToString();
            _answerpage.SetActive(false);
            NextQuiz();
        }
    }

    private void OnShowResultPoints(string _jsondata)
    {
        List<UserAsnwer> _resultAnswers = JsonConvert.DeserializeObject<List<UserAsnwer>>(_jsondata);
        GameObject _resultPrefab = DataManager.instance.GetGameData(GameDataEnum.wordtopic).subPrefabs[1];

        if (_resultAnswers != null && _resultAnswers.Count > 0)
        {
            _showResultPnl.SetActive(true);
            _quizHandler.End();

            for (int i = 0; i < _resultAnswers.Count; i++)
            {
                GameObject _resultItem = Instantiate(_resultPrefab, transform.position, transform.rotation);
                _resultItem.transform.parent = _contentResultParent.transform;
                _resultItem.transform.localScale = new Vector3(2, 2, 1);
                TextMeshProUGUI[] resulttxt = _resultItem.GetComponentsInChildren<TextMeshProUGUI>();
                resulttxt[0].text = i.ToString();
                resulttxt[1].text = _resultAnswers[i].currectanswer.ToString();
                resulttxt[2].text = _resultAnswers[i].useranswer.ToString();
            }
        }
        else
        {
            End();
        }
    }

    public void OnShowAnswer(bool _isCurrect)
    {
        if (_isCurrect)
        {
            _currectAnswerCount++;
            _answerpage.GetComponent<Image>().color = Color.green;
        }
        else
        {
            _answerpage.GetComponent<Image>().color = Color.red;
        }

        int _currectaswer = (_isCurrect) ? 1 : 0;
        _answerpage.SetActive(true);
        UserAsnwer _newanswer = new UserAsnwer
        {
            word = _quizList[_currentRound - 1].word,
            useranswer = _currectaswer,
            currectanswer = _quizList[_currentRound - 1].answer
        };
        _answerdata.Add(_newanswer);
        StartCoroutine(CheckPointsResult());

    }

    private void ShowResult(Color _color, string feadback)
    {
        _feadBackGameTxt.SetActive(true);
        TextMeshProUGUI _text = _feadBackGameTxt.GetComponent<TextMeshProUGUI>();
        _text.color = _color;
        _text.text = feadback;
    }

    public void ResetData()
    {
        _currentRound = 0;
        _currectAnswerCount = 0;
        _quizHandler.ResetData();
    }

    public void End()
    {
        Destroy(this.gameObject);
    }
}

[Serializable]
public class UserAsnwer
{
    public string word;
    public int useranswer;
    public int currectanswer;
}
