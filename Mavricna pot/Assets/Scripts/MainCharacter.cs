using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class MainCharacter : MonoBehaviour
{
    [SerializeField] 
    private float jumpForce = 4;
    [SerializeField] 
    private Animator animator;
    [SerializeField] 
    private Rigidbody rigidBody;

    private float currentV = 0;
    //hitrost za premik levo in desno ko je ta zaznan
    private float currentH = 0;

    private readonly float interpolation = 10;

    private bool wasGrounded;

    private float jumpTimeStamp = 0;
    private float minJumpInterval = 0.25f;

    private bool isGrounded;
    private List<Collider> collisions = new List<Collider>();
  
    private int previousLane = 1;
    private float h = 0;
    private int lane = 1; //0 - levo, 1 - sredina, 2 - desno
    private float[] positionX = { -3.4f, 0.0f, 3.4f };

    //da vemo v katero smer se premaknemo
    private string previousKeyPress = "";

    //as long as the starting animation is going on, player cannot move
    private float animationDuration = 3.0f;

    //holds canvas object for projecting power up pickups
    private GameObject canvas;
    //this is meant for UI power up icon maintanance
    private float distanceBetwenUIIcons = -63.0f;
    private float nextPositionXOffsetOfUIIcon = 0.0f;
    //magnet instance icon power and prefab
    public GameObject magnetActiveIconPrefab;
    private GameObject magnetActiveIconInstance = null;
    private List<GameObject> activeIconInstances = new List<GameObject>();
    private int numOfActiveIcons = 0;
    //2x instance icon power and prefab
    public GameObject twoXActiveIconPrefab;
    private GameObject twoXActiveIconInstance = null;
    //private GameObject twoXActiveIconInstance = null;


    //collect collectibles in naredi določeno akcijo
    void OnTriggerEnter(Collider other)
    {
        //magnet coin power up
        if (other.gameObject.CompareTag("Magnet"))
        {
            other.gameObject.SetActive(false);
            GameState.enableMagnet();
            //Show on canvas that magnet is active -> ce instanca ze obstaja je ne kreiramo se enkrat
            if (magnetActiveIconInstance == null)
            {
                magnetActiveIconInstance = Instantiate(magnetActiveIconPrefab) as GameObject;
                magnetActiveIconInstance.transform.SetParent(canvas.transform, false);
                //set x position of icon power up where it is supposed to go
                magnetActiveIconInstance.transform.localPosition += new Vector3 (nextPositionXOffsetOfUIIcon, 0, 0);
                //posodobi pozicijo naslednjega power up icona
                activeIconInstances.Add(magnetActiveIconInstance);
                nextPositionXOffsetOfUIIcon += distanceBetwenUIIcons;
            }
            //prikazemo trenutno stanje magnet time counterja
            //vzamemo 2. gameobject znotraj instance magnet UI
            int magnetActiveIconIndex = activeIconInstances.IndexOf(magnetActiveIconInstance);
            Text magnetTimeCounter = activeIconInstances[magnetActiveIconIndex].gameObject.transform.GetChild(1).gameObject.GetComponent<Text>();
            magnetTimeCounter.text = GameState.totalMagnetTime.ToString("F0");

        }
        //2x power up
        else if (other.gameObject.CompareTag("2X"))
        {
            other.gameObject.SetActive(false);
            GameState.enable2XPower();
            //Show on canvas that 2x is active -> ce instanca ze obstaja je ne kreiramo se enkrat
            if (twoXActiveIconInstance == null)
            {
                twoXActiveIconInstance = Instantiate(twoXActiveIconPrefab) as GameObject;
                twoXActiveIconInstance.transform.SetParent(canvas.transform, false);
                //set x position of icon power up where it is supposed to go
                twoXActiveIconInstance.transform.localPosition += new Vector3(nextPositionXOffsetOfUIIcon, 0, 0);
                //posodobi pozicijo naslednjega power up icona
                activeIconInstances.Add(twoXActiveIconInstance);
                nextPositionXOffsetOfUIIcon += distanceBetwenUIIcons;
            }
            //prikazemo trenutno stanje 2x time counterja
            //vzamemo 2. gameobject znotraj instance 2x UI
            int twoXActiveIconIndex = activeIconInstances.IndexOf(twoXActiveIconInstance);
            Text twoXTimeCounter = activeIconInstances[twoXActiveIconIndex].gameObject.transform.GetChild(1).gameObject.GetComponent<Text>();
            twoXTimeCounter.text = GameState.total2XPowerTime.ToString("F0");
        }
    }

    private void Start()
    {
        canvas = GameObject.Find("Canvas");
    }

    void Update()
    {
        animator.SetBool("Grounded", isGrounded);

        if (Time.time < animationDuration)
        {
            //Če smo še vedno v animaciji samo teče naprej
            Run();
        }
        else
        {
            //če nismo več v animaciji vključimo možnost premikanja levo in desno in ostale funkcionalnosti igre
            //1.) FUNKCIONALNOST MAGNETA
            if (GameState.hasMagnetPower())
            {
                //najde objekte v magnetRadius okoli sebe
                Collider[] colliders;
                colliders = Physics.OverlapSphere(this.transform.position, GameState.magnetRadius);

                foreach (Collider collider in colliders)
                {
                    if (collider.CompareTag("Coin"))
                    {
                        Transform coinTransform = collider.transform;
                        Vector3 goTo = new Vector3(this.transform.position.x, this.transform.position.y + 1.5f, this.transform.position.z + 6.0f*Time.deltaTime);
                        var diffVector = (coinTransform.position - goTo);
                        var distanceFromPlayer = diffVector.sqrMagnitude;
                        if (distanceFromPlayer < 1.5f)
                        {
                            collider.gameObject.SetActive(false);
                        }
                        //premaknemo kovanček proti playerju z enako hitrostjo kot se gibajo platforme
                        coinTransform.position = Vector3.Lerp(coinTransform.position, goTo, (GameState.moveSpeedPlatform) * Time.deltaTime);
                    }
                }
                GameState.totalMagnetTime -= Time.deltaTime;
                //posodobimo trenutno stanje magnet time counterja
                //vzamemo 2. gameobject znotraj instance magnet UI
                int magnetActiveIconIndex = activeIconInstances.IndexOf(magnetActiveIconInstance);
                Text magnetTimeCounter = activeIconInstances[magnetActiveIconIndex].gameObject.transform.GetChild(1).gameObject.GetComponent<Text>();
                magnetTimeCounter.text = GameState.totalMagnetTime.ToString("F0");
                if (GameState.totalMagnetTime <= 0.0f)
                {
                    GameState.disableMagnet();
                    //destrojamo instanco magnet icona, ker je power potekel
                    Destroy(activeIconInstances[magnetActiveIconIndex]);
                    activeIconInstances.RemoveAt(magnetActiveIconIndex);
                    magnetActiveIconInstance = null;

                    //posodobi pozicijo naslednjega powerupa (zamakni nazaj za razmik med dvema)
                    nextPositionXOffsetOfUIIcon -= distanceBetwenUIIcons;

                    //Posodobimo v seznamu vseh icon power upov position power upov za pravkar deletanim da se pomaknejo naprej
                    for (int i = magnetActiveIconIndex; i < activeIconInstances.Count; i++)
                    {
                        activeIconInstances[i].transform.localPosition -= new Vector3(distanceBetwenUIIcons, 0, 0);
                    }
                }
            }

            //2.) FUNKCIONALNOST PREMIKANJA
            //ce pritisnemo na A
            if (Input.GetKeyDown(KeyCode.A))
            {

                if (GameState.swapKeys)
                {
                    //code for D key, because A is now D
                    if ((lane + 1) <= 2)
                    {
                        h = 1;
                        if (!previousKeyPress.Equals("D"))
                        {
                            previousLane = lane;
                        }
                        lane += 1;
                        previousKeyPress = "D";
                    }
                }
                else
                {
                    if ((lane - 1) >= 0)
                    {
                        h = -1;
                        if (!previousKeyPress.Equals("A"))
                        {
                            previousLane = lane;
                        }
                        lane -= 1;
                        previousKeyPress = "A";
                    }
                }
            }
            //ce pritisnemo D
            if (Input.GetKeyDown(KeyCode.D))
            {
                if (GameState.swapKeys)
                {
                    if ((lane - 1) >= 0)
                    {
                        h = -1;
                        if (!previousKeyPress.Equals("A"))
                        {
                            previousLane = lane;
                        }
                        lane -= 1;
                        previousKeyPress = "A";
                    }
                }
                else
                {
                    if ((lane + 1) <= 2)
                    {
                        h = 1;
                        if (!previousKeyPress.Equals("D"))
                        {
                            previousLane = lane;
                        }
                        lane += 1;
                        previousKeyPress = "D";
                    }
                }

            }

            //3.) FUNKCIONALNOST 2X POWER UPA (kovanček je vreden dvakrat več)
            if (GameState.has2XPower())
            {
                GameState.total2XPowerTime -= Time.deltaTime;
                int twoXActiveIconIndex = activeIconInstances.IndexOf(twoXActiveIconInstance);
                //Posodobimo stanje 2x time counterja
                Text twoXTimeCounter = activeIconInstances[twoXActiveIconIndex].gameObject.transform.GetChild(1).gameObject.GetComponent<Text>();
                twoXTimeCounter.text = GameState.total2XPowerTime.ToString("F0");
                if (GameState.total2XPowerTime <= 0.0f)
                {
                    GameState.disable2XPower();
                    //destrojamo instanco 2x icona, ker je power potekel
                    Destroy(activeIconInstances[twoXActiveIconIndex]);
                    activeIconInstances.RemoveAt(twoXActiveIconIndex);
                    twoXActiveIconInstance = null;
                    //posodobi pozicijo naslednjega powerupa (zamakni nazaj za razmik med dvema)
                    nextPositionXOffsetOfUIIcon -= distanceBetwenUIIcons;

                    //Posodobimo v seznamu vseh icon power upov position power upov za pravkar deletanim da se pomaknejo naprej
                    for (int i = twoXActiveIconIndex; i < activeIconInstances.Count; i++)
                    {
                        activeIconInstances[i].transform.localPosition -= new Vector3(distanceBetwenUIIcons, 0, 0);
                    }
                }
            }
            //NA KONCU SE ŠE PREMAKNEMO -> oziroma samo animiramo tek
            Run();
        }


        wasGrounded = isGrounded;
    }

    private void Run()
    {
        float v = 1;
        
        //current V je samo zato, da vemo kako hitro naj poteka animacija teka
        currentV = Mathf.Lerp(currentV, v, Time.deltaTime * interpolation);
        currentH = Mathf.Lerp(currentH, h, Time.deltaTime * interpolation);

        transform.position += transform.right * currentH * GameState.leftRightSpeed * Time.deltaTime;

        var pos = transform.position;

        //za premikanje levo in desno
        pos.x = Mathf.Clamp(transform.position.x, (previousLane < lane) ? positionX[previousLane] : positionX[lane], (previousLane > lane) ? positionX[previousLane] : positionX[lane]);
        transform.position = pos;

        animator.SetFloat("MoveSpeed", currentV);

        JumpingAndLanding();
    }

    private void JumpingAndLanding()
    {
        bool jumpCooldownOver = (Time.time - jumpTimeStamp) >= minJumpInterval;

        if (jumpCooldownOver && isGrounded && Input.GetKey(KeyCode.Space))
        {
            jumpTimeStamp = Time.time;
            rigidBody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }

        if (!wasGrounded && isGrounded)
        {
            animator.SetTrigger("Land");
        }

        if (!isGrounded && wasGrounded)
        {
            animator.SetTrigger("Jump");
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        ContactPoint[] contactPoints = collision.contacts;
        for (int i = 0; i < contactPoints.Length; i++)
        {
            if (Vector3.Dot(contactPoints[i].normal, Vector3.up) > 0.5f)
            {
                if (!collisions.Contains(collision.collider))
                {
                    collisions.Add(collision.collider);
                }
                isGrounded = true;
            }
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        ContactPoint[] contactPoints = collision.contacts;
        bool validSurfaceNormal = false;
        for (int i = 0; i < contactPoints.Length; i++)
        {
            if (Vector3.Dot(contactPoints[i].normal, Vector3.up) > 0.5f)
            {
                validSurfaceNormal = true; break;
            }
        }

        if (validSurfaceNormal)
        {
            isGrounded = true;
            if (!collisions.Contains(collision.collider))
            {
                collisions.Add(collision.collider);
            }
        }
        else
        {
            if (collisions.Contains(collision.collider))
            {
                collisions.Remove(collision.collider);
            }
            if (collisions.Count == 0) { isGrounded = false; }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collisions.Contains(collision.collider))
        {
            collisions.Remove(collision.collider);
        }
        if (collisions.Count == 0) { isGrounded = false; }
    }

}
