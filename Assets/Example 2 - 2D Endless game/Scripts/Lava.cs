using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class Lava : MonoBehaviour
{
    [SerializeField] Transform player;
    [SerializeField] Volume volume;
    [SerializeField] float startSpeed = 0.5f;
    [SerializeField] float speedIncreasePerSecond = 0.01f;
    [SerializeField] float maxDistanceBelowPlayer = 10f;
    [SerializeField] float dangerDistance = 5f;

    GameManager gameManager;
    float speed;
    Vignette vignette;

    void Start()
    {
        gameManager = GameManager.Instance;
        speed = startSpeed;
        volume.profile.TryGet(out vignette);
    }

    void Update()
    {
        if (gameManager.IsGameOver) return;

        speed += speedIncreasePerSecond * Time.deltaTime;
        transform.position += Vector3.up * speed * Time.deltaTime;

        float top = transform.position.y + transform.localScale.y / 2f;
        float lowestAllowedTop = player.position.y - maxDistanceBelowPlayer;
        if (top < lowestAllowedTop)
        {
            transform.position += Vector3.up * (lowestAllowedTop - top);
        }

        float distance = player.position.y - top;
        float danger = 1f - Mathf.Clamp01(distance / dangerDistance);
        if (distance < dangerDistance)
        {
            vignette.intensity.value = Mathf.Lerp(0.3f, 1f, danger);
        }
        else
        {
            vignette.intensity.value = 0f;
        }
    }
}
