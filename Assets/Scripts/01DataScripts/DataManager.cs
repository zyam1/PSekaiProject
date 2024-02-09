using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
public class playerData{
    public string playerName;//유저 이름
    public int money;//돈(나중에 범위 벗어나면 99999로뜨게 설정 해야함)
    public string marry;//결혼 상대 
    public string usingItem;//사용중인 아이템


}
public class endingData{
    public bool start;//스타 
    public bool king;//왕
    public bool tyrant;//폭군
    public bool darklord;//마왕
    public bool knight;//기사
    public bool pope;//교황
    public bool soldier;//군인
    public bool pianist;//피아니스트
    public bool cook;//요리사
    public bool devil;//악마
    public bool clown;//광대
}

public class dayData{
    public int year;
    public int month;
    public int day;
    public string weather;
}
public class DataManager : MonoBehaviour
{
    //인스턴스 (=싱글톤:전역 참조 가능) 
    /*
    DataManager.instance.DoSomething();
    외부에서 이런식으로 사용, DoSomething()은 Datamanager 의 메서드 
    */
    public static DataManager instance;
    playerData nowPlayer = new playerData();
    string path;
    string filename="save.json";
    private void Awake(){
        if(instance==null){
            instance=this;
        }else if(instance !=this){
            Destroy(instance.gameObject);
        }
        DontDestroyOnLoad(this.gameObject);
        path = Application.persistentDataPath+"/";
        Debug.Log(path);
    }
    void Start()
    {
       
    }
    public void SaveData()
    {
        // nowPlayer.playerName="datainstance";//여기에 인풋값을 넣어준다
        nowPlayer.playerName="띠띠님 왕 바보";
        string data =JsonUtility.ToJson(nowPlayer);
        File.WriteAllText(path+filename,data);
        Debug.Log(data);
    }
    public void LoadData()
    {
        string data = File.ReadAllText(path + filename);
        nowPlayer =JsonUtility.FromJson<playerData>(data);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.anyKey){
            SaveData();
            Debug.Log("데이터가 저장 되었습니다");
        
        }
    }
}
