using UnityEngine;


public class Collectable : MonoBehaviour
{
    public enum CollectableType { OXYGEN, FUEL };

    [SerializeField] private CollectableType _type;
    [SerializeField] [Range(1, 10)] private int _amount;

    private Animator _animator;


    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }


    private void Start()
    {
        SetAnimation();
    }


    private void SetAnimation()
    {
        bool isOxygen = _type == CollectableType.OXYGEN;
        _animator.SetBool("Oxygen", isOxygen);
        _animator.SetBool("Fuel", !isOxygen);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<Player>().UpdateResource(_amount, _type);
            gameObject.SetActive(false); // Disappear when gathered
        }
    }
}
