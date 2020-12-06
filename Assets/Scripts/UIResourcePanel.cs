using UnityEngine;
using UnityEngine.UI;

public class UIResourcePanel : MonoBehaviour
{
    [SerializeField] private Collectable.CollectableType _type;
    [SerializeField] private Text _text;


    private void OnEnable()
    {
        Player.ResourcesUpdated += UpdateResource;
    }


    private void OnDisable()
    {
        Player.ResourcesUpdated -= UpdateResource;
    }


    private void UpdateResource(int amount, Collectable.CollectableType type)
    {
        if(type == _type)
        {
            _text.text = amount.ToString();
        }
    }
}
