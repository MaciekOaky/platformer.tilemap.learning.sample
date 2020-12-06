using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Water : MonoBehaviour
{
    private Tilemap _tilemap;

    private float _changeTime = .5f;
    private float _revealAlpha = .7f;
    private Coroutine _waterChangeCoroutine;

    private void Awake()
    {
        _tilemap = GetComponent<Tilemap>();
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<Player>().SetInWaterState(true);

            if (_waterChangeCoroutine != null) StopCoroutine(_waterChangeCoroutine);
            _waterChangeCoroutine = StartCoroutine(WaterTransparency(reveal: true));
        }
    }


    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<Player>().SetInWaterState(false);

            if (_waterChangeCoroutine != null) StopCoroutine(_waterChangeCoroutine);
            _waterChangeCoroutine = StartCoroutine(WaterTransparency(reveal: false));
        }
    }


    private IEnumerator WaterTransparency(bool reveal)
    {
        Color startingColor = _tilemap.color;
        Color finalColor = new Vector4(startingColor.r, startingColor.g, startingColor.b, reveal ? _revealAlpha : 1f);

        for (float time = 0f; time < _changeTime; time += Time.deltaTime)
        {
            _tilemap.color = Vector4.Lerp(startingColor, finalColor, time / _changeTime);
            yield return new WaitForEndOfFrame();
        }

        _tilemap.color = finalColor;
    }
}
