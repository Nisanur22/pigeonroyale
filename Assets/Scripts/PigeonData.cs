using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class PigeonData
{
    public string pigeonName;
    public int maxHealth;
    public GameObject pigeonPrefabRight; // sağa bakan prefab
    public GameObject pigeonPrefabLeft;  // sola bakan prefab
    // İstersen skill isimleri, açıklamaları, vs. de ekleyebilirsin
}
