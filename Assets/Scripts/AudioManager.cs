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


    [Multiline] public string BelowTwoListDescription;
    public string[] SceneNamesList;
    public string[] BgmNameList;


    private void Start()
    {
        //시작시 이전 볼륨양 가져와서 설정한다.
        audioMixer.SetFloat("BGM", Mathf.Log10(PlayerPrefs.GetFloat("BgmVolume")) * 20);
        audioMixer.SetFloat("SFX", Mathf.Log10(PlayerPrefs.GetFloat("SfxVolume")) * 20);

        // BgmSlider들이 null 이 아니라면 기존 저장된 Volume값을 슬라이더 값을 설정.
        if (BgmSlider != null && SfxSlider != null)
        {
            BgmSlider.value = PlayerPrefs.GetFloat("BgmVolume");
            SfxSlider.value = PlayerPrefs.GetFloat("SfxVolume");

        }

        string nowSceneBgmName=null;

        #region 씬 이름에 따라 BGM 이름(nowSceneBgmName) 결정
        string nowSceneName = SceneManager.GetActiveScene().name.ToString();

        //씬 이름에 따라 BGM 이름 정함
        for (int i = 0; i < SceneNamesList.Length; i++)
        {
            if (nowSceneName == SceneNamesList[i])
            {
                if (i == 1 || i == 2) //EndingScene일 경우 플레이어 게임 결과에 따라 
                {
                    string winnerNick = PlayerPrefs.GetString("Winner"); // 우승자 이름 저장
                    if (winnerNick == PlayerPrefs.GetString("_myNick")) //게임승리자가 누구인지 확인 
                    {
                        nowSceneBgmName = BgmNameList[1]; // 승리한 BGM
                        Debug.Log(nowSceneBgmName);
                        break;
                    }
                    else
                    {
                        nowSceneBgmName = BgmNameList[2]; // 졌을 때 BGM
                        Debug.Log(nowSceneBgmName);
                        break;
                    }

                }
                nowSceneBgmName = BgmNameList[i];
                Debug.Log(nowSceneBgmName);
                break;
            }

        }
        #endregion 씬 이름에 따라 BGM 이름(nowSceneBgmName) 결정

        Debug.Log(nowSceneBgmName);
        PlayBGM(nowSceneBgmName);

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
        PlayerPrefs.SetFloat("BgmVolume", BgmSlider.value);

        //audioMixer.SetFloat("BGM", BgmSlider.value);
    }

    public void SetSfxVolume()
    {
        ////로그연산값 전달
        audioMixer.SetFloat("SFX", Mathf.Log10(SfxSlider.value) * 20);
        PlayerPrefs.SetFloat("SfxVolume", SfxSlider.value);
        //audioMixer.SetFloat("SFX",SfxSlider.value);
    }

}
