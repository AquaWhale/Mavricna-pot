using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera : MonoBehaviour
{
    private Transform playerTransform;
    //izracuna zacetni offset od playerja, da ga uposteva pri izracunu
    private Vector3 startOffsetVectorFromPlayer;
    //vektor za katerega se bo premaknila kamera vsak update
    private Vector3 moveVector;

    //animacija kamere na začetku
    private float transition = 0.0f;
    private float animationDuration = 3.0f;
    private Vector3 animationOffset = new Vector3(0, 8, 5);

    private

    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        //izracunamo za koliko je camera odmaknjena od playerja na začetku
        startOffsetVectorFromPlayer = transform.position - playerTransform.position;
    }

    // Update is called once per frame
    void Update()
    {
        //na zacetku pristejemo moveVector playerju (dobimo kje se kamera nahaja glede na player in začetni offset)
        moveVector = playerTransform.position + startOffsetVectorFromPlayer;

        //kamero animiramo samo če transition še ni končan, potem ko se konča, kamera sledi playerju, ker je del objekta Player
        if (transition <= 1.0f) 
        {
            //Animacija na začetku igre, pomik kamere od zgoraj na igralca
            transform.position = Vector3.Lerp(moveVector + animationOffset, moveVector, transition);
            transition += Time.deltaTime * 1 / animationDuration;
            transform.LookAt(playerTransform.position + Vector3.up);
        }
    }
}
