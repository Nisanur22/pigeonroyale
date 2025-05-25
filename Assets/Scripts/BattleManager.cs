using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BattleManager : MonoBehaviour
{
    public Transform player1Area;
    public Transform player2Area;
    public Slider player1HealthBar;
    public Slider player2HealthBar;
    public Text turnIndicatorText;
    public Button attackButton;
    public Button skill1Button;

    [Header("Hit Bar System")]
    public GameObject hitBarPanel;
    public RectTransform hitBarArea;
    public RectTransform hitBarLine;
    public float hitBarSpeed = 1f;
    public float criticalZoneSize = 0.2f;
    public float successZoneSize = 0.4f;

    [Header("End Game UI")]
    public GameObject endGamePanel;
    public Text gameOverText;

    private int currentPlayerTurn = 0; // 0: Player1, 1: Player2
    private int[] playerHealth = new int[2];
    private int maxHealth = 100;
    private bool battleEnded = false;

    private GameObject player1PigeonInstance;
    private GameObject player2PigeonInstance;

    Animator player1Animator;
    Animator player2Animator;

    private bool isHitBarActive = false;
    private float currentLinePos = 0f;
    private int pendingAttackType = 0; // 0: attack, 1: skill1
    private float hitBarDirection = 1f; // 1: sağa, -1: sola

    private int[] criticalCount = new int[2]; // Her oyuncu için kritik sayısı

    void Start()
    {
        Debug.Log("BattleManager Start called");

        var gm = GameManager.Instance;
        if (gm == null)
        {
            Debug.LogError("GameManager.Instance is null!");
            return;
        }

        int p1Index = gm.players[0].selectedCharacterIndex;
        int p2Index = gm.players[1].selectedCharacterIndex;
        Debug.Log($"Player indices - P1: {p1Index}, P2: {p2Index}");

        if (p1Index < 0 || p1Index >= gm.pigeons.Length || p2Index < 0 || p2Index >= gm.pigeons.Length)
        {
            Debug.LogError($"Invalid pigeon indices! P1: {p1Index}, P2: {p2Index}, Total pigeons: {gm.pigeons.Length}");
            return;
        }

        var p1Data = gm.pigeons[p1Index];
        var p2Data = gm.pigeons[p2Index];

        if (player1PigeonInstance != null) Destroy(player1PigeonInstance);
        if (player2PigeonInstance != null) Destroy(player2PigeonInstance);

        // Yönüne uygun prefabları kullan
        player1PigeonInstance = Instantiate(
            p1Data.pigeonPrefabRight, // sağa bakan prefab
            player1Area.position,
            Quaternion.identity,
            player1Area
        );
        player2PigeonInstance = Instantiate(
            p2Data.pigeonPrefabLeft, // sola bakan prefab
            player2Area.position,
            Quaternion.identity,
            player2Area
        );

        player1Animator = player1PigeonInstance.GetComponent<Animator>();
        player2Animator = player2PigeonInstance.GetComponent<Animator>();

        // Başlangıç değerleri
        maxHealth = p1Data.maxHealth;
        playerHealth[0] = p1Data.maxHealth;
        playerHealth[1] = p2Data.maxHealth;
        player1HealthBar.maxValue = p1Data.maxHealth;
        player2HealthBar.maxValue = p2Data.maxHealth;
        player1HealthBar.value = p1Data.maxHealth;
        player2HealthBar.value = p2Data.maxHealth;
        UpdateTurnIndicator();
        EnableActionButtons(true);
    }

    public void OnAttackButton()
    {
        if (battleEnded) return;
        pendingAttackType = 0;
        StartHitBar();
        EnableActionButtons(false);
    }

    public void OnSkill1Button()
    {
        if (battleEnded) return;
        if (!skill1Button.interactable) return; // Skill açılmadıysa kullanılamaz
        pendingAttackType = 1;
        StartHitBar();
        EnableActionButtons(false);
        criticalCount[currentPlayerTurn] = 0; // Skill kullanınca sayaç sıfırlanır
        skill1Button.interactable = false; // Skill tekrar pasif olur
    }

    void StartHitBar()
    {
        isHitBarActive = true;
        hitBarPanel.SetActive(true);
        currentLinePos = 0f;
        hitBarDirection = 1f; // Her seferinde sağa başlasın
        UpdateHitBarLine(0f);
    }

    void EndHitBar()
    {
        isHitBarActive = false;
        hitBarPanel.SetActive(false);

        int target = (currentPlayerTurn == 0) ? 1 : 0;
        int attackerIndex = currentPlayerTurn;
        var gm = GameManager.Instance;
        var attackerData = gm.pigeons[gm.players[attackerIndex].selectedCharacterIndex];

        if (pendingAttackType == 0)
        {
            // Attack
            int damage = CalculateDamage();
            playerHealth[target] -= damage;
            if (playerHealth[target] < 0) playerHealth[target] = 0;
        }
        else
        {
            // Skill
            if (attackerData.skillType == SkillType.Heal)
            {
                // Sebap kuşu: kendine can bas
                playerHealth[attackerIndex] += CalculateSkillEffect();
                if (playerHealth[attackerIndex] > attackerData.maxHealth)
                    playerHealth[attackerIndex] = attackerData.maxHealth;
            }
            else
            {
                // Diğer kuşlar: hasar ver
                int damage = CalculateSkillEffect();
                playerHealth[target] -= damage;
                if (playerHealth[target] < 0) playerHealth[target] = 0;
            }
        }

        UpdateHealthBars();

        // Animasyon tetikle (mevcut kodun aynısı)
        if (pendingAttackType == 0)
        {
            if (currentPlayerTurn == 0)
                player1Animator.SetTrigger("attack");
            else
                player2Animator.SetTrigger("attack");
        }
        else
        {
            if (currentPlayerTurn == 0)
                player1Animator.SetTrigger("skill1");
            else
                player2Animator.SetTrigger("skill1");
        }

        CheckBattleEnd();
        if (!battleEnded) NextTurn();
    }

    void UpdateHitBarLine(float value)
    {
        float width = hitBarArea.rect.width;
        Vector2 anchoredPos = hitBarLine.anchoredPosition;
        anchoredPos.x = (value - 0.5f) * width;
        hitBarLine.anchoredPosition = anchoredPos;
    }

    int CalculateDamage()
    {
        float value = currentLinePos;
        int attackerIndex = currentPlayerTurn;
        var gm = GameManager.Instance;
        var attackerData = gm.pigeons[gm.players[attackerIndex].selectedCharacterIndex];

        int baseDamage = attackerData.attackPower;

        // Kritik (yeşil)
        if (value > 0.5f - criticalZoneSize / 2f && value < 0.5f + criticalZoneSize / 2f)
        {
            criticalCount[attackerIndex]++;
            if (criticalCount[attackerIndex] >= 2)
                skill1Button.interactable = true;
            return Mathf.RoundToInt(baseDamage * 1.5f);
        }
        // Başarılı (sarı)
        else if ((value > 0.5f - successZoneSize / 2f && value < 0.5f - criticalZoneSize / 2f) ||
                 (value > 0.5f + criticalZoneSize / 2f && value < 0.5f + successZoneSize / 2f))
        {
            return baseDamage;
        }
        // Başarısız (turuncu/kırmızı)
        else
        {
            return 0;
        }
    }

    int CalculateSkillEffect()
    {
        int attackerIndex = currentPlayerTurn;
        var gm = GameManager.Instance;
        var attackerData = gm.pigeons[gm.players[attackerIndex].selectedCharacterIndex];

        if (attackerData.skillType == SkillType.Heal)
        {
            // Sebap kuşu: kendine can basacak
            return attackerData.skillPower;
        }
        else
        {
            // Diğer kuşlar: hasar verecek
            float value = currentLinePos;
            int baseDamage = attackerData.skillPower;

            if (value > 0.5f - criticalZoneSize / 2f && value < 0.5f + criticalZoneSize / 2f)
                return Mathf.RoundToInt(baseDamage * 1.5f);
            else if ((value > 0.5f - successZoneSize / 2f && value < 0.5f - criticalZoneSize / 2f) ||
                     (value > 0.5f + criticalZoneSize / 2f && value < 0.5f + successZoneSize / 2f))
                return baseDamage;
            else
                return 0;
        }
    }

    void NextTurn()
    {
        currentPlayerTurn = 1 - currentPlayerTurn;
        UpdateTurnIndicator();
        attackButton.interactable = true;
        skill1Button.interactable = (criticalCount[currentPlayerTurn] >= 2);
    }

    void UpdateTurnIndicator()
    {
        turnIndicatorText.text = (currentPlayerTurn == 0) ? "1. Oyuncunun Sırası" : "2. Oyuncunun Sırası";
    }

    void UpdateHealthBars()
    {
        player1HealthBar.value = playerHealth[0];
        player2HealthBar.value = playerHealth[1];
    }

    void CheckBattleEnd()
    {
        if (playerHealth[0] <= 0 || playerHealth[1] <= 0)
        {
            battleEnded = true;
            EnableActionButtons(false);

            // Die animasyonunu tetikle
            if (playerHealth[0] <= 0)
                player1Animator.SetTrigger("die");
            if (playerHealth[1] <= 0)
                player2Animator.SetTrigger("die");

            string resultText = "Game Over";
            if (playerHealth[0] <= 0 && playerHealth[1] <= 0)
                resultText = "Berabere!";
            else if (playerHealth[0] <= 0)
                resultText = "2. Oyuncu Kazandı!";
            else
                resultText = "1. Oyuncu Kazandı!";

            GameManager.Instance.ShowGameOver(resultText);
        }
    }

    void EnableActionButtons(bool enable)
    {
        attackButton.interactable = enable;
        skill1Button.interactable = enable;
    }

    void Update()
    {
        if (isHitBarActive)
        {
            currentLinePos += Time.deltaTime * hitBarSpeed * hitBarDirection;

            // Sınırları kontrol et ve yönü ters çevir
            if (currentLinePos > 1f)
            {
                currentLinePos = 1f;
                hitBarDirection = -1f;
            }
            else if (currentLinePos < 0f)
            {
                currentLinePos = 0f;
                hitBarDirection = 1f;
            }

            UpdateHitBarLine(currentLinePos);

            if (Input.GetKeyDown(KeyCode.Space))
            {
                EndHitBar();
            }
        }
    }
} 