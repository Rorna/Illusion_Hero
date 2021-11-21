using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//날씨 관리 스크립트
public class WeatherManager : MonoBehaviour
{
    static public WeatherManager instance;
    private void Awake()
    {

        if (instance == null)
        {
            DontDestroyOnLoad(this.gameObject); 
            instance = this;
        }   
        else Destroy(this.gameObject);
    }

    private AudioManager theAudio; //날씨 소리 제어
    public ParticleSystem rain; //파티클 시스템 통제용 변수
    public string rain_sound; 

    void Start()
    {      
        theAudio = FindObjectOfType<AudioManager>();
    }

    public void Rain() //비
    {
        theAudio.Play(rain_sound); 
        rain.Play(); 
    }
    public void RainStop() //비끝
    {
        theAudio.Stop(rain_sound); 
        rain.Stop();
    }

    public void RainDrop() //몇방울 내리게 할건지
    {
        rain.Emit(100); //1방울만 내림
    }
}
