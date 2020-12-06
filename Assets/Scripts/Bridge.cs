using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bridge : MonoBehaviour
{
    private Collider2D _collider;


    private void Awake()
    {
        _collider = GetComponent<Collider2D>();
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
            collision.collider.GetComponent<Player>().SetOnBridgeState(_collider);
    }


    private void OnCollisionExit2D(Collision2D collision)
    {

        if (collision.collider.CompareTag("Player"))
            collision.collider.GetComponent<Player>().SetOnBridgeState(null);
    }
}
