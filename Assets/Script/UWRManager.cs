using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class UWRManager : MonoBehaviour
{
    public static UWRManager instance;

    private void Awake()
    {
        instance = this;
    }
    public void SaveData(List<UserAsnwer> parameters, Action<string> resultcallback)
    {
        StartCoroutine(SendSaveRequest(parameters, resultcallback));
    }

    private IEnumerator SendSaveRequest(List<UserAsnwer> parameters, Action<string> resultcallback)
    {
        //USE IEMUER OR SYNCE WHE SEND REQ(ALSO IS SAMPLE)
        yield return new WaitForSeconds(2.3f);

        var data = JsonConvert.SerializeObject(parameters);
        File.WriteAllText(Application.dataPath + "/Resources/JsonFile/answer.json", data);
        resultcallback(data);
        print("create json");

    }
}
