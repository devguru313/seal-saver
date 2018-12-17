using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ReadInputJSONResponse
{
    public int PlayerID;
    public string RESTEndPoint;
    public string UserID;
    public string data;
    public string phpOrREST;
    public string status;
    public List<QuestionHint> Hints;
    //public QuestionHint[] Hints;
}

[Serializable]
public class QuestionHint
{
    public string QuestionID;
    public string Hint;
}
