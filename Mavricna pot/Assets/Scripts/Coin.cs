using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Coin : MonoBehaviour
{
    //effect pobiranja kovančkov
    [SerializeField]
    private GameObject coinPickupEffect;

    private float coinEffectDuration = 2.0f;

    private Text coinScore;

    private void Start()
    {
        coinScore = GameObject.Find("CoinScore").GetComponent<Text>();
        coinScore.text = GameState.coinScore.ToString("0000000");
    }
    //ko se player zaleti vanj
    private void OnTriggerEnter(Collider other)
    {
        //ce se player zaleti vanj
        if (other.gameObject.CompareTag("Player"))
        {
            this.gameObject.SetActive(false);
            //naredi effect pobranega
            GameObject effect = Instantiate(coinPickupEffect) as GameObject;
            effect.transform.SetParent(other.transform);
            effect.transform.position = new Vector3(other.transform.position.x, other.transform.position.y + 1.5f, other.transform.position.z);
            GameState.collectCoin();
            coinScore.text = GameState.coinScore.ToString("0000000");
            Destroy(effect, coinEffectDuration);

        }
    }
}
