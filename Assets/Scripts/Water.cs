using UnityEngine;

public class Water : MonoBehaviour
{
    private Animator _animator;
    

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<Player>().SetInWaterState(true);
            if(_animator != null) _animator.SetTrigger("Reveal");
        }
    }


    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<Player>().SetInWaterState(false);
            if(_animator != null) _animator.SetTrigger("Hide");
        }
    }
}
