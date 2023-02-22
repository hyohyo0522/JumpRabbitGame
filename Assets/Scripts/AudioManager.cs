using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


[System.Serializable] // 인스펙터 창에서 수정할 수 있게 해주는 기능
public class Sound
{
    public string name;
    public AudioClip clip; // 실제 Mp3 파일 및 웨이브 파일을 여기에 넣어주면 된다. 
}


public class AudioManager : MonoBehaviour
{
    public AudioMixer audioMixer;
    public Slider BgmSlider;
    public Slider SfxSlider;
    public string startBgm;

    private void Start()
    {
        PlayBGM(startBgm);

    }

    public static AudioManager instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = FindObjectOfType<AudioManager>();
            }

            return m_instance;
        }
    }

    private static AudioManager m_instance; // 싱글톤이 할당될 변수

    [SerializeField] Sound[] sfx = null;
    [SerializeField] Sound[] bgm = null;

    [SerializeField] AudioSource bgmPlayer = null; // 배경은은 하나이므로 배열로 만들지x
    [SerializeField] AudioSource[] sfxPlayer = null;  // 효과음은 여러개이므로 배열로 만든다!

    public void PlayBGM(string p_bgmName)
    {
        for(int i =0; i < bgm.Length; i++)
        {
            if(p_bgmName == bgm[i].name)
            {
                bgmPlayer.clip = bgm[i].clip;
                bgmPlayer.Play();
            }
        }
    }

    public void StopBGM()
    {
        bgmPlayer.Stop();
    }

    public void PlaySFX(string p_sfxmName)
    {
        for (int i = 0; i < sfx.Length; i++)
        {
            if (p_sfxmName == sfx[i].name)
            {
                // Debug.Log("오디오 클립을 찾았다 : " + sfx[i].name);
                for(int x = 0; x < sfxPlayer.Length; x++)
                {
                    if (!sfxPlayer[x].isPlaying)
                    {
                        // Debug.Log("재생중이지 않은 오디오를 찾았다! : " + p_sfxmName);
                        //sfx[x].clip = sfx[i].clip;
                        sfxPlayer[x].PlayOneShot(sfx[i].clip);
                        //Debug.Log("플레이하였다 " + sfx[i].name);
                        return;
                    }
                }

                //Debug.Log("모든 오디오 플레이어가 재생중입니다.");
                return;
            }
        }

        //Debug.Log(p_sfxmName + "이름의 효과음이 없습니다.");
    }


    public void SetBgmVolume()
    {
        ////로그연산값 전달
        audioMixer.SetFloat("BGM", Mathf.Log10(BgmSlider.value) * 20);

        //audioMixer.SetFloat("BGM", BgmSlider.value);
    }

    public void SetSfxVolume()
    {
        ////로그연산값 전달
        audioMixer.SetFloat("SFX", Mathf.Log10(SfxSlider.value) * 20);
        //audioMixer.SetFloat("SFX",SfxSlider.value);
    }

}
