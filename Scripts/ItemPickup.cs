using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//아이템 습득했을 때 스크립트
//습득시 오디오 한번 실행, 아이템 획득 함수 호출로 아이템 획득, 오브젝트 파괴
public class ItemPickup : MonoBehaviour
{
    public int itemID; //데이터베이스에 있던 아이템 아이디를 참조해서 검색
    public int _count;
    public string pickUpSound;
    private void OnTriggerStay2D(Collider2D collision)
    {
        if(Input.GetKeyDown(KeyCode.Z))
        {
            AudioManager.instance.Play(pickUpSound);
            Inventory.instance.GetAnItem(itemID, _count);
            Destroy(this.gameObject);
        }
    }
}
