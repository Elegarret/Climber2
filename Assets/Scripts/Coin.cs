using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    public GameObject model;
    public ParticleSystem ps;
    public bool active = true;

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (active && other.gameObject.CompareTag("Car"))
		{
            ps.Play();
            active = false;
            model.SetActive(false);
			GameController.inst.Coins++;
			GameController.inst.coinsPerLevel++;
		}
	}
}
