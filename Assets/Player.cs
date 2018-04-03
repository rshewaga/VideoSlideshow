using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;

/// <summary>
/// Rob Shewaga @Wabolas 2018/04
/// Handles the VideoPlayer and resizing the background and video images.
/// </summary>

public class Player:MonoBehaviour{
	public static Player inst;
	public RectTransform	m_BGTrans;
	public RectTransform	m_playerTrans;
	public VideoPlayer		m_player;
	public RawImage			m_playerImage;
	public List<string>		m_videos;
	public float			m_timePer;
	public bool				m_playing;

	private int				mp_vidIndex;		//Index into m_videos of the currently playing video.
	private float			mp_curTimePer;
	private bool			mp_textureReset;	//Whether or not the PlayerImage's texture has been set to the newly loaded VideoPlayer's texture.

	private void Awake() {
		if(inst==null) inst=this;
		else {
			Debug.Log("Err: Player.Awake detects duplicate - destroying this.");
			DestroyImmediate(this);
		}
	}
	void Start() {
		m_playing=false;

		mp_vidIndex=0;
		mp_curTimePer=0;
		mp_textureReset=true;
	}

	void Update() {
		ScaleImagesAnyAspect();

		if((!mp_textureReset)&&(m_player.isPrepared)) {
			m_playerImage.texture=m_player.texture;
			mp_textureReset=true;
		}

		if(m_playing) {
			mp_curTimePer+=Time.deltaTime;
			if(mp_curTimePer>=m_timePer) {	//Time to show this video over, increment to show next.
				mp_curTimePer=0;
				mp_vidIndex++;
				if(mp_vidIndex==m_videos.Count) mp_vidIndex=0;

				m_player.url=m_videos[mp_vidIndex];
				m_player.Play();
				mp_textureReset=false;
			}

			if(Input.GetKeyDown(KeyCode.F1)) ReturnToMainMenu();
			if(Input.GetKeyDown(KeyCode.LeftArrow)) {
				mp_curTimePer=0;
				mp_vidIndex--;
				if(mp_vidIndex<0) mp_vidIndex=m_videos.Count-1;

				m_player.url=m_videos[mp_vidIndex];
				m_player.Play();
				mp_textureReset=false;
			}
			else if(Input.GetKeyDown(KeyCode.RightArrow)) {
				mp_curTimePer=0;
				mp_vidIndex++;
				if(mp_vidIndex==m_videos.Count) mp_vidIndex=0;

				m_player.url=m_videos[mp_vidIndex];
				m_player.Play();
				mp_textureReset=false;
			}
		}
	}

	public void StartPlayer(string[] _vidFiles, float _timePer, float _playbackSpeed) {
		//Start the player given the list of filenames for video files and time to show each video.
		m_videos=new List<string>();
		for(int i=0; i<_vidFiles.Length; i++)
			if((_vidFiles[i].EndsWith(".mp4"))||(_vidFiles[i].EndsWith(".webm"))) m_videos.Add(""+_vidFiles[i]);

		m_timePer=_timePer;
		m_player.playbackSpeed=0.83f*_playbackSpeed;

		mp_vidIndex=0;
		m_player.url=m_videos[mp_vidIndex];
		m_player.Play();
		mp_textureReset=false;

		m_playing=true;
	}

	public void ScaleImagesAnyAspect() {
		//Scale the size of the BG and the player RectTransforms to fit any aspect application window perfectly.
		float _currentScreenAspect=(float)Screen.height/(float)Screen.width;
		m_BGTrans.sizeDelta=new Vector2(1920, 1080*(_currentScreenAspect/(9.0f/16.0f)));
		m_playerTrans.sizeDelta=new Vector2(1920, 1080*(_currentScreenAspect/(9.0f/16.0f)));

		if(m_player.texture!=null){
			//Set the player's scaling so that the video's aspect ratio is maintained while filling as much of the screen as possible.
			float _vidAspect=(float)m_player.texture.height/(float)m_player.texture.width;
			
			if(_currentScreenAspect>_vidAspect) {
				//Width full, height adjust				
				m_playerTrans.localScale=new Vector3(1, _vidAspect/_currentScreenAspect, 1);
			}
			else {
				//Height full, width adjust
				m_playerTrans.localScale=new Vector3(_currentScreenAspect/_vidAspect, 1, 1);
			}
		}
	}

	public void ReturnToMainMenu() {
		m_playing=false;

		mp_vidIndex=0;
		mp_curTimePer=0;
		mp_textureReset=true;

		m_player.Stop();
		m_videos.Clear();
		m_playerImage.texture=null;
		MainMenu.inst.m_C_MainMenu.SetActive(true);
	}
}
