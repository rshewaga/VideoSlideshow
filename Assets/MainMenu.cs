using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

/// <summary>
/// Rob Shewaga @Wabolas 2018/04
/// Handles the start menu.
/// </summary>

public class MainMenu:MonoBehaviour {
	public static MainMenu inst;
	public InputField	m_vidDir;
	public Text			m_vidsFound;
	public InputField	m_timePer;
	public InputField	m_playbackSpeed;
	public GameObject	m_C_MainMenu;
	public Image		m_I_BGColor;
	public ColorPicker	m_CP_BGColor;

	private void Awake() {
		if(inst==null) inst=this;
		else {
			Debug.Log("Err: MainMenu.Awake detects duplicate - destroying this.");
			DestroyImmediate(this);
		}
	}
	void Start() {
		Application.runInBackground=true;
	}

	void Update() {
		string[] _files=null;
		try {
			if(m_vidDir.text.Length>0)
				_files=Directory.GetFiles(m_vidDir.text);
		}
		catch(DirectoryNotFoundException e){}
		catch(IOException e){}

		if(_files!=null) {
			int _mp4Count=0;
			for(int i=0; i<_files.Length; i++)
				if((_files[i].EndsWith(".mp4"))||(_files[i].EndsWith(".webm"))) _mp4Count++;
			m_vidsFound.text="Found "+_mp4Count+" videos.";
		}
		else m_vidsFound.text="No videos found.";

		m_I_BGColor.color=m_CP_BGColor.m_hoverColor;

		if(Input.GetKeyDown(KeyCode.Escape)) Application.Quit();
	}

	public void UI_Start() {
		if(m_vidDir.text.Length==0) return;

		string[] _files=null;
		try {
			_files=Directory.GetFiles(m_vidDir.text);
		}
		catch(DirectoryNotFoundException e){}

		if(_files!=null) {
			int _vidCount=0;
			for(int i=0; i<_files.Length; i++)
				if(_files[i].EndsWith(".mp4")) _vidCount++;

			if(_vidCount>0) {
				Player.inst.StartPlayer(_files, float.Parse(m_timePer.text), float.Parse(m_playbackSpeed.text));

				Player.inst.m_BGTrans.gameObject.GetComponent<Image>().color=m_CP_BGColor.m_hoverColor;
				m_C_MainMenu.SetActive(false);
			}
		}
	}
}
