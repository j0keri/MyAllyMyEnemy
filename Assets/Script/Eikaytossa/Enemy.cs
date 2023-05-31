using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private Transform player;
    private float dist;
    public float moveSpeed;
    public float howClose;

    void Start()
    {

    }

    public float damageAmount = 10.0f;  // Vihollisen aiheuttama vahinko

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("T�rm�ys tapahtui.");

        Health playerHealth = collision.gameObject.GetComponent<Health>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(damageAmount);
            Debug.Log("Pelaaja otti vahinkoa.");
        }
        else
        {
            Debug.Log("Health-komponenttia ei l�ytynyt pelaajalta.");
        }
    }

    void Update()
    {
        // Yrit� hakea pelaaja-olio, jos sit� ei ole viel� l�ydetty
        if (player == null)
        {
            GameObject[] players = GameObject.FindGameObjectsWithTag("player");
            float closestDistance = Mathf.Infinity; // Alustetaan l�himm�n pelaajan et�isyys ��rett�m�n suureksi

            foreach (GameObject potentialPlayer in players) // K�yd��n l�pi kaikki "player"-tagilliset objektit
            {
                float distance = Vector3.Distance(potentialPlayer.transform.position, transform.position); // Lasketaan et�isyys t�st� Enemy-oliosta pelaajaan
                if (distance < closestDistance) // Jos t�m� pelaaja on l�hemp�n� kuin aiemmat pelaajat
                {
                    closestDistance = distance; // P�ivitet��n l�himm�n pelaajan et�isyys
                    player = potentialPlayer.transform; // Asetetaan t�m� pelaaja seurattavaksi
                }
            }
        }

        if (player != null)
        {
            dist = Vector3.Distance(player.position, transform.position);
            /*  // Tulostaa et�isyyden ja howClose-arvon joka kerta kun Update()-metodia kutsutaan
            Debug.Log("Distance to player: " + dist + ", How close: " + howClose);*/

            if (dist <= howClose)
            {
                transform.LookAt(player);
                GetComponent<Rigidbody>().AddForce(transform.forward * moveSpeed);
            }
        }
    }
}