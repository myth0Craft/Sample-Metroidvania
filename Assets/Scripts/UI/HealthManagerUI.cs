using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthManagerUI : MonoBehaviour
{
    [SerializeField] private Sprite fullHeartSprite;
    [SerializeField] private Sprite emptyHeartSprite;
    [SerializeField] private GameObject heartPrefab;
    [SerializeField] private PlayerHealthManager playerHealth;

    private List<Image> heartImages = new();

    void Start()
    {
        CreateHearts();
        UpdateHearts();
    }

    void OnEnable()
    {
        if (playerHealth != null)
            playerHealth.OnHealthChanged += UpdateHearts;
    }

    void OnDisable()
    {
        if (playerHealth != null)
            playerHealth.OnHealthChanged -= UpdateHearts;
    }

    void CreateHearts()
    {
        foreach (Transform child in transform)
            Destroy(child.gameObject);

        heartImages.Clear();

        for (int i = 0; i < playerHealth.getMaxHealth(); i++)
        {
            GameObject heart = Instantiate(heartPrefab, transform);
            Image img = heart.GetComponent<Image>();
            heartImages.Add(img);
            
        }
    }

    public void UpdateHearts()
    {
        for (int i = 0; i < heartImages.Count; i++)
        {
            if (i < playerHealth.currentHealth)
                heartImages[i].sprite = fullHeartSprite;
            else
                heartImages[i].sprite = emptyHeartSprite;
        }
    }
}
