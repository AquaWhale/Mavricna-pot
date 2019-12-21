using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slide : MonoBehaviour
{
    private float originalSpeed;
    //za toliko se bo povečala hitrost, ko player se podrsa po slidu
    public static float slipperySpeed = 15.0f;

    //effect water dropov
    [SerializeField]
    private GameObject waterEffect;

    private float waterEffectDuration = 2.0f;


    //ko začne player hodit po slidu
    private void OnCollisionEnter(Collision collision)
    {
        //ce player colida z nasim slidom
        if (collision.collider.gameObject.CompareTag("Player"))
        {
            originalSpeed = GameState.moveSpeedPlatform;
            GameState.moveSpeedPlatform += slipperySpeed;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        //ce player colida z nasim slidom
        if (collision.collider.gameObject.CompareTag("Player"))
        {
            GameState.moveSpeedPlatform = originalSpeed;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        //ce player colida z nasim slidom
        if (collision.collider.gameObject.CompareTag("Player"))
        {
            //naredi water effect
            GameObject effect = Instantiate(waterEffect) as GameObject;
            effect.transform.position = new Vector3(collision.collider.transform.position.x, collision.collider.transform.position.y, collision.collider.transform.position.z);
            Destroy(effect, waterEffectDuration);
        }
    }
}
