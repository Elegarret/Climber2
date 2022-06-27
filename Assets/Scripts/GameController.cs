using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class GameController : MonoBehaviour
{
	public enum States { start, play, endgame, gameover, win };
	[HideInInspector] public States State = States.start;
	[HideInInspector] public int HighScore = 0;
	[HideInInspector] public static GameController inst;
	[HideInInspector] public int CoinsLevel = 0;

	[HideInInspector] public AudioManager audioManager;
	[HideInInspector] public MapManager mapManager;
	[HideInInspector] public SaveGame loadedData;
	[HideInInspector] public bool dataLoaded = false;

	public Canvas canvas;

	private TextMeshProUGUI scoreText;
	private TextMeshProUGUI scoreMultText;
	private GameObject GameoverLayer;
	private GameObject GameplayLayer;
	private GameObject WelcomeLayer;
	private GameObject WinLayer;
	public static int tryNumber = 0;

	public delegate void OnDataLoaded();
	public event OnDataLoaded onDataLoaded;

	private int score = 0;
	private int _scoreMult = 1;
	public int Score
	{
		get { return score; }
		set
		{
			score = value;
			scoreText.text = value.ToString();
		}
	}

	public int scoreMult{
		get { return _scoreMult; }
		set
		{
			if (value > _scoreMult)
			{
				scoreMultText.transform.DOKill();
				scoreMultText.transform.DOPunchScale(1.5f * Vector3.one, 0.25f); //.transform.DOShakePosition(0.25f, 50, 20);
			}
			_scoreMult = value;
			scoreMultText.text = $"x{_scoreMult}";
		}
	}

	private int coins = 0;
	public int coinsPerLevel = 0;
	public int Coins
	{
		get { return coins; }
		set
		{
			UIManager.inst.UpdateCoinsVisual(value);
			coins = value;
		}
	}



	private void Awake()
	{
		Application.targetFrameRate = 400;
		inst = this;
		Load();
	}

	// Use this for initialization
	void Start()
	{
		State = States.start;

		GameoverLayer = canvas.transform.Find("GameOverScreen").gameObject;
		GameplayLayer = canvas.transform.Find("PlayScreen").gameObject;
		WelcomeLayer = canvas.transform.Find("WelcomeScreen").gameObject;
		WinLayer = canvas.transform.Find("WinScreen").gameObject;

		scoreText = GameplayLayer.transform.Find("Score").gameObject.GetComponent<TextMeshProUGUI>();

		scoreText.gameObject.SetActive(false);

		GameoverLayer.SetActive(false);
		GameplayLayer.SetActive(false);
		WelcomeLayer.SetActive(true);
		WinLayer.SetActive(false);

		mapManager = GetComponent<MapManager>();
		mapManager.Init();

		WelcomeLayer.transform.Find("BestScore").gameObject.GetComponent<Text>().text = "Best Score: " + HighScore.ToString();
		WelcomeLayer.transform.Find("Play").gameObject.SetActive(false);

		audioManager = FindObjectOfType<AudioManager>();
	}

	public void Mute()
	{
		loadedData.muted = !loadedData.muted;
		audioManager.SwitchMute(loadedData.muted);
		audioManager.Play("10");
		Save();
	}

	public void AllowPlay()
	{
		WelcomeLayer.transform.Find("Play").gameObject.SetActive(true);
	}
	public void DisablePlay()
	{
		WelcomeLayer.transform.Find("Play").gameObject.SetActive(false);
	}
	public void Lose()
	{
		if (State == States.gameover) return;
		State = States.gameover;

		GameplayLayer.SetActive(false);
		//GameoverLayer.transform.Find("Score").gameObject.GetComponent<Text>().text = "Score: " + score.ToString();
		GameoverLayer.transform.Find("NewRecord").gameObject.SetActive(score > HighScore);
		if (score > HighScore) HighScore = score;
		GameoverLayer.transform.Find("BestScore").gameObject.GetComponent<Text>().text = "Best Score: " + HighScore.ToString();
		GameoverLayer.transform.Find("LevelPercent").gameObject.GetComponent<Text>().text = "% complete";

		//TinySauce.OnGameFinished(levelNumber: loadedData.currLevel.ToString(), false, Score);
		Camera.main.GetComponent<CameraFollow>().StopFollow();
		StartCoroutine(WaintAndShowLose());
		tryNumber++;
		Debug.Log("lose! " + (score / 25 * 25) + ", try: " + tryNumber);
	}

	IEnumerator WaintAndShowLose()
	{
		yield return new WaitForSeconds(1f);

		GameoverLayer.SetActive(true);
		Save();
	}

	public void EndGame()
	{
		if (State == States.endgame || State == States.win) return;
		State = States.endgame;
	}

	public void Win()
	{
		if (State == States.win) return;
		// WinLayer.transform.Find("NewRecord").gameObject.SetActive(score > HighScore);
		if (score > HighScore) HighScore = score;
		//GameoverLayer.transform.Find("BestScore").gameObject.GetComponent<Text>().text = "Best Score: " + HighScore.ToString();
		//WinLayer.transform.Find("LevelComplete").gameObject.GetComponent<TextMeshProUGUI>().text = "Level " + loadedData.currLevel + " complete!";
		//WinLayer.transform.Find("Princess").gameObject.SetActive(false/*loadedData.currLevel % 3 == 0*/);

		//TinySauce.OnGameFinished(levelNumber: loadedData.currLevel.ToString(), true, Score);

		TinySauce.OnGameFinished(true, 0, levelNumber: loadedData.currLevel.ToString());
		loadedData.currLevel++;
		State = States.win;

		Taptic.Success();
		tryNumber = 0;
		Save();
		StartCoroutine(WaitAndShowWin());
		GameplayLayer.SetActive(false);
		UIManager.inst.win_moneyCaption.text = (coinsPerLevel * scoreMult).ToString();
		coins += coinsPerLevel * scoreMult;
	}

	IEnumerator WaitAndShowWin()
	{
		yield return new WaitForSeconds(0.5f);
		WinLayer.SetActive(true);
	}

	public void NextLevel()
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}

	public void Restart()
	{
		Debug.Log("restart");
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}

	public void StartGame()
	{
		WelcomeLayer.SetActive(false);
		GameplayLayer.SetActive(true);
		State = States.play;
		//TinySauce.OnGameStarted(levelNumber: loadedData.currLevel.ToString());
	}

	public static  bool IsUIClicked()
	{
		if (Input.GetMouseButtonDown(0) /*|| Input.GetMouseButtonUp(0)*/)
		{
			Touch[] touches = Input.touches;
			int pointerId = -1; // -1 is mouse
			bool isTouchInRightPhase = true;    // for mouse
			if (touches.Length > 0)
			{
				pointerId = touches[0].fingerId;
				isTouchInRightPhase = touches[0].phase == TouchPhase.Began;
			}
			if (isTouchInRightPhase)
			{
				if (EventSystem.current.IsPointerOverGameObject(pointerId))
				{
					//Debug.Log("Clicked on the UI");
					return true;
				}
			}
		}
		return false;
	}

	// Update is called once per frame
	void Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
			Touch[] touches = Input.touches;
			int pointerId = -1; // -1 is mouse
			bool isTouchInRightPhase = true;    // for mouse
			if (touches.Length > 0)
			{
				pointerId = touches[0].fingerId;
				isTouchInRightPhase = touches[0].phase == TouchPhase.Began;
			}
			if (isTouchInRightPhase)
			{
				if (EventSystem.current.IsPointerOverGameObject(pointerId))
				{
					//Debug.Log("Clicked on the UI");
					return;
				}
			}
		}

		if (Input.GetMouseButtonDown(0))
		{
			if (State == States.start)
			{
				StartGame();
				GameplayLayer.SetActive(true);
				Debug.Log("first tap");
			}
			else if (State == States.play)
			{
				WelcomeLayer.SetActive(false);

			}
			else if (State == States.win && WinLayer.activeSelf)
			{
				NextLevel();
			}
		}
	}

	public void Load()
	{
		//PlayerPrefs.DeleteAll();
		string jsonData = PlayerPrefs.GetString("MySettings");
		loadedData = JsonUtility.FromJson<SaveGame>(jsonData);

		if (loadedData != null)
		{
			Coins = loadedData.Coins;
			//loadedData.currLevel = 0;
			//loadedData.gamesWin = 197;
			//loadedData.enemyData = null;
			//loadedData.lvlInk = 30;
			//Coins = 10000000;
			Debug.Log($"loaded; gamesPlayed: {loadedData.gamesPlayed}, gamesWin: {loadedData.gamesWin}");
		}
		else
		{
			loadedData = new SaveGame();
			Coins = 0;
			Debug.Log($"first launch, gamesPlayed: {loadedData.gamesPlayed}, gamesWin: {loadedData.gamesWin}");
		}

		onDataLoaded?.Invoke();
		dataLoaded = true;
	}

	public void Save()
	{
		SaveGame saveData = new SaveGame(); 
		saveData.Coins = Coins;
		saveData.muted = loadedData.muted;
		saveData.noVibro = loadedData.noVibro;
		saveData.gamesPlayed = loadedData.gamesPlayed;
		saveData.gamesWin = loadedData.gamesWin;
		saveData.currLevel = loadedData.currLevel;

		Debug.Log($"save gamesPlayed: {saveData.gamesPlayed}, gamesWin: {saveData.gamesWin}");

		string jsonData = JsonUtility.ToJson(saveData);
		PlayerPrefs.SetString("MySettings", jsonData);
		PlayerPrefs.Save();
	}
}

[System.Serializable]
public class SaveGame
{
	public int Coins = 0;
	public bool muted = false;
	public bool noVibro = false;
	public int gamesWin = 0;
	public int gamesPlayed = 0;
	public int currLevel = 0;
}
