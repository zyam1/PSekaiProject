using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class YarnDataInterAct : MonoBehaviour
{
    // Start is called before the first frame update
    // Dialogue Runner에서 대화 시작 및 선택지를 처리하기 위한 변수
        private DialogueRunner dialogueRunner;
    void Start()
    {
        //싱글톤 접근 플레이어 이름
        string playerName = DataManager.instance.nowPlayer.playerName;
        // DialogueRunner 인스턴스 가져오기
        dialogueRunner = GetComponent<DialogueRunner>();
        // DialogueRunner 인스턴스 가져오기
        dialogueRunner = GetComponent<DialogueRunner>();
        
        // 대화 시작 (예: "Start"라는 대화 노드에서 시작)
        dialogueRunner.StartDialogue("Start");
        
    }

    public void SetPlayerName(string newName)
    {
        DataManager.instance.nowPlayer.playerName = newName;
        Debug.Log("Player Name Updated to: " + DataManager.instance.nowPlayer.playerName);
    }

    
    [YarnCommand("setName")]
    public void SetName(string name)
    {
        SetPlayerName(name);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
