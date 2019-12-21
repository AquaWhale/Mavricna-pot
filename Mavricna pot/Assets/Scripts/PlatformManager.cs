using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformManager : MonoBehaviour
{
    MainCharacter playerScript;

    [SerializeField]
    private GameObject[] platforms;

    private GameObject player;

    private float platformOffset = 80.0f;
    private int currIndex = 0;
    private float platformLength = 200.0f;
    private int numOfPlatforms = 0;
    private float waitUntilDestroy = 120.0f;
    //hranimo pozicijo zadnje postavljene platforme
    private float positionOfLast = 80;

    private List<GameObject> activePlatforms;

    //beleži čez koliko časa bomo povečali hitrost -> povečevali jo bomo enakomerno dokler ne doseže maksimalno
    public float speedIncreaseAfterTime; //
    public float speedMultiplier; //za koliko bomo pomnozili hitrost, da bo narasla
    //belezi kdaj nazadnje je povečal hitrost
    private float lastTimeSpeedIncrese = 0.0f;

    //hitrost katero lahko doseže maksimalno, potem se neha pospeševanje
    private float maxSpeed;

    private float currentV = 0;

    private readonly float interpolation = 10;

    // Start is called before the first frame update
    private void Start()
    {
        //setamo parametre hitrosti
        maxSpeed = 3 * GameState.moveSpeedPlatform;

        player = GameObject.FindGameObjectWithTag("Player");
        numOfPlatforms = platforms.Length;
        activePlatforms = new List<GameObject>();
        playerScript = GameObject.FindObjectOfType<MainCharacter>();

        for (int i = 0; i < platforms.Length; i++)
        {
            spawnPlatform(true);
            positionOfLast += platformLength;
        }
    }

    private void Update()
    {
        if ((activePlatforms[0].transform.position.z + waitUntilDestroy) < player.transform.position.z )
        //if (playerTransform.position.z - waitUntilDestroy > (platformOffset - numOfPlatforms * platformLength)) 
        {
            spawnPlatform();
            deletePlatform();
        }
        movePlatform();
    }

    //Funkcija, ki bo prestavila platformo na konec zadnje postavljene platforme
    private void spawnPlatform(bool start = false)
    {
        //postavi kopijo(novo instanco) platforme na konec in v objekt Platform Manager
        GameObject newPlatform = Instantiate(platforms[currIndex]) as GameObject;
        newPlatform.transform.SetParent(this.transform);
        if (!start)
        {
            positionOfLast = activePlatforms[numOfPlatforms - 1].transform.position.z;
            newPlatform.transform.position = new Vector3(-4.649377f, -2.548595f, positionOfLast + platformLength);
        }
        else
        {
            newPlatform.transform.position = new Vector3(-4.649377f, -2.548595f, positionOfLast);
        }
        //poveča število postavljenih platform
        currIndex++;
        //zapisemo ostanek pri deljenju da bo vedno 0 1 ali 2 v generiranju po vrsti
        currIndex = currIndex % platforms.Length;
        //poveča razdaljo na kateri mora postavit naslednjo platformo
        platformOffset += platformLength;
        //doda dodano platformo na seznam aktivnih platform, ki so vidve kameri in po katerih bo igralec tekel se
        activePlatforms.Add(newPlatform);
        //setamo position zadnjega dodanega elementa

    }
    private void deletePlatform()
    {
        //odstranimo platformo, ki je prva na listi
        Destroy(activePlatforms[0]);
        //odstranimo jo iz seznama aktivnih ker ne obstaja več
        activePlatforms.RemoveAt(0);

    }

    private void movePlatform()
    {
        float v = 1;

        currentV = Mathf.Lerp(currentV, v, Time.deltaTime * interpolation);

        transform.position += new Vector3(transform.forward.x, transform.forward.y, transform.forward.z*(-1.0f)) * currentV * GameState.moveSpeedPlatform * Time.deltaTime;


        //preverimo, če smo je že pretekel čas po katerem povečamo hitrost, da povečamo hitrost in preverimo, če ni hitrost že narasla na maksimalno hitrost, potem prenehamo povečevat hitrost
        if (GameState.moveSpeedPlatform < maxSpeed && (Time.time - lastTimeSpeedIncrese) > speedIncreaseAfterTime) //pogledamo če še nismo dosegli max hitrosti in če je že pretekel čas v katerem bomo povelevali hitrost
        {
            //zabeležimo da smo pravkar povečali hitrost
            lastTimeSpeedIncrese = Time.time;

            //povečamo hitrost premikanja platform
            GameState.moveSpeedPlatform = GameState.moveSpeedPlatform * speedMultiplier;

            //povečamo hitrost premikanja playerja levo in desno
            GameState.leftRightSpeed *= speedMultiplier;

        }
    }

}
