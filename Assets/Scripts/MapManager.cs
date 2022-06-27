using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LevelData
{
	public Level level;
}

public class MapManager : MonoBehaviour
{
	public LevelData[] levels;
	public Level startLevel = null;

    public GameObject previewCam;

	[HideInInspector] public Level level;

    // Start is called before the first frame update
    public void Init()
	{
		//var levels = leveldraw;
		var index = GameController.inst.loadedData.currLevel % levels.Length;
		Debug.Log("loading level " + index);
		level = Instantiate(startLevel==null ? levels[index].level : startLevel);
	}

	// Update is called once per frame
	void Update()
	{
		
	}
}
