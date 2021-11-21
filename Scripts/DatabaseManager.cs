using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//데이터베이스 관리 스크립트(싱글턴)

public class DatabaseManager : MonoBehaviour
{
    static public DatabaseManager instance; //싱글턴화

    private PlayerStat thePlayerStat; //스탯관리변수

    public GameObject prefabs_floating_Text; //플로팅 텍스트
    public GameObject parent; //캔버스

    private WaitForSeconds waitTime = new WaitForSeconds(0.01f);
    private void Awake()
    {
        if (instance == null)
        {
            DontDestroyOnLoad(this.gameObject);
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
            Debug.Log("재생성");
        }
    }

    public string[] var_name;
    public float[] var; //float값 기억시키는 배열 변수

    public string[] switch_name;
    public bool[] switches; //true, false값을 기억시키는 배열 변수

    //아이템 리스트 추가
    public List<Item> itemList = new List<Item>();

    private void FloatingText(int number, string color) //플로팅 텍스트 호출 함수, 매개변수는 수치와 색상
    {
        //플로팅 메시지 띄울 위치
        Vector3 vector = thePlayerStat.transform.position;
        vector.y += 60; //캐릭터 머리위에 띄우도록 설정

        GameObject clone = Instantiate(prefabs_floating_Text, vector, Quaternion.Euler(Vector3.zero));
        clone.GetComponent<FloatingText>().text.text = number.ToString(); //데미지 숫자
        if (color == "GREEN")
            clone.GetComponent<FloatingText>().text.color = Color.green; //텍스트 green로 바꿈
        else if (color == "BLUE")
            clone.GetComponent<FloatingText>().text.color = Color.blue; //텍스트 BLUE로 바꿈

        clone.GetComponent<FloatingText>().text.fontSize = 25; //텍스트 크기
        clone.transform.SetParent(parent.transform); 
    }
    public void UseItem(int _itemID) //아이템 효과, 뭐 포션 마셨을 때의 처리 등등
    {
        switch(_itemID)
        {
            
            case 10001: //붉은 포션
                if (thePlayerStat.hp >= thePlayerStat.currentHP + 50) //전체 hp와 물약먹은 현재 hp비교
                    thePlayerStat.currentHP += 50;
                else thePlayerStat.currentHP = thePlayerStat.hp;
                FloatingText(50, "GREEN");
                break;
            case 10002: //파란 포션
                if (thePlayerStat.mp >= thePlayerStat.currentMP + 15) //전체 mp와 물약먹은 현재 mp비교
                    thePlayerStat.currentMP += 15;
                else thePlayerStat.currentMP = thePlayerStat.mp;
                FloatingText(50, "BLUE");
                break;

        }
    }

    void Start()
    {
        thePlayerStat = FindObjectOfType<PlayerStat>(); //플레이어스탯 초기화
        itemList.Add(new Item(10001, "빨간 포션", "체력을 50 채워주는 마법의 물약", Item.ItemType.Use));
        itemList.Add(new Item(10002, "파란 포션", "마나를 15 채워주는 마법의 물약", Item.ItemType.Use));
        itemList.Add(new Item(10003, "농축 빨간 포션", "체력을 350 채워주는 마법의 물약", Item.ItemType.Use));
        itemList.Add(new Item(10004, "농축 파란 포션", "마나를 80 채워주는 마법의 물약", Item.ItemType.Use));
        itemList.Add(new Item(11001, "랜덤 상자", "랜덤으로 포션이 나온다. 낮은 확률로 꽝", Item.ItemType.Use));
        itemList.Add(new Item(20001, "짧은 검", "기본적인 용사의 검", Item.ItemType.Equip, 3));
        itemList.Add(new Item(20301, "사파이어 반지", "1분에 마나 1을 회복시켜주는 마법 반지", Item.ItemType.Equip, 0, 0, 1)); //초당 1회복
        //itemList.Add(new Item(20002, "무딘 도끼", "1분에 마나 1을 회복시켜주는 마법 반지", Item.ItemType.Equip, 0, 0, 1)); 
        itemList.Add(new Item(30001, "고대 유물의 조각 1", "반으로 쪼개진 고대 유물의 파편", Item.ItemType.Quest));
        itemList.Add(new Item(30002, "고대 유물의 조각 2", "반으로 쪼개진 고대 유물의 파편", Item.ItemType.Quest));
        itemList.Add(new Item(30003, "고대 유물", "고대 유적에 잠들어있던 고대의 유물", Item.ItemType.Quest));
    }
}
