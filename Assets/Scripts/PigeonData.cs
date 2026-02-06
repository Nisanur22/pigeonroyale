using UnityEngine;
using UnityEngine.UI;

public enum SkillType
{
    Damage,
    Heal
}

[System.Serializable]
public class PigeonData
{
    public string pigeonName;
    public int maxHealth;
    public int attackPower;      // Temel saldırı gücü
    public int skillPower;       // Skill gücü
    public SkillType skillType;   // Inspector'dan ayarlanabilir
    public GameObject pigeonPrefabRight; // sağa bakan prefab
    public GameObject pigeonPrefabLeft;  // sola bakan prefab
    // İstersen skill isimleri, açıklamaları, vs. de ekleyebilirsin
}
