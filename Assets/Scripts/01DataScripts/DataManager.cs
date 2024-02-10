using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

//플레이어
public class playerData{
    public string playerName;//유저 이름
    public int money;//돈(나중에 범위 벗어나면 99999로뜨게 설정 해야함)
    public string marry;//결혼 상대 
    public string usingItem;//사용중인 아이템


}
//엔딩
public class endingData{
    public bool start;//스타 
    public bool king;//왕
    public bool tyrant;//폭군(띠띠)
    public bool darklord;//마왕
    public bool knight;//기사
    public bool pope;//교황
    public bool soldier;//군인
    public bool pianist;//피아니스트
    public bool cook;//요리사
    public bool devil;//악마(쿠피)
    public bool clown;//광대
}
//날짜
public class dayData{
    public int year;
    public int month;
    public int day;
    public string weather;
}
//----------------------------싱글톤 (전역 사용 함수 제작)----------------------------------------------------------
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
    //-----------------------------데이터 세이브-------------------------------------------------
    //슬롯 1에 저장하는 함수
    public void SaveData1()
    {
        
        // nowPlayer.playerName="datainstance";//여기에 인풋값을 넣어준다->매니저를 통한 데이터 저장
        nowPlayer.playerName="띠띠님 왕 바보";
        string nowPlayerData1 =JsonUtility.ToJson(nowPlayer);
        File.WriteAllText(path+filename,nowPlayerData1);
     
    }
    //슬롯2에 저장하는 함수 
     public void SaveData2()
    {
        
        // nowPlayer.playerName="datainstance";//여기에 인풋값을 넣어준다->매니저를 통한 데이터 저장
        nowPlayer.playerName="띠띠님 왕 바보";
        string nowPlayerData2 =JsonUtility.ToJson(nowPlayer);
        File.WriteAllText(path+filename,nowPlayerData2);
     
    }
      //슬롯3에 저장하는 함수 
     public void SaveData3()
    {
        
        // nowPlayer.playerName="datainstance";//여기에 인풋값을 넣어준다->매니저를 통한 데이터 저장
        nowPlayer.playerName="띠띠님 왕 바보";
        string nowPlayerData3 =JsonUtility.ToJson(nowPlayer);
        //데이터라는 변수에 nowPlayer객체를 넣어서 json파일로 저장해서 파일에 저장
        File.WriteAllText(path+filename,nowPlayerData3);
     
    }
    //---------------------------데이터 로드---------------------------------------------------------
    //슬롯1 데이터 여는 함수
    public void LoadData1()
    {
        string data = File.ReadAllText(path + filename);
        nowPlayer =JsonUtility.FromJson<playerData>(data);
        //
    }
    //슬롯2 데이터 여는 함수
    public void LoadData2()
    {
        string data = File.ReadAllText(path + filename);
        nowPlayer =JsonUtility.FromJson<playerData>(data);
    }
     //슬롯3 데이터 여는 함수
    public void LoadData3()
    {
        string data = File.ReadAllText(path + filename);
        nowPlayer =JsonUtility.FromJson<playerData>(data);
    }

    //-------------------------------------데이터 사용 ----------------------------------------------
    // Update is called once per frame
    void Update()
    {
        if(Input.anyKey){
            SaveData1();//이런식으로 사용하면 슬롯1에 데이터 저장
            LoadData2();//이런식으로 사용하면 슬롯2에 데이터 저장
            // 함수를 나눠야 하는건지 한 함수에서 변수로 슬롯이름을 받아서 저장할 건지 효율적인 방법 생각 해보기
        }
    }
}
