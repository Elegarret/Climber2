using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;


public class UIManager : MonoBehaviour
{
	public static UIManager inst;

	public GameObject GameoverLayer;
	public GameObject GameplayLayer;
	public GameObject WelcomeLayer;
	public GameObject WinLayer;
	public GameObject DrawLayer;
	public GameObject SettingsLayer;
	public GameObject GarageLayer;
	public GameObject NameLayer;
	public GameObject ShopLayer;
	public Camera overlayCamera;
	public Text scoreText;
	public GameObject progressBar;

	//DRAWLAYER REFERENCES
	[Space]
	[Header("Draw screen")]
	public TextMeshProUGUI draw_moneyCaption;

	//WINLAYER REFERENCES
	[Space]
	[Header("Win screen")]
	public TextMeshProUGUI win_moneyCaption;

	[Space]
	[Header("Gameplay screen")]
	public TextMeshProUGUI play_moneyCaption;

	[Space]
	[Header("Shop screen")]
	public TextMeshProUGUI shop_moneyCaption;

	private void Awake()
	{
		if(inst!= null)
		{
			Destroy(gameObject);
		}
		else
		{
			inst = this;
		}
	}

	private void Start()
	{

	}


	public void UpdateCoinsVisual(int amount)
	{
		if (DrawLayer && DrawLayer.activeSelf && Mathf.Abs(amount - GameController.inst.Coins) > 10 && GameController.inst.Coins != 0)
		{
			int coinBuffer = GameController.inst.Coins;
			DOTween.To(() => coinBuffer, x => coinBuffer = x, amount, 4).OnUpdate(() => draw_moneyCaption.text = coinBuffer.ToString());
		}
		else if (draw_moneyCaption)
		{
			draw_moneyCaption.text = $"{amount}";
		}

		if (play_moneyCaption)
		{
			play_moneyCaption.text = $"{amount}";
		}

		if (win_moneyCaption)
		{
			if (amount > GameController.inst.Coins + 10)
			{
				int coinBuffer = GameController.inst.Coins;
				DOTween.To(() => coinBuffer, x => coinBuffer = x, amount, 4).OnUpdate(() => win_moneyCaption.text = coinBuffer.ToString());
			}
			else
			{
				win_moneyCaption.text = $"{amount}";
			}
		}

		if(ShopLayer)
		{
			if (Mathf.Abs(amount - GameController.inst.Coins) > 10 && GameController.inst.Coins != 0)
			{
				int coinBuffer = GameController.inst.Coins;
				DOTween.To(() => coinBuffer, x => coinBuffer = x, amount, 4).OnUpdate(() => shop_moneyCaption.text = coinBuffer.ToString());
			}
			else
				shop_moneyCaption.text = $"{amount}";
		}
	}
}
