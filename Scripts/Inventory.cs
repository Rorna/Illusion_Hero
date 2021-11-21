using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//인벤토리 관리 스크립트
public class Inventory : MonoBehaviour
{
    public static Inventory instance;

    private DatabaseManager theDatabase; //database 접근용 변수
    private OrderManager theOrder;
    private AudioManager theAudio;
    private OkOrCancel theOOC;
    private Equipment theEquip; //장비 함수 호출용

    public string key_sound;
    public string enter_sound;
    public string cancel_sound;
    public string open_sound; //인벤토리 실행시 소리
    public string beep_sound; //부적절한 사용시, 오류, 잘못된 선택 하려할때의 소리

    private InventorySlot[] slots; //인벤토리 슬롯들(보여지는 것)
    private List<Item> inventoryItemList; //플레이어가 '실제로' 소지한 아이템 리스트

    private List<Item> inventoryTabList; 

    public Text Description_Text; //부연 설명, 탭의 설명이나 아이템의 설명
    public string[] tabDescription; //탭 부연 설명

    public Transform tf; //slot의 부모객체(Grid Slot)를 가져다 씀, 부모객체를 이용하여 Grid Slot의 자식 객체를 slots에 넣을거임

    public GameObject go; //인벤토리 활성화 비활성화시 필요한 변수
    public GameObject[] selectedTabImages; //selected_Tab을 넣을 거임
    public GameObject go_OOC; //선택창 활성화 비활성화
    public GameObject prefab_floating_Text; //프리팹 넣을 변수

    private int selectedItem; //선택된 아이템, 어떤 아이템을 선택했는지 변수로 확인
    private int selectedTab; //선택된 탭

    private int page; //페이지
    private int slotCount; //활성화된 슬롯의 개수
    private const int MAX_SLOTS_COUNT = 12; //최대 슬롯 개수, 1페이지당 슬롯의 개수가 12개


    private bool activated; //인벤토리 활성화시 true
    private bool tabActivated; //탭 활성화시 true(탭 고를때 아이템이 넘어가지 않게 방지)
    private bool itemActivated; //아이템 활성화시 true(아이템 고를때 탭이 넘어가지 않게 방지)
    private bool stopKeyInput; //키입력 제한, 아이템 사용(yes/no)에서 선택지 고를 때, 아이템이 움직여지는 것 방지
    private bool preventExec; //중복실행 제한

    private WaitForSeconds waitTime = new WaitForSeconds(0.01f); //메모리 효과적 사용, 미리 생성해서 이것을 사용

    void Start()
    {
        instance = this;

        theAudio = FindObjectOfType<AudioManager>();
        theOrder = FindObjectOfType<OrderManager>();
        theDatabase = FindObjectOfType<DatabaseManager>();
        theOOC = FindObjectOfType<OkOrCancel>();
        theEquip = FindObjectOfType<Equipment>();

        inventoryItemList = new List<Item>();
        inventoryTabList = new List<Item>();
        slots = tf.GetComponentsInChildren<InventorySlot>(); //tf의 자식개체들이 slots안에 들어가게 됨
    }

    public List<Item> SaveItem() //세이브시 아이템인벤토리리스트를 반환(프라이빗으로 설정되었기때문에 함수로 값 전달)
    {
        return inventoryItemList; //소지한 아이템 값 전달
    }

    public void LoadItem(List<Item> _itemList) //인벤토리 로드 함수
    {
        inventoryItemList = _itemList; //세이브 하기 전에 저장해두었던 데이터를 덮어씌움
    }

    //장비에 있던 아이템을 인벤토리(inventoryItemList)에 추가하는 함수, Equipment 스크립트에서 사용
    public void EquipToInventory(Item _item)
    {
        inventoryItemList.Add(_item);
    }

    public void GetAnItem(int _itemID, int _count=1) //아이템 획득 함수, 여러개 얻을 수 있지만 기본값은 1로 설정
    {
        for(int i=0; i<theDatabase.itemList.Count; i++) //데이터베이스에 해당 아이템이 있는지 검색
        {
            if(_itemID == theDatabase.itemList[i].itemID) //데이터베이스의 아이템 아이디와 일치하면 (있다면)
            {
                //플로팅 텍스트 생성
                var clone = Instantiate(prefab_floating_Text, PlayerManager.instance.transform.position, Quaternion.Euler(Vector3.zero));
                //FloatingText 컴포넌트의 텍스트 컴포넌트의 텍스트를 (아이템 이름 _count개 획득)으로 바꿔줌
                clone.GetComponent<FloatingText>().text.text=theDatabase.itemList[i].itemName+" "+_count+"개 획득 +";
                clone.transform.SetParent(this.transform); //clone은 inventory의 자식개체로 생성될거임, canvas 밖에서 생성되면 안보임

                for(int j=0; j<inventoryItemList.Count; j++) //소지품에 같은 아이템이 있는지 검색
                {
                    if(inventoryItemList[j].itemID == _itemID) 
                    {
                        if(inventoryItemList[j].itemType==Item.ItemType.Use) 
                        {
                            inventoryItemList[j].itemCount += _count; 
                            return;
                        }
                        else //소모품을 제외한 나머지 타입이라면
                        {
                            //해당 아이템을 처음 획득하는 거라면
                            inventoryItemList.Add(theDatabase.itemList[i]); //슬롯이 하나 더 늘어남   
                        }
                        return;
                    }
                }
                //해당 아이템을 처음 획득하는 거라면 하나 추가
                inventoryItemList.Add(theDatabase.itemList[i]); //슬롯이 하나 더 늘어남
                //몇개를 얻었는지 추가(배열 마지막 인덱스의 아이템 카운트를 파라미터로 넘어온 카운터만큼 증가시킴(_count가 2면 두개 증가)
                inventoryItemList[inventoryItemList.Count - 1].itemCount = _count;
                return;
            }
        }
        //데이터베이스에 없는 아이템이면
        Debug.LogError("데이터베이스에 해당 ID값을 가진 아이템이 존재하지 않습니다.");
    }

    //인벤토리 슬롯 초기화 함수
    public void RemoveSlot() //초기 slot 삭제(탭 선택구간이므로 slot이 초기에 있을 필요 없음)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].RemoveItem(); //슬롯 삭제
            slots[i].gameObject.SetActive(false); //비활성화
        }
    }

    //-----탭 관련 함수
    public void ShowTab() //탭 활성화 함수
    {
        RemoveSlot();
        SelectedTab();
    }
    public void SelectedTab() //선택된 탭 나타내는 함수, 선택된 탭을 제외하고 다른 모든 탭의 컬러 알파값을 0으로 조정
    {
        StopAllCoroutines();
        //선택된것이 빛나도록
        Color color = selectedTabImages[selectedTab].GetComponent<Image>().color; //객체의 Image컴포넌트를 가져온후 그 컴포넌트 내의 color 제어
        color.a = 0f; 

       
        for(int i=0; i<selectedTabImages.Length; i++)
        {
            selectedTabImages[i].GetComponent<Image>().color = color;
        }
        
        Description_Text.text = tabDescription[selectedTab];
        StartCoroutine(SelectedTabEffectCoroutine());
    }
    IEnumerator SelectedTabEffectCoroutine() //선택된 탭 활성화 효과(반짝반짝)
    {
        //탭 선택할 수 있는 선택창이 활성화 되면 아이템창으로 넘어가기 전까지 계속 반짝거리게 함
        while(tabActivated)
        {
            Color color = selectedTabImages[0].GetComponent<Image>().color;

            while(color.a<0.5f) //색 진하게
            {
                color.a += 0.03f;
                //선택된 탭만 반짝거리도록
                selectedTabImages[selectedTab].GetComponent<Image>().color = color;
                yield return waitTime;
            }
            while (color.a >0f) //다시 옅어지게
            {
                color.a -= 0.03f;
                //선택된 탭만 반짝거리도록
                selectedTabImages[selectedTab].GetComponent<Image>().color = color;
                yield return waitTime;
            }

            yield return new WaitForSeconds(0.3f);
        }
    }
    //-----탭 관련 함수

    //-----아이템 관련 함수

    public void ShowItem() //아이템 활성화(inventoryTabList에 조건에 맞는 아이템들만 넣어주고, 인벤토리 슬롯에 출력)
    {
        //소모품 선택했다 장비 선택했을 때 중복으로 나오는 것 방지를 위해 싹 한번 초기화 시킴
        inventoryTabList.Clear();
        RemoveSlot(); //슬롯까지 제거해줘야 완전한 제거가 됨
        selectedItem = 0; //처음은 무조건 0번째가 선택되도록
        page = 0; //아이템창 열때마다 첫페이지가 위치하도록 설정

        //다시 새롭게 추가
        //탭에 따른 아이템 분류, 그것을 인벤토리 탭 리스트에 추가
        switch(selectedTab) //탭에 따라 각각 다른 아이템들이 들어가도록 관리
        {
            case 0: //소모품
                //플레이어가 소지한 모든 인벤토리 아이템 검사
                for(int i=0; i<inventoryItemList.Count; i++)
                {

                    if (inventoryItemList[i].itemType == Item.ItemType.Use) //inventoryItemList의 값의 타입이 소모품(use)일 경우
                        inventoryTabList.Add(inventoryItemList[i]); //inventoryItemListr의 값을 탭 리스트에 넣음
                }
                //반복문을 다 돌고 나면 소모품 타입의 아이템들이 tabList에 들어가게 됨
                break;
            case 1: //장비 
                for (int i = 0; i < inventoryItemList.Count; i++)
                {
                    if (inventoryItemList[i].itemType == Item.ItemType.Equip) 
                        inventoryTabList.Add(inventoryItemList[i]); 
                }
                break;
            case 2: //퀘스트
                for (int i = 0; i < inventoryItemList.Count; i++)
                {
                    if (inventoryItemList[i].itemType == Item.ItemType.Quest)
                        inventoryTabList.Add(inventoryItemList[i]);
                }
                break;
            case 3: //기타
                for (int i = 0; i < inventoryItemList.Count; i++)
                {
                    if (inventoryItemList[i].itemType == Item.ItemType.ETC)
                        inventoryTabList.Add(inventoryItemList[i]); 
                }
                break;
        }

        ShowPage();

        SelectedItem(); //선택된것만 반짝반짝 효과
    }

    public void ShowPage() //1페이지당 12개 아이템만 들어가도록 하는 함수
    {
        slotCount = -1; //아무것도 없음을 의미

        //인벤토리 탭 리스트의 내용들을 각각 인벤토리 슬롯에 넣어줌

        for (int i = page * MAX_SLOTS_COUNT; i < inventoryTabList.Count; i++)
        {
            //slot 배열은 인벤토리에서 표시되는 아이템들을 표시하기 위한 배열임(그것말고 기능 없). 12까지밖에 없음(1페이지당 12개아이템) 
            slotCount = i - (page * MAX_SLOTS_COUNT); //슬롯 배열엔 13번째나 23번째가 없으므로 빼줘야함
            slots[slotCount].gameObject.SetActive(true); //15면 3번째 슬롯에 아이템이 들어가게됨
            slots[slotCount].Additem(inventoryTabList[i]);

            //1페이지에 아이템이 전부 채워졌다면 종료
            if (slotCount == MAX_SLOTS_COUNT - 1) //11이면 종료. 이게 없으면 inventoryTabList.Count 값 도달할때 까지 계속 반복문 돔
                break;
        }
    }

    public void SelectedItem() //선택되었음을 나타내는 함수, 선택된 아이템을 제외하고 다른 모든 아이템의 컬러 알파값을 0으로 조정
    {
        StopAllCoroutines();
        if (slotCount> -1) //0은 하나가 있다는 뜻, 그러므로 -1보다 크게 하여 0도 포함시켜야 이 함수가 활성화됨
        {
            Color color = slots[0].selected_Item.GetComponent<Image>().color; //color 초기화
            color.a = 0f; //color의 값을 0으로 만듬

            for (int i = 0; i <= slotCount; i++) //슬롯의 개수만큼 반복
                slots[i].selected_Item.GetComponent<Image>().color = color; //다른 항목으로 이동시 반짝거리는 잔상이 남아있는데, 그걸 0으로 초기화(잔상 없앰)

            Description_Text.text = inventoryTabList[selectedItem].itemDescription; //선택된 아이템의 부연 설명표시
            StartCoroutine(SelectedItemEffectCoroutine()); //반짝임 효과
        }
        else //해당 타입의 아이템이 없을 경우
            Description_Text.text = "해당 타입의 아이템을 소유하고 있지 않습니다.";
    }
    IEnumerator SelectedItemEffectCoroutine() //선택된 아이템 반짝임 효과
    {
        //탭 선택할 수 있는 선택창이 활성화 되면 아이템창으로 넘어가기 전깢 계속 반짝거리게 함
        while (itemActivated)
        {
            Color color = slots[0].GetComponent<Image>().color;

            while (color.a < 0.5f) //색 진하게
            {
                color.a += 0.03f;
                //선택된 탭만 반짝거리도록
                slots[selectedItem].selected_Item.GetComponent<Image>().color = color;
                yield return waitTime;
            }
            while (color.a > 0f) //다시 옅어지게
            {
                color.a -= 0.03f;
                //선택된 탭만 반짝거리도록
                slots[selectedItem].selected_Item.GetComponent<Image>().color = color;
                yield return waitTime;
            }

            yield return new WaitForSeconds(0.3f);
        }
    }
    //-----아이템 관련 함수


    // Update is called once per frame
    void Update()
    {
        //키입력 처리
        if(!stopKeyInput)
        {
            if(Input.GetKeyDown(KeyCode.I)) //i키 누르면 인벤토리 활성화
            {
                activated = !activated;
                if(activated)
                {
                    theAudio.Play(open_sound);
                    theOrder.NotMove();
                    go.SetActive(true); //인벤토리창 활성화
                    selectedTab = 0; //소모품
                    tabActivated = true; //먼저 탭 볼수 있도록 함
                    itemActivated = false;
                    ShowTab();

                }
                else //인벤토리 사라짐
                {
                    theAudio.Play(cancel_sound);
                    StopAllCoroutines();
                    //비활성화
                    go.SetActive(false);
                    tabActivated = false;
                    itemActivated = false;

                    theOrder.Move();
                }
            }
            //인벤토리 활성화
            //방향키 이동(왼쪽 오른쪽키)
            if(activated) //인벤토리 활성화 되어있을 경우
            {
                if(tabActivated) //탭이 활성화 되어있는 경우 키 입력 처리
                {
                    if(Input.GetKeyDown(KeyCode.RightArrow))
                    {
                        if (selectedTab < selectedTabImages.Length - 1)
                            selectedTab++;
                        else //맨끝에 도달하면
                            selectedTab = 0;
                        theAudio.Play(key_sound);
                        SelectedTab(); //위치가 바뀌었으니 해당위치에 다시 이 함수 호출을 통해 반짝 효과 줌
                    }
                    else if (Input.GetKeyDown(KeyCode.LeftArrow))
                    {
                        if (selectedTab > 0)
                            selectedTab--;
                        else //맨끝에 도달하면
                            selectedTab = selectedTabImages.Length - 1;
                        theAudio.Play(key_sound);
                        SelectedTab(); 
                    }
                    else if(Input.GetKeyDown(KeyCode.Z)) //선택시 아래 슬롯들이 활성화 되어야함
                    {
                        theAudio.Play(enter_sound);
                        //선택된 탭은 짙게 표현
                        Color color = selectedTabImages[selectedTab].GetComponent<Image>().color;
                        color.a = 0.25f;
                        selectedTabImages[selectedTab].GetComponent<Image>().color = color;

                        itemActivated = true;
                        tabActivated = false;
                        preventExec = true; //탭선택과 아이템 선택 모두 Z키를 공유하고 있으므로 중복선택 방지처리 활성화

                        ShowItem();
                    }

                }

                else if(itemActivated) //아이템 활성화 시 키입력 처리
                {
                    if(inventoryTabList.Count>0) //0이상일 경우에만 방향키 입력이 이루어짐
                    {
                        
                        if (Input.GetKeyDown(KeyCode.DownArrow))
                        {
                            //페이지 이동
                            if (selectedItem+2>slotCount) //미리 더해서 다음 페이지를 넘는지 확인
                            {
                                //넘었으면
                                if (page < (inventoryTabList.Count - 1) / MAX_SLOTS_COUNT) //현재 페이지가 최대페이지보다 작으면
                                    page++; //페이지 증가
                                else //최대 페이지에 도달했으면
                                    page = 0; //최대페이지에 도달하면 다시 0으로 설정

                                RemoveSlot(); //페이지를 바꿨으니 기존의 것들은 삭제해야하므로 함수 호출
                                ShowPage(); //새 페이지 불러옴
                                selectedItem = -2; //페이지 이동시 가장 처음으로 위치하게 됨

                            }
                            // 1 2
                            // 3 4 슬롯의 순서가 이런식으로 가므로 +=2 해줘야함
                            //아래 버튼 눌렀을 경우 +2씩 증가하므로 -2
                            if (selectedItem < slotCount - 1) selectedItem += 2; //여기서 다시 0이 되므로 맨 처음 아이템을 선택하게 됨
                            else //배열의 크기와 같다면
                            {
                                // 1->3, 2->4 이런식으로 이동
                                selectedItem %= 2; //2로 나눈 나머지를 저장, 짝수면 0번째, 홀수면 1번째
                            }
                            theAudio.Play(key_sound);
                            SelectedItem(); //선택된 항목 반짝이게 함수 호출

                        }
                        else if (Input.GetKeyDown(KeyCode.UpArrow))
                        {
                            //페이지 이동
                            //위방향키는 -2씩 감소하므로 -2
                            if (selectedItem -2 <0) //미리 더해서 다음 페이지를 넘는지 확인
                            {
                                if (page != 0) //페이지가 0이 아니라면
                                    page--; //페이지 감소
                                else //제일 첫번째 페이지에 도달했으면
                                    page = (inventoryTabList.Count - 1) / MAX_SLOTS_COUNT; //맨뒤, 최대 페이지로 이동

                                RemoveSlot(); //페이지를 바꿨으니 기존의 것들은 삭제해야하므로 함수 호출
                                ShowPage(); //새 페이지 불러옴
                            }

                            if (selectedItem > 1) selectedItem -= 2;
                            else //1보다 작다면
                            {
                                selectedItem = slotCount - selectedItem;
                            }
                            theAudio.Play(key_sound);
                            SelectedItem(); //선택된 항목 반짝이게 함수 호출
                        }
                        else if (Input.GetKeyDown(KeyCode.RightArrow))
                        {
                            //페이지 이동
                            //오른쪽 이동은 1씩 이동하므로 +1
                            if (selectedItem + 1 > slotCount) //미리 더해서 다음 페이지를 넘는지 확인
                            {
                                //넘었으면
                                if (page < (inventoryTabList.Count - 1) / MAX_SLOTS_COUNT) //현재 페이지가 최대페이지보다 작으면
                                    page++; //페이지 증가
                                else //최대 페이지에 도달했으면
                                    page = 0; //최대페이지에 도달하면 다시 0으로 설정

                                RemoveSlot(); //페이지를 바꿨으니 기존의 것들은 삭제해야하므로 함수 호출
                                ShowPage(); //새 페이지 불러옴
                                selectedItem = -1; //페이지 이동시 가장 처음으로 위치하게 됨
                            }

                            // 1 2
                            // 3 4 에서 4의 위치에서 누르면 1로 감, 2면 3으로 이동
                            if (selectedItem < slotCount) //슬롯카운트보다 작으면
                                selectedItem++;
                            else selectedItem = 0;
                            theAudio.Play(key_sound);
                            SelectedItem(); //선택된 항목 반짝이게 함수 호출
                        }
                        else if (Input.GetKeyDown(KeyCode.LeftArrow))
                        {
                            //페이지 이동
                            //왼방향키는 -1씩 감소하므로 -1
                            if (selectedItem - 1 < 0) //미리 더해서 다음 페이지를 넘는지 확인
                            {
                                if (page != 0) //페이지가 0이 아니라면
                                    page--; //페이지 감소
                                else //제일 첫번째 페이지에 도달했으면
                                    page = (inventoryTabList.Count - 1) / MAX_SLOTS_COUNT; //맨뒤, 최대 페이지로 이동

                                RemoveSlot(); //페이지를 바꿨으니 기존의 것들은 삭제해야하므로 함수 호출
                                ShowPage(); //새 페이지 불러옴
                                
                            }

                            if (selectedItem > 0)
                                selectedItem--;
                            else //0보다 작으면
                            {
                                // 1 2
                                // 3 4 에서 1의 위치에서 누르면 4로 감, 2면 1로 이동
                                selectedItem = slotCount; //슬롯카운트 만큼 넣어줌 (배열의 크기만큼 넣어줌 -> 이건 아닌듯)
                            }
                            theAudio.Play(key_sound);
                            SelectedItem(); //선택된 항목 반짝이게 함수 호출
                        }
                        else if (Input.GetKeyDown(KeyCode.Z) && !preventExec) //중복 실행 방지
                        {
                            if (selectedTab == 0) //소모품
                            {
                                // 사용할거냐 등의 선택지 호출
                                StartCoroutine(OOCCoroutine("사용", "취소"));
                            }
                            else if (selectedTab == 1) //장비
                            {
                                //장비 장착
                                // 사용할거냐 등의 선택지 호출
                                StartCoroutine(OOCCoroutine("장착", "취소"));

                            }
                            else //퀘스트나 기타 아이템은 사용 못함(비프음 출력)
                            {
                                theAudio.Play(beep_sound);
                            }
                        }
                    }
                    
                    if (Input.GetKeyDown(KeyCode.X)) //취소 버튼
                    {
                        theAudio.Play(cancel_sound);
                        StopAllCoroutines(); //반짝거리는 효과 멈춤
                        itemActivated = false; //아이템 목록들 싹 사라짐
                        tabActivated = true;
                        ShowTab(); //다시 탭이 활성화 됨
                    }
                }
                //중복 실행 방지
                if (Input.GetKeyUp(KeyCode.Z))
                    preventExec = false;
            }
        }
    }

    IEnumerator OOCCoroutine(string _up, string _down) //ok or cancel, 선택지 코루틴
    {
        theAudio.Play(enter_sound);
        stopKeyInput = true; //선택지 창 뜰시를 위해 활성화처리

        go_OOC.SetActive(true);
        theOOC.ShowTwoChoice(_up, _down);

        //플레이어가 선택할때까지 대기
        yield return new WaitUntil(() => !theOOC.activated); //theOOC.activated가 false가 될때까지 대기, false가 되면 대기 해제

        //사용 했을 때 루틴
        if(theOOC.GetResult()) //플레이어가 '사용'을 선택하면
        {
            for(int i=0; i<inventoryItemList.Count; i++) //inventoryItemList의 길이만큼 반복
            {
                if(selectedTab==0) //만약 현재 탭이 소모품 탭에 있으면(즉, 아이템이 소모품이면)
                {
                    //아이템사용
                    theDatabase.UseItem(inventoryItemList[i].itemID); //아이템 사용 효과 함수 호출

                    if (inventoryItemList[i].itemID == inventoryTabList[selectedItem].itemID)
                    {
                        if (inventoryItemList[i].itemCount > 1) //아이템 개수가 2개이상이면
                            inventoryItemList[i].itemCount--; //숫자를 줄여줌
                        else //1개만 가지고 있으면 해당 아이템 삭제
                            inventoryItemList.RemoveAt(i); //i번째의 인벤토리를 제거
                        ShowItem(); //아이템 창 재정렬
                        break; 
                    }
                }
                else if(selectedTab==1) //장비템이면
                {
                    theEquip.EquipItem(inventoryItemList[i]); //인벤토리에 있는 아이템을 장착
                    inventoryItemList.RemoveAt(i);
                    ShowItem();
                    break;
                }
            }
        }
        stopKeyInput = false; 
        go_OOC.SetActive(false);
    }
}
