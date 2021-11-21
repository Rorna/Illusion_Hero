using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; //이미지 가져다 쓸거임


//적의 스탯을 관리하는 스크립트
public class EnemyStat : MonoBehaviour
{
    public int hp;
    public int currentHp;
    public int atk;
    public int def;
    public int exp;


    //체력바 제어
    public GameObject healthBarBackground;
    public Image healthBarFilled;

    void Start()
    {
        currentHp = hp;
        healthBarFilled.fillAmount = 1f; //기본값을 꽉채운 상태로 초기화
    }

    public int Hit(int _playerAtk) //피격함수, 반환값 이용해 플로팅텍스트 띄울거임
    {
        int playerAtk = _playerAtk;
        int dmg;
        if (def >= playerAtk) dmg = 1; //적의 공격력이 플레이어의 공격력보다 크면 데미지 1들어감
        else dmg = playerAtk - def; //아니면 플레이어의 공격력에서 방어력 깐만큼 데미지

        currentHp -= dmg;
        if(currentHp<=0)
        {
            Destroy(this.gameObject);
            PlayerStat.instance.currentEXP += exp; //경험치 추가
        }

        healthBarFilled.fillAmount = (float)currentHp / hp; //0과 1사이 값으로 표현하기위해
        healthBarBackground.SetActive(true); //초기 비활성화 되어있으므로 활성화

        StopAllCoroutines(); //그전에 돌고있는 체력바 비활성화 코루틴 해제
        StartCoroutine(WaitCoroutine()); //체력바 비활성화 코루틴
        return dmg;
    }

    IEnumerator WaitCoroutine() //정해진 시간동안 공격이 없으면 체력바 비활성화 하는 코루틴
    {
        yield return new WaitForSeconds(3f);
        healthBarBackground.SetActive(false);
    }

}
