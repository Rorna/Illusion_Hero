using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Equipment : MonoBehaviour
{
    private OrderManager theOrder; //notMove 함수 이용
    private AudioManager theAudio;
    private PlayerStat thePlayerStat; //플레이어 스탯 연동용
    private Inventory theInven; //인벤토리 연동
    private OkOrCancel theOOC; //선택 패널

    //sound
    public string key_sound;
    public string enter_sound;
    public string open_sound;
    public string close_sound;
    public string takeoff_sound;
    public string equip_sound;

    //상수, 보기쉽게 하기위해 선언
    private const int WEAPON = 0, SHILED = 1, AMULT = 2, LEFT_RING = 3, RIGHT_RING = 4,
                      HELMET = 5, ARMOR = 6, LEFT_GLOVE = 7, RIGHT_GLOVE = 8, BELT = 9,
                      LEFT_BOOTS = 10, RIGHT_BOOTS = 11;

    //텍스트 출력시 쓸 상수
    private const int ATK = 0, DEF = 1, HPR = 6, MPR = 7;

    //스탯이 기존에 비해 얼마나 추가되었는지 표시를 위한 변수, atk: 7(+2), 나중에 명중률 이런것 추가
    public int added_atk, added_def, added_hpr, added_mpr; //기존 스탯에서 이놈들을 빼면 원래 스탯이 남음

    public GameObject equipWeapon; //장착 아이템 오브젝트(여기선 칼)


    public GameObject go; //장비창, 활성화 비활성화 목적용
    public GameObject go_OOC; //선택 패널

    public Text[] text; //스탯 텍스트
    public Image[] img_slots; //장비 아이콘들
    public GameObject go_selected_Slot_UI; //선택된 장비 슬롯 UI.

    public Item[] equipItemList; //장착된 장비 리스트.

    private int selectedSlot; //선택된 장비 슬롯

    public bool activated=false;
    private bool inputKey=true;


    // Start is called before the first frame update
    void Start()
    {
        theInven = FindObjectOfType<Inventory>();
        theOrder = FindObjectOfType<OrderManager>();
        theAudio = FindObjectOfType<AudioManager>();
        thePlayerStat = FindObjectOfType<PlayerStat>();
        theOOC = FindObjectOfType<OkOrCancel>();

    }

    public void ShowTxT() //스탯 텍스트 출력함수
    {
        //atk
        if(added_atk==0) //추가된 수치가 없으면 그냥 넣어줌
            text[ATK].text = thePlayerStat.atk.ToString();
        else //있으면 추가
        {
            text[ATK].text = thePlayerStat.atk.ToString()+" (+"+added_atk+")";
        }
        //def
        if (added_def == 0) //추가된 수치가 없으면 그냥 넣어줌
            text[DEF].text = thePlayerStat.def.ToString();
        else //있으면 추가
        {
            text[DEF].text = thePlayerStat.def.ToString() + " (+" + added_def + ")";
        }
        //hpr
        if (added_hpr == 0) //추가된 수치가 없으면 그냥 넣어줌
            text[HPR].text = thePlayerStat.recover_hp.ToString();
        else //있으면 추가
        {
            text[HPR].text = thePlayerStat.recover_hp.ToString() + " (+" + added_hpr + ")";
        }
        //mpr
        if (added_mpr == 0) //추가된 수치가 없으면 그냥 넣어줌
            text[MPR].text = thePlayerStat.recover_mp.ToString();
        else //있으면 추가
        {
            text[MPR].text = thePlayerStat.recover_mp.ToString() + " (+" + added_mpr + ")";
        }
    }

    //////////////////아이템 장비 관련 함수/////////////////////
    public void EquipItem(Item _item) //넘어오는 아이템(_Item)을 장착
    {
        //넘어온 아이템 아이디를 스트링 변환하여 temp에 삽입
        string temp = _item.itemID.ToString();
        temp = temp.Substring(0, 3); //슬라이스 기능(앞글자 0부터 3번째까지 자름), 200001이면 200이 반환됨
        switch (temp) //값에 따라 분류
        {
            case "200": // 무기
                EquipItemCheck(WEAPON, _item);
                equipWeapon.SetActive(true); //아이템 이미지활성화
                equipWeapon.GetComponent<SpriteRenderer>().sprite = _item.itemIcon; //아이템 스프라이트로 설정
                break;
            case "201": // 방패
                EquipItemCheck(SHILED, _item);
                break;
            case "202": // 아뮬렛
                EquipItemCheck(AMULT, _item);
                break;
            case "203": // 반지
                EquipItemCheck(LEFT_RING, _item);
                break;
        }
    }
    public void EquipItemCheck(int _count, Item _item) //장비 어느 항목인지 체크 함수(반지인지 아뮬렛인지 등등), 실제 장착 함수
    {
        //슬롯 확인
        if(equipItemList[_count].itemID==0) //해당 슬롯이 비어있으면(아무것도 장착하지 않았음)
        {
            equipItemList[_count] = _item; //그 자리에 들어온 아이템을 삽입
        }
        else //무언가 장착 되어 있으면
        {
            //기존의 것을 빼야함
            theInven.EquipToInventory(equipItemList[_count]); //기존의 것을 인벤토리로 옮김
            equipItemList[_count] = _item;
        }
        EquipEffect(_item); //아이템 장착 효과 함수 호출
        theAudio.Play(equip_sound);
        ShowTxT(); //장착했으므로 스탯 변동 됨, 그러므로 호출
    }
    //////////////////아이템 장비 관련 함수/////////////////////

    //////////////////ui 관련 함수/////////////////////
    public void SelectedSlot() //무엇이 선택되었는지 보여주는 함수
    {
        //슬롯 UI의 값을 현재 선택된 이미지 슬롯의 위치값으로 변경
        go_selected_Slot_UI.transform.position = img_slots[selectedSlot].transform.position;
    }
    public void ClearEquip() //장비창 초기화 함수
    {
        //장비창 초기화, 싹다 투명하게
        Color color = img_slots[0].color;
        color.a = 0f; //투명
        for(int i=0; i<img_slots.Length; i++)
        {
            img_slots[i].sprite = null;
            img_slots[i].color = color;
        }
    }
    public void ShowEquip() //장비가 있으면 장비 아이콘 삽입하는 함수, 해당 장비를 보여주는 함수
    {
        Color color = img_slots[0].color;
        color.a = 1f; //보이게
        for (int i = 0; i < img_slots.Length; i++)
        {
            if(equipItemList[i].itemID!=0) //무언가 들어있다면
            {
                img_slots[i].sprite = equipItemList[i].itemIcon; //아이콘 삽입 
                img_slots[i].color = color; //보이게
            }
        }
    }    

    private void EquipEffect(Item _item) //장착 효과 함수
    {
        thePlayerStat.atk += _item.atk;
        thePlayerStat.def += _item.def;
        thePlayerStat.recover_hp += _item.recover_hp;
        thePlayerStat.recover_mp += _item.recover_mp;

        //추가분
        added_atk += _item.atk;
        added_def += _item.def;
        added_hpr += _item.recover_hp;
        added_mpr += _item.recover_mp;
    }

    private void TakeOffEffect(Item _item) //장착 해제(스탯 원래대로)
    {
        thePlayerStat.atk -= _item.atk;
        thePlayerStat.def -= _item.def;
        thePlayerStat.recover_hp -= _item.recover_hp;
        thePlayerStat.recover_mp -= _item.recover_mp;
        //만약 최대 체력 증가 효과였으면 해제시 다시 원래 체력으로 돌아가야함, current_hp=hp; 이런식으로

        //감소분
        added_atk -= _item.atk;
        added_def -= _item.def;
        added_hpr -= _item.recover_hp;
        added_mpr -= _item.recover_mp;
    }

    void Update() //키 입력 관련
    {
        if(inputKey) //키입력 활성화
        {
            if(Input.GetKeyDown(KeyCode.E))
            {
                activated = !activated;

                if(activated) //활성화
                {
                    theOrder.NotMove(); //움직이지 못하게 설정
                    theAudio.Play(open_sound);
                    go.SetActive(true); //장비창 활성화
                    selectedSlot = 0; //처음 선택값은 첫번째값으로
                    SelectedSlot();
                    ClearEquip(); //장비창 초기화
                    ShowEquip(); //장착된게 있으면 장착된 것을 보여줌
                    ShowTxT(); //스탯 텍스트 반영
                }
                else
                {
                    theOrder.Move(); //움직이게 설정
                    theAudio.Play(close_sound);
                    go.SetActive(false); //장비창 비활성화
                    ClearEquip(); //장비창 초기화
                }

            }

            if(activated) //장비창 내 이동
            {
                if(Input.GetKeyDown(KeyCode.UpArrow)) //레프트와 기능이 같음
                {
                    //0
                    //1
                    //2 순서임
                    if (selectedSlot > img_slots.Length - 1)
                        selectedSlot--;
                    else
                        selectedSlot = img_slots.Length-1; //맨 밑으로 이동
                    theAudio.Play(key_sound);
                    SelectedSlot();
                }
                else if(Input.GetKeyDown(KeyCode.DownArrow)) //라이트와 기능 같음
                {
                    //길이보다작으면 아래로 내려감
                    if (selectedSlot < img_slots.Length - 1)
                        selectedSlot++;
                    else //같으면 다시 초기 위치로(맨 위로)
                        selectedSlot = 0;
                    theAudio.Play(key_sound);
                    SelectedSlot();
                }
                else if (Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    if (selectedSlot > 0)
                        selectedSlot--;
                    else
                        selectedSlot = img_slots.Length - 1; //맨 오른쪽으로 이동
                    theAudio.Play(key_sound);
                    SelectedSlot();
                }
                else if (Input.GetKeyDown(KeyCode.RightArrow))
                {
                    if (selectedSlot < img_slots.Length - 1)
                        selectedSlot++;
                    else
                        selectedSlot = 0; //맨 왼쪽 이동
                    theAudio.Play(key_sound);
                    SelectedSlot();
                }
                else if (Input.GetKeyDown(KeyCode.Z)) //장착/해제 키
                {
                    //빈 슬롯에 벗기 호출 되는 것 방지
                    if (equipItemList[selectedSlot].itemID != 0) //아이템id 값이 0이 아닐때에만 실행, 즉 무언가 장착이 되어있을 때에만 패널 실행
                    {
                        theAudio.Play(enter_sound);
                        inputKey = false;
                        StartCoroutine(OOCCoroutine("벗기", "취소")); //코루틴 호출
                    }
                }
            }
        }
    }

    IEnumerator OOCCoroutine(string _up, string _down) //선택지 코루틴
    {
        go_OOC.SetActive(true);
        theOOC.ShowTwoChoice(_up, _down);

        //플레이어가 선택할때까지 대기
        yield return new WaitUntil(() => !theOOC.activated); //theOOC.activated가 false가 될때까지 대기, false가 되면 대기 해제

        //플레이어가 '해제'를 선택하면, 장비를 꺼내서 인벤토리에 넣음
        //그러나 inventoryItemList가 private이므로 Inventory 스크립트에 EquipToInventory(Item _item) 함수를 이용하여 삽입
        if (theOOC.GetResult()) //플레이어가 '해제'를 선택하면, 장비를 꺼내서 인벤토리에 넣음
        {
            //현재 착용하고 있는 아이템 슬롯을 인벤토리에 삽입
            theInven.EquipToInventory(equipItemList[selectedSlot]);
            TakeOffEffect(equipItemList[selectedSlot]); //해제 효과 함수 호출
            if (selectedSlot == WEAPON) //벗은 장비가 무기일경우
                equipWeapon.SetActive(false); //무기 오브젝트 비활성화
            ShowTxT(); //해제함수 호출로 스탯 변동 되었으니 텍스트 호출
            //현재 착용하고 있는 장비템 없애야함
            equipItemList[selectedSlot] = new Item(0, "", "", Item.ItemType.Equip); //빈껍데기를 삽입
            theAudio.Play(takeoff_sound);

            //다시 변경사항이 생겼으니 장비창 초기화
            ClearEquip();
            ShowEquip(); //다시 장비창 보여줌
           
        }
        inputKey = true; //방향키 입력이 가능해지도록 다시 true
        go_OOC.SetActive(false);
    }
}

