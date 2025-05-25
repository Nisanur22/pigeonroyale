using UnityEngine;
using UnityEngine.UI;

public class ArenaBackgroundManager : MonoBehaviour
{
    public static ArenaBackgroundManager Instance { get; private set; }

    public Image backgroundImage;
    public Sprite[] arenaBackgrounds;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void SetArena(int index)
    {
        if (index >= 0 && index < arenaBackgrounds.Length)
            backgroundImage.sprite = arenaBackgrounds[index];
        if (GameManager.Instance != null)
            GameManager.Instance.selectedArenaIndex = index;
    }

    void Start()
    {
        if (GameManager.Instance != null)
            SetArena(GameManager.Instance.selectedArenaIndex);
    }
} 