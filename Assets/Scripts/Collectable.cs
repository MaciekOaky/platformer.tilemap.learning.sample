using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[ExecuteInEditMode]
public class Collectable : MonoBehaviour
{
    public enum CollectableType { OXYGEN, FUEL };
    public enum CollectableSize { SMALL, NORMAL };

    [SerializeField] private CollectableType _type;
    [SerializeField] private CollectableSize _size;
    [SerializeField] private List<Sprite> _smallSprites, _normalSprites;
    [SerializeField] [Range(1,10)] private int _smallAmount, _normalAmount;
    [SerializeField] private float _animationSpeed;

    private SpriteRenderer _sr;


    private void Awake()
    {
        _sr = GetComponent<SpriteRenderer>();
    }


    private void Start()
    {
        StartCoroutine(Animation());
    }


    private void Update()
    {
        if (!Application.isPlaying)
        {
            var sprites = GetSprites();
            if(sprites != null && sprites.Count > 1)
                _sr.sprite = sprites[0];
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<Player>().UpdateResource(GetAmount(), _type);
            gameObject.SetActive(false); // Disappear when gathered
        }
    }


    private int GetAmount() => _size == CollectableSize.NORMAL ? _normalAmount : _smallAmount;
    private List<Sprite> GetSprites() => _size == CollectableSize.NORMAL ? _normalSprites : _smallSprites;

    private IEnumerator Animation()
    {
        List<Sprite> sprites = GetSprites();

        int id = 0;

        if (sprites.Count > 1) // Run animation only if we have more than 1 sprite.
            while (true)
            {
                _sr.sprite = sprites[id++ % sprites.Count];
                yield return new WaitForSeconds(_animationSpeed > Mathf.Epsilon ? _animationSpeed : .1f);
            }
    }
}
