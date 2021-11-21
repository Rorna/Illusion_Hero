using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//플레이어 스탯 스크립트, 플레이어에 장착
//공격을 입으면 깜빡거리는 효과 추가

public class PlayerStat : MonoBehaviour
{
    public static PlayerStat instance;

    public int character_Lv;
    public int[] needExp; //필요한 경험치
    public int currentEXP;


    public int hp;
    public int currentHP;
    public int mp;
    public int currentMP;

    public int atk;
    public int def;

    public int recover_hp; //분당 체력 회복
    public int recover_mp; //분당 마나 회복

    public string dmgSound; //피격 사운드

    public float time; //분당 회복 아이템시 필요한 변수, 1초당 회복이면 1
    private float current_time;

    public GameObject prefabs_Floating_text; //플로팅 텍스트(프리팹)
    public GameObject parent; //캔버스, 캔버스안에 플로팅텍스트 띄움

    //체력 마나 바 조절 변수
    public Slider hpSlider;
    public Slider mpSlider;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        currentHP = hp;
        currentMP = mp;
        current_time = time;
    }

    public void Hit(int _enemyAtk)
    {
        int dmg;

        if (def >= _enemyAtk) dmg = 1; //방어력이 적공격력 보다 높으면
        else dmg = _enemyAtk - def; //방어력 만큼 데미지 까임

        currentHP -= dmg;

        if (currentHP <= 0)
        {
            Debug.Log("체력 0미만, 게임오버");
        }


        AudioManager.instance.Play(dmgSound);

        //플로팅 메시지 띄울 위치
        Vector3 vector = this.transform.position;
        vector.y += 60; //캐릭터 머리위에 띄우도록 설정

        GameObject clone = Instantiate(prefabs_Floating_text, vector, Quaternion.Euler(Vector3.zero));
        clone.GetComponent<FloatingText>().text.text = dmg.ToString(); //데미지 숫자
        clone.GetComponent<FloatingText>().text.color = Color.red; 
        clone.GetComponent<FloatingText>().text.fontSize = 25; 
        clone.transform.SetParent(parent.transform); 

        StopAllCoroutines(); //연속피격일때를 위해 모든 코루틴 종료
        StartCoroutine(HitCoroutine());
    }

    IEnumerator HitCoroutine() //깜빡거리는 효과
    {
        Color color = GetComponent<SpriteRenderer>().color;

        color.a = 0f;
        GetComponent<SpriteRenderer>().color = color;
        yield return new WaitForSeconds(0.1f);
        color.a = 1f;
        GetComponent<SpriteRenderer>().color = color;
        yield return new WaitForSeconds(0.1f);
        color.a = 0f;
        GetComponent<SpriteRenderer>().color = color;
        yield return new WaitForSeconds(0.1f);
        color.a = 1f;
        GetComponent<SpriteRenderer>().color = color;
        yield return new WaitForSeconds(0.1f);
        color.a = 0f;
        GetComponent<SpriteRenderer>().color = color;
        yield return new WaitForSeconds(0.1f);
        color.a = 1f; 
        GetComponent<SpriteRenderer>().color = color;

    }
    // Update is called once per frame
    void Update()
    {
        //슬라이더의 최대값 설정
        hpSlider.maxValue = hp; //hpSlider의 최대값을 최대 체력(hp)로 설정 
        mpSlider.maxValue = mp;

        //현재 값을 슬라이더의 값에 반영
        hpSlider.value = currentHP;
        mpSlider.value = currentMP;
        if(currentEXP>=needExp[character_Lv]) //현재경험치가 해당 레벨의 필요경험치보다 많을경우
        {
            character_Lv++; //레벨업
            hp += character_Lv * 2; //특정공식만큼 체력 상승
            mp += character_Lv + 2;

            currentHP = hp; //체력 가득
            currentMP = mp; //마나 가득
            atk++; //공격력 증가
            def++; //방어력 증가
        }

        //시간 감소
        current_time -= Time.deltaTime;
        if(current_time<=0) //시간이 다 지나면
        {
            if(recover_hp>0)
            {
                if (currentHP + recover_hp <= hp) //최대체력보다 적을경우
                    currentHP += recover_hp; //체력 회복
                else
                    currentHP = hp;
            }
            current_time = time; //다시 시간 초기화
        }
    }
}
