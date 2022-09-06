using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MenuManager : MonoBehaviour
{
    public static bool GameIsPaused = false;
    public GameObject pauseMenuUI;
    public GameObject ShopPanel;

    public GameObject SettingsPanel;
    public GameObject QualityButton;

    public int quantityStartItem = 0;
    public int qualityStartItem = 0;

    private List<StackItem> items = new List<StackItem>();
    public int ItemCount => items.Count;

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;



    }
    public void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;

    }

    public void OpenShop()
    {
        ShopPanel.SetActive(true);
    }
    public void IncreaaseQuantityStartItem()
    {
        quantityStartItem= PlayerPrefs.GetInt("quantityStartItem")+1;
        PlayerPrefs.SetInt("quantityStartItem",quantityStartItem);
        StackManager.Instance.StartQuantityWoods(1);
        Debug.Log("increased!");
    }
    //public void IncreaaseQualityStartItem()
    //{
     
    //        qualityStartItem = PlayerPrefs.GetInt("qualityStartItem")+1;
    //        PlayerPrefs.SetInt("qualityStartItem", qualityStartItem);

    //        StackManager.Instance.StartQualityWoods(PlayerPrefs.GetInt("qualityStartItem"));
        
       
    //}
    public void CloseShop()
    {
        ShopPanel.SetActive(false);
    }
    public void OpenShopInGame()
    {
        Pause();
        ShopPanel.SetActive(true);
    }
    public void OpenSettings()
    {
        SettingsPanel.SetActive(true);
    }
    public void CloseSettings()
    {
        SettingsPanel.SetActive(false);
    }
}
