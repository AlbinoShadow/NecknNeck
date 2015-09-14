/*
 * Name: ImperfectMazeGen.cs
 * 
 * Use: Generates an imperfect maze, when imperfect maze is defined as
 *      a maze that "splits" and is not one single path. It must use its
 *      own vertice connect code.
 * 
 * Method: It generates the maze via depth searching that occasionally
 *         moves backwards to create a different path, leading to various
 *         outward paths.
 */

using UnityEngine;
using System;
using System.Collections;

public enum powerUp {
    speedUp,
    speedDownEnemy,
    arrow
};

public class PowerUps : MonoBehaviour {

    private int powerUpType;
    private SpriteRenderer myRenderer;
    private PlayerControl playerControls;

    IEnumerator speedUp(GameObject player)
    {
        print("speedUp");
        Destroy(this.GetComponent<BoxCollider2D>());
        myRenderer.color = Color.clear;
        playerControls.smooth = playerControls.smooth * 2;
        yield return new WaitForSeconds(15);
        playerControls.smooth = playerControls.smooth / 2;
        Destroy(this);

    }

    IEnumerator speedDownEnemy(GameObject player)
    {
        // Not implemented yet... Needs network shit done first.
        print("speedDown");
        Destroy(this.GetComponent<BoxCollider2D>());
        myRenderer.color = Color.clear;
        yield return new WaitForSeconds(15);
        Destroy(this);
    }

    IEnumerator arrow(GameObject player)
    {
        // Not implemented yet.
        print("arrow");
        Destroy(this.GetComponent<BoxCollider2D>());
        myRenderer.color = Color.clear;
        yield return new WaitForSeconds(15);
        Destroy(this);
    }

    void Awake()
    {
        GameObject player = GameObject.FindGameObjectWithTag(Tags.player);
        int randomPowerUp = UnityEngine.Random.Range(0,(Enum.GetNames(typeof(powerUp)).Length));
        
        playerControls = player.GetComponent<PlayerControl>();
        myRenderer = gameObject.GetComponent<SpriteRenderer>();
        powerUpType = randomPowerUp;

        if(powerUpType == (int)powerUp.speedUp)
            myRenderer.color = Color.green;

        if(powerUpType == (int)powerUp.speedDownEnemy)
            myRenderer.color = Color.gray;

        if(powerUpType == (int)powerUp.arrow)
            myRenderer.color = Color.blue;
    }

    void OnTriggerEnter2D(Collider2D playersCollider)
    {
        print("entered");
        GameObject player = playersCollider.gameObject;

        if(powerUpType == (int)powerUp.speedUp)
            StartCoroutine(speedUp(player));

        if(powerUpType == (int)powerUp.speedDownEnemy)
            StartCoroutine(speedDownEnemy(player));

        if(powerUpType == (int)powerUp.arrow)
            StartCoroutine(arrow(player));
    }
}