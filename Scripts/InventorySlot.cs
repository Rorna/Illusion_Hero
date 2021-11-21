using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//슬롯 관리 스크립트
//인벤토리 슬롯의 아이템과 개수, 아이템의 아이콘을 바꿔주는 스크립트
public class InventorySlot : MonoBehaviour
{
    public Image icon;
    public Text itemName_Text;
    public Text itemCount_Text;
    public GameObject selected_Item; //패널(반짝거리는 효과를 위해 사용)

    public void Additem(Item _item)
    {
        itemName_Text.text = _item.itemName;
        icon.sprite = _item.itemIcon; //icon의 스프라이트를 매개변수의 itemicon으로 설정
        if(Item.ItemType.Use==_item.itemType) //아이템의 종류가 소모품일 경우에만 뒤의 소지수가 뜨도록 설정
        {
            if (_item.itemCount > 0) //1개라도 있으면
                itemCount_Text.text = "x " + _item.itemCount.ToString(); //int->string
            else //없으면
                itemCount_Text.text = ""; //빈공간
        }
    }

    public void RemoveItem()
    {
        itemName_Text.text = "";
        itemCount_Text.text = ""; //빈 껍데기로 초기화
        icon.sprite = null;
    }
}
