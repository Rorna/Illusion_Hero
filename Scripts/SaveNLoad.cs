using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//세이브 로드 관련 라이브러리
using System.IO; //파일 입출력
using System.Runtime.Serialization.Formatters.Binary; //직렬화 된걸 바이너리 파일로 만듬


public class SaveNLoad : MonoBehaviour
{
    [System.Serializable] //save load시 필수 속성
    public class Data //모든세이브 기록 담을 클래스
    {
        //플레이어의 위치값(벡터는 직렬화가 되면 못씀, 그래서 float으로 선언함)
        public float playerX;
        public float playerY;
        public float playerZ;

        public int playerLv;
        public int playerHP;
        public int playerMP;

        public int playerCurrentHP;
        public int playerCurrentMP;
        public int playerCurrentEXP;

        public int playerHPR; //회복량
        public int playerMPR; //회복량

        public int playerATK;
        public int playerDEF;

        public int added_atk;
        public int added_def;
        public int added_hpr;
        public int added_mpr;

        //아이템을 아이템의 아이디값으로 관리
        public List<int> playerItemInventory; //플레이어가 가지고 있는 아이템
        public List<int> playerItemInventoryCount; //아이템 소지수
        public List<int> playerEquipItem; //장비한 아이템의 아이디값 저장

        public string mapName; //어느 맵에 있었는지
        public string sceneName; //어느 씬에 있었는지

        //데이터 베이스 변수 관리
        public List<bool> swList; //데이터베이스에 있던 스위치들 일제히 관리. 일제 저장하고 한꺼번에 불러올거임
        public List<string> swNameList; //스위치의 이름
        public List<string> varNameList; //변수
        public List<float> varNumberList; 

    }

    private PlayerManager thePlayer; //플레이어의 위치 취득용
    private PlayerStat thePlayerStat; //플레이어 스탯 취득
    private DatabaseManager theDatabase; 
    private Inventory theInven;
    private Equipment theEquip;
    private FadeManager theFade; //페이드 효과

    public Data data; //선언해놓은 클래스 사용

    private Vector3 vector; //여기에 플레이어 위치 담고 이것을 이용해서 float으로 선언된 플레이어의 위치 불러옴

    public void CallSave() //세이브 함수, 모든 정보 기록함
    {
        //초기화
        theDatabase = FindObjectOfType<DatabaseManager>();
        thePlayer = FindObjectOfType<PlayerManager>();
        thePlayerStat = FindObjectOfType<PlayerStat>();
        theEquip = FindObjectOfType<Equipment>();
        theInven = FindObjectOfType<Inventory>();

        //플레이어의 위치 삽입
        data.playerX = thePlayer.transform.position.x;
        data.playerY = thePlayer.transform.position.y;
        data.playerZ = thePlayer.transform.position.z;

        //기타 스탯 및 요소 삽입
        data.playerLv = thePlayerStat.character_Lv;
        data.playerHP = thePlayerStat.hp;
        data.playerMP = thePlayerStat.mp;

        data.playerCurrentHP = thePlayerStat.currentHP;
        data.playerCurrentMP = thePlayerStat.currentMP;
        data.playerCurrentEXP = thePlayerStat.currentEXP;

        data.playerATK = thePlayerStat.atk;
        data.playerDEF = thePlayerStat.def;
        data.playerMPR = thePlayerStat.recover_mp;
        data.playerHPR = thePlayerStat.recover_hp;

        data.added_atk = theEquip.added_atk;
        data.added_def = theEquip.added_def;
        data.added_hpr = theEquip.added_hpr;
        data.added_mpr = theEquip.added_mpr;

        data.mapName = thePlayer.currentMapName;
        data.sceneName = thePlayer.currentSceneName;

        //인벤토리나 장비템
        Debug.Log("기초 데이터 성공");

        //아이템 중복 저장으로 복사가 이루어지는것 방지
        data.playerItemInventory.Clear();
        data.playerItemInventoryCount.Clear();
        data.playerEquipItem.Clear();


        //데이터베이스의 변수, 스위치
        for(int i=0; i<theDatabase.var_name.Length; i++) 
        {
            data.varNameList.Add(theDatabase.var_name[i]);
            data.varNumberList.Add(theDatabase.var[i]); 
        }
        for (int i = 0; i < theDatabase.switch_name.Length; i++) 
        {
            data.swNameList.Add(theDatabase.switch_name[i]);
            data.swList.Add(theDatabase.switches[i]);
        }

        List<Item> itemList = theInven.SaveItem(); //밑의 for문위한 임시 변수

        //아이템
        for(int i=0; i<itemList.Count; i++) //플레이어가 소지한 개수만큼 반복
        {
            Debug.Log("인벤토리 아이템 저장 완료: "+itemList[i].itemID);
            data.playerItemInventory.Add(itemList[i].itemID); //아이템의 i번째 있는 아이템의 아이디 삽입(아이템 삽입)
            data.playerItemInventoryCount.Add(itemList[i].itemCount); //아이템 개수 삽입
        }

        //장비
        for(int i=0; i<theEquip.equipItemList.Length; i++)
        {
            data.playerEquipItem.Add(theEquip.equipItemList[i].itemID);
            Debug.Log("장비 아이템 저장 완료: " + theEquip.equipItemList[i].itemID);
        }

        //세이브 파일 생성
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.dataPath + "/SaveFile.dat");
        bf.Serialize(file, data);
        file.Close(); 

        Debug.Log(Application.dataPath + "의 위치에 저장했습니다. ");
    }

    public void CallLoad() //로드 함수
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(Application.dataPath + "/SaveFile.dat", FileMode.Open);

        if(file!=null && file.Length>0) //세이브 파일 있을 때만 로드
        {
            data = (Data)bf.Deserialize(file);

            theDatabase = FindObjectOfType<DatabaseManager>();
            thePlayer = FindObjectOfType<PlayerManager>();
            thePlayerStat = FindObjectOfType<PlayerStat>();
            theEquip = FindObjectOfType<Equipment>();
            theInven = FindObjectOfType<Inventory>();
            theFade = FindObjectOfType<FadeManager>();

            //로드시 페이드 효과 적용
            theFade.FadeOut();

            //맵 및 씬
            thePlayer.currentMapName = data.mapName;
            thePlayer.currentSceneName = data.sceneName;

            //플레이어 위치
            vector.Set(data.playerX, data.playerY, data.playerZ); 
            thePlayer.transform.position = vector;

            //플레이어 스탯
            thePlayerStat.character_Lv = data.playerLv;
            thePlayerStat.hp = data.playerHP;
            thePlayerStat.mp = data.playerMP;
            thePlayerStat.currentHP = data.playerCurrentHP;
            thePlayerStat.currentMP = data.playerCurrentMP;
            thePlayerStat.currentEXP = data.playerCurrentEXP;
            thePlayerStat.atk = data.playerATK;
            thePlayerStat.def = data.playerDEF;
            thePlayerStat.recover_hp = data.playerHPR;
            thePlayerStat.recover_mp = data.playerMPR;

            //장비 추가 스탯
            theEquip.added_atk = data.added_atk;
            theEquip.added_def = data.added_def;
            theEquip.added_hpr = data.added_hpr;
            theEquip.added_mpr = data.added_mpr;

            //데이터베이스 스위치 및 변수
            //한번에 꺼내서 배열로 넣어줌
            theDatabase.var = data.varNumberList.ToArray(); 
            theDatabase.var_name = data.varNameList.ToArray();
            theDatabase.switches = data.swList.ToArray();
            theDatabase.switch_name = data.swNameList.ToArray();

            //장비템
            for (int i = 0; i < theEquip.equipItemList.Length; i++) //0부터 11까지 반복함(장비슬롯이 12개이므로)
            {
                for (int x = 0; x < theDatabase.itemList.Count; x++)
                { 
                    if (data.playerEquipItem[i] == theDatabase.itemList[x].itemID)
                    {
                        theEquip.equipItemList[i] = theDatabase.itemList[x];
                        Debug.Log("장착된 아이템을 로드했습니다 : " + theEquip.equipItemList[i].itemID);
                        break; //하나 찾으면 다음단계로 넘어감
                    }
                }
            }

            //인벤토리의 아이템 추가
            List<Item> itemList = new List<Item>();

            for (int i = 0; i < data.playerItemInventory.Count; i++) 
            {
                for (int x = 0; x < theDatabase.itemList.Count; x++) 
                {
                    if (data.playerItemInventory[i] == theDatabase.itemList[x].itemID)
                    {
                        itemList.Add(theDatabase.itemList[x]);
                        Debug.Log("인벤토리 아이템을 로드했습니다 : " + theDatabase.itemList[x].itemID);
                        break;
                    }
                }
            }

            //아이템의 개수 추가
            for (int i = 0; i < data.playerItemInventoryCount.Count; i++)
            {
                itemList[i].itemCount = data.playerItemInventoryCount[i];
            }

            //아이템 및 추가 스탯 적용
            theInven.LoadItem(itemList); 
            theEquip.ShowTxT(); 

            StartCoroutine(WaitCoroutine());

        }
        else //파일 없으면
        {
            Debug.Log("저장된 세이브 파일이 없습니다.");
        }
        file.Close();
    }

    IEnumerator WaitCoroutine() //페이드 효과 후 씬전환 이뤄지도록 대기
    {
        yield return new WaitForSeconds(2f);

        GameManager theGM = FindObjectOfType<GameManager>();
        theGM.LoadStart(); //로드

        //씬 이동
        SceneManager.LoadScene(data.sceneName);
    }
}