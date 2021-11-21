using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//EnemyStat 스크립트의 Hit함수 호출 스크립트
//이 스크립트는 플레이어의 공격시 나오는 스프라이트에 붙여넣음
//이 스크립트의 역할은 적을 공격하면 플로팅 텍스트도 띄움
public class HurtEnemy : MonoBehaviour
{
    public GameObject prefabs_Floating_Text;
    public GameObject parent;
    public GameObject effect;

    public string atkSound;

    private PlayerStat thePlayerStat;

    // Start is called before the first frame update
    void Start()
    {
        thePlayerStat = FindObjectOfType<PlayerStat>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag=="enemy") //충돌한게 적이면
        {
            //부딪힌 객체의 컴포넌트 가져옴
            //dmg=플레이어가 적을 공격했을 때 입힌 데미지
            int dmg = collision.gameObject.GetComponent<EnemyStat>().Hit(thePlayerStat.atk); //hit함수 호출
            AudioManager.instance.Play(atkSound);

            //플로팅 메시지 띄울 위치
            Vector3 vector = collision.transform.position; //충돌한 놈의 위치값

            //effect 프리팹 생성
            Instantiate(effect, vector, Quaternion.Euler(Vector3.zero));
            vector.y += 60; //캐릭터 머리위에 띄우도록 설정

            GameObject clone = Instantiate(prefabs_Floating_Text, vector, Quaternion.Euler(Vector3.zero));
            clone.GetComponent<FloatingText>().text.text = dmg.ToString(); //데미지 숫자
            clone.GetComponent<FloatingText>().text.color = Color.white;
            clone.GetComponent<FloatingText>().text.fontSize = 25;
            clone.transform.SetParent(parent.transform); 


        }
    }
}
