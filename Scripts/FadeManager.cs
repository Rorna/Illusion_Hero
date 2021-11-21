using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//fade 구현 스크립트

public class FadeManager : MonoBehaviour
{
    public SpriteRenderer white;
    public SpriteRenderer black;
    private Color color; //이미지의 투명도 조절

    //waitforseconds 다수 생성시의 메모리 부하 방지용 변수
    private WaitForSeconds waitTime = new WaitForSeconds(0.01f);
    public void FadeOut(float _speed=0.02f)
    {
        StopAllCoroutines();
        StartCoroutine(FadeOutCoroutine(_speed));
    }

    IEnumerator FadeOutCoroutine(float _speed) //페이드인 함수
    {
        color = black.color; //color 초기화(블랙)

        while(color.a<1f) //color의 알파값이 0, 즉 완전히 사라질때 계속 반복
        {
            color.a += _speed;
            black.color = color;
            yield return waitTime;
        }

    }

    public void FadeIn(float _speed = 0.02f)
    {
        StopAllCoroutines(); //코루틴 중첩으로 인해 꼬이는 사태 방지
        StartCoroutine(FadeInCoroutine(_speed));
    }

    IEnumerator FadeInCoroutine(float _speed)
    {
        color = black.color; //color 초기화(블랙)
        while (color.a > 0f) 
        {
            color.a -= _speed;
            black.color = color;
            yield return waitTime;
        }
    }

    public void Flash(float _speed=0.1f) //반짝반짝 효과 함수
    {
        StopAllCoroutines();
        StartCoroutine(FlashCoroutine(_speed));
    }
    IEnumerator FlashCoroutine(float _speed)
    {
        color = white.color; 

        //밝아졌다가 다시 사라짐
        while (color.a < 1f) 
        {
            color.a += _speed;
            white.color = color;
            yield return waitTime;
        }
        while (color.a > 0f)
        {
            color.a -= _speed;
            white.color = color;
            yield return waitTime;
        }
    }
    public void FlashOut(float _speed = 0.02f) 
    {
        StopAllCoroutines();
        StartCoroutine(FlashOutCoroutine(_speed));
    }
    IEnumerator FlashOutCoroutine(float _speed) //플래시 아웃 함수
    {
        color = white.color; //color 초기화(블랙)

        //밝아졌다가 다시 사라짐
        while (color.a < 1f) 
        {
            color.a += _speed;
            white.color = color;
            yield return waitTime;
        }
    }

    public void FlashIn(float _speed = 0.02f) //플래시 인 함수
    {
        StopAllCoroutines();
        StartCoroutine(FlashInCoroutine(_speed));
    }
    IEnumerator FlashInCoroutine(float _speed) //페이드인 함수
    {
        color = white.color; 
        while (color.a > 0f) 
        {
            color.a -= _speed;
            white.color = color;
            yield return waitTime;
        }
    }
}
