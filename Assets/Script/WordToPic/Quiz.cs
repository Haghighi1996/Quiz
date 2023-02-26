using System;
using System.Collections.Generic;


[Serializable]
public class QuizList
{
    public Quiz[] quizes;
}


[Serializable]
public class Quiz
{
    public string word;
    public int answer;
    public Dictionary<string, string> options;
    public OptionData[] optionSprite;
}


