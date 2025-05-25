using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public int selectedArenaIndex = 0;

    // Oyun durumları
    public enum GameState
    {
        MainMenu,
        ArenaSelection,
        CharacterSelection,
        Battle,
        GameOver
    }

    public GameState CurrentState { get; private set; }

    // Arena seçenekleri
    public enum ArenaType
    {
        Sky,
        City,
        Mountain
    }

    public ArenaType SelectedArena { get; private set; }

    // Oyuncu verileri
    [System.Serializable]
    public class PlayerData
    {
        public int selectedCharacterIndex;
        public int selectedCharacterIndex2; // İkinci güvercin için
        public int health;
        public int maxHealth;
        public List<int> availableSkills;
        public bool isSecondCharacterSelected; // İkinci güvercin seçildi mi
    }

    public PlayerData[] players = new PlayerData[2];

    // Pigeon verileri
    [Header("Pigeon Data")]
    public PigeonData[] pigeons;

    // UI referansları
    [Header("UI Panels")]
    public GameObject mainMenuPanel;
    public GameObject arenaSelectionPanel;
    public GameObject characterSelectionPanel;
    public GameObject battlePanel;
    public GameObject gameOverPanel;

    public bool[] isPlayerReady = new bool[2];

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

        }
        else
        {
            Destroy(gameObject);
            Debug.Log("GameManager Desstroyed");
        }

        // Oyuncu verilerini başlat
        for (int i = 0; i < players.Length; i++)
        {
            players[i] = new PlayerData
            {
                selectedCharacterIndex = -1,
                selectedCharacterIndex2 = -1,
                health = 100,
                maxHealth = 100,
                availableSkills = new List<int> { 0, 1, 2 }, // 3 temel yetenek
                isSecondCharacterSelected = false
            };
        }
    }

    private void Start()
    {
        SetGameState(GameState.MainMenu);
    }

    public void SetGameState(GameState newState)
    {
        CurrentState = newState;
        
        // UI panellerini güncelle
        mainMenuPanel.SetActive(newState == GameState.MainMenu);
        arenaSelectionPanel.SetActive(newState == GameState.ArenaSelection);
        characterSelectionPanel.SetActive(newState == GameState.CharacterSelection);
        battlePanel.SetActive(newState == GameState.Battle);
        gameOverPanel.SetActive(newState == GameState.GameOver);

        // Duruma göre ek işlemler
        switch (newState)
        {
            case GameState.MainMenu:
                ResetGame();
                break;
            case GameState.Battle:
                StartBattle();
                break;
            case GameState.GameOver:
                ShowGameResults();
                break;
        }
    }

    public void SelectArena(ArenaType arena)
    {
        SelectedArena = arena;
        SetGameState(GameState.CharacterSelection);
    }

    public void SelectCharacter(int playerIndex, int characterIndex)
    {
        if (playerIndex >= 0 && playerIndex < players.Length)
        {
            if (!players[playerIndex].isSecondCharacterSelected)
            {
                players[playerIndex].selectedCharacterIndex = characterIndex;
                players[playerIndex].isSecondCharacterSelected = true;
            }
            else
            {
                players[playerIndex].selectedCharacterIndex2 = characterIndex;
            }
        }
    }

    private void StartBattle()
    {
        // Savaş başlangıç ayarları
        players[0].health = players[0].maxHealth;
        players[1].health = players[1].maxHealth;
    }

    private void ShowGameResults()
    {
        // Oyun sonu ekranını güncelle
    }

    private void ResetGame()
    {
        // Oyun verilerini sıfırla
        for (int i = 0; i < players.Length; i++)
        {
            players[i].selectedCharacterIndex = -1;
            players[i].selectedCharacterIndex2 = -1;
            players[i].health = players[i].maxHealth;
            players[i].isSecondCharacterSelected = false;
            isPlayerReady[i] = false;
        }
    }

    // UI butonları için public metodlar
    public void OnPlayButtonClicked()
    {
        SetGameState(GameState.ArenaSelection);
    }

    public void OnArenaSelected(int arenaIndex)
    {
        SelectArena((ArenaType)arenaIndex);
    }

    public void OnCharacterSelectedPlayer1(int characterIndex)
    {
        Debug.Log("Player 1 seçimi: " + characterIndex);
        players[0].selectedCharacterIndex = characterIndex;
    }

    public void OnCharacterSelectedPlayer2(int characterIndex)
    {
        Debug.Log("Player 2 seçimi: " + characterIndex);
        players[1].selectedCharacterIndex = characterIndex;
    }

    public void OnCharacterSelected(int playerIndex, int characterIndex)
    {
        SelectCharacter(playerIndex, characterIndex);
    }

    public void OnReturnToMainMenu()
    {
        SetGameState(GameState.MainMenu);
    }

    public void OnPlayerReady(int playerIndex)
    {
        isPlayerReady[playerIndex] = true;
        // Her iki oyuncu da hazırsa savaşa geç
        if (isPlayerReady[0] && isPlayerReady[1])
        {
            SetGameState(GameState.Battle);
        }
    }
}

