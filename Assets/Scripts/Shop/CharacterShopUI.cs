using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterShopUI : MonoBehaviour
{

    [Header("Layout Settings")]
    [SerializeField] float itemSpacing = .5f;
    float itemHeight;

    [SerializeField] ScrollRect scrollRect;

    [Header("UI Elements")]
    [SerializeField] GameObject ShopPanel;
    [SerializeField] Transform ShopMenu;
    [SerializeField] Transform ShopItemsContainer;
    [SerializeField] GameObject itemPrefab;

    [Space(20f)]
    [SerializeField] CharacterShopDatabase characterDB;

    Animator anim;




 

    int newSelectedItemIndex = 0;
    int previousSelectedItemIndex = 0;

    [System.Obsolete]
    void Start()
    {



        GenerateShopItemsUI();

        SetSelectedCharacter();

        SelectItemUI(GameDataManager.GetSelectedCharacterIndex());

        AutoScrollShopList(GameDataManager.GetSelectedCharacterIndex());
    }
    void AutoScrollShopList(int itemIndex)
    {
        //scrollRect.verticalNormalizedPosition = 0f; //means scroll to the bottom
        //scrollRect.verticalNormalizedPosition = 1f; //means scroll to the top
        scrollRect.horizontalNormalizedPosition = Mathf.Clamp01(1f - (itemIndex / (float)(characterDB.CharactersCount - 1)));
    }



    public void SetSelectedCharacter()
    {
        int index = GameDataManager.GetSelectedCharacterIndex();


        GameDataManager.SetSelectedCharacter(characterDB.GetCharacter(index), index);
        ChangePlayerSkin();


    }
    public void ChangePlayerSkin()
    {
        Character character = GameDataManager.GetSelectedCharacter();

        if (character.image != null)
        {

           // playerImage.sprite = character.image;

        }
    }
    [System.Obsolete]
    void GenerateShopItemsUI()
    {

        for (int i = 0; i < GameDataManager.GetAllPurchasedCharacter().Count; i++)
        {
            int purchasedCharacterIndex = GameDataManager.GetPurchasedCharacter(i);
            characterDB.PurchaseCharacter(purchasedCharacterIndex);
        }



        itemHeight = ShopItemsContainer.GetChild(0).GetComponent<RectTransform>().sizeDelta.y;
        Destroy(ShopItemsContainer.GetChild(0).gameObject);
        ShopItemsContainer.DetachChildren();


        for (int i = 0; i < characterDB.CharactersCount; i++)
        {
            Character character = characterDB.GetCharacter(i);
            CharacterItemUI uiItem = Instantiate(itemPrefab, ShopItemsContainer).GetComponent<CharacterItemUI>();

            uiItem.SetItemPosition(Vector2.down * i * (itemHeight + itemSpacing));


            //uiItem.SetCharacterName(character.name);
            uiItem.SetCharacterImage(character.image);
            //uiItem.SetCharacterFunInfo(character.funinfo);
            uiItem.SetCharacterPrice(character.price);


            if (character.isPurchased)
            {
                uiItem.SetCharacterAsPurchased();
                uiItem.OnItemSelect(i, OnItemSelected);
            }
            else
            {
               
                
                    uiItem.SetCharacterPrice(character.price);
                    uiItem.OnItemPurchase(i, OnItemPurchased);
                
       
            }

            ShopItemsContainer.GetComponent<RectTransform>().sizeDelta =
                Vector2.up * (itemHeight + itemSpacing) * characterDB.CharactersCount;



        }

    }



    void OnItemSelected(int index)
    {
        SelectItemUI(index);

        GameDataManager.SetSelectedCharacter(characterDB.GetCharacter(index), index);
        ChangePlayerSkin();




    }
    void SelectItemUI(int itemIndex)
    {
        previousSelectedItemIndex = newSelectedItemIndex;
        newSelectedItemIndex = itemIndex;

        CharacterItemUI prevUiItem = GetItemUI(previousSelectedItemIndex);
        CharacterItemUI newUiItem = GetItemUI(newSelectedItemIndex);

        prevUiItem.DeselectItem();
        newUiItem.SelectItem();

        anim = newUiItem.GetComponent<Animator>();
        anim.Play("ShopUICharacterAnimation");



        GameDataManager.SetSelectedCharacterIndex(newSelectedItemIndex);
    }
    CharacterItemUI GetItemUI(int index)
    {
        return ShopItemsContainer.GetChild(index).GetComponent<CharacterItemUI>();
    }

    void OnItemPurchased(int index)
    {
        Character character = characterDB.GetCharacter(index);
        CharacterItemUI uiItem = GetItemUI(index);

        if (GameDataManager.CanSpendCoins(character.price))
        {
            GameDataManager.SpendCoins(character.price);
            

            GameSharedUI.Instance.UpdateCoinsUIText();


            characterDB.PurchaseCharacter(index);

            uiItem.SetCharacterAsPurchased();
            uiItem.OnItemSelect(index, OnItemSelected);

            GameDataManager.AddPurchasedCharacter(index);


        }
      
  
        
       


    }


}
