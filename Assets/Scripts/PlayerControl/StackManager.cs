using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class StackManager : Singleton<StackManager>
{
    [SerializeField] public Transform stackRoot;
    [SerializeField] private List<Transform> spreadPoints;

    private List<StackItem> items = new List<StackItem>();
    public int ItemCount => items.Count;
    [SerializeField] private float itemDeltaPosZ = 1.25f;

    [SerializeField] public GameObject wood_Prefab;
    public int CurrentStackValue => currentStackValue;
    private int currentStackValue = 0;

    [Header("Menu Upgrades")]
    
    
    // animation
    private bool animationPerforming = false;

 
    public void StartQuantityWoods(int startQuantityWoods)
    {
        if (startQuantityWoods == 0)
        {
            startQuantityWoods = PlayerPrefs.GetInt("quantityStartItem");
        }


        for (int i = 0; i <startQuantityWoods; i++)
        {
            if (items.Count > 0)
            {
                Instantiate(wood_Prefab, items[ItemCount - 1].transform.position, Quaternion.identity);
            }
            else
            {
                Instantiate(wood_Prefab, stackRoot.position, Quaternion.identity);
            }
        }


    }
    //public void StartQualityWoods(int startQualityWoods)
    //{
    //    Debug.Log("currentStackQualityValue:" + currentStackQualityValue);
    //    Debug.Log("stackk" + startQualityWoods);
        


    //    for (int i = 0; i < items.Count;)
    //    {

    //        for (int j = 0 ; j < 1 && currentStackQualityValue < startQualityWoods; j++)
    //        {
    //            currentStackQualityValue++;
    //            items[i].Improve();
                
    //            //Debug.Log("stackk" + startQualityWoods);
    //            //Debug.Log("i:" + i);
    //            //Debug.Log("currentStackQualityValue:" + currentStackQualityValue);
               
    //        }
    //        i++;

    //    }

    //}
    public void AddItem(StackItem item)
    {
        if(items.Count > 0)
        {
            item.SetTarget(items[ItemCount - 1].transform, itemDeltaPosZ);
        }
        else// first item to add
        {
            item.SetTarget(stackRoot, itemDeltaPosZ);
        }

        items.Add(item);
        currentStackValue += item.Level;
        
        if(!animationPerforming)
        {
            animationPerforming = true;
            StartCoroutine(PerformCollectAnim());
        }       
    }
    

    public void DestroyItem(StackItem collisionItem)
    {
        int collisionIndex = items.IndexOf(collisionItem);

        if (collisionIndex == ItemCount - 1)// last item collided with an obstacle
        {
            currentStackValue -= collisionItem.Level;
            items.RemoveAt(ItemCount - 1);
            Destroy(collisionItem.gameObject);            
        }
        else// horizontal collision
        {
            for (int i = collisionIndex; i < ItemCount; i++)
            {
                items[i].Throw(spreadPoints[i].position);
                currentStackValue -= items[i].Level;
            }

            items.RemoveRange(collisionIndex, ItemCount - collisionIndex);
        }
    }

    public void DepositItem(StackItem depositItem)
    {
        MoneyManager.Instance.Deposit(depositItem.Level);
        DestroyItem(depositItem);
    }

    public void UpdateStackValue()
    {
        currentStackValue++;
    }

    private IEnumerator PerformCollectAnim()
    {
        for (int i = ItemCount - 1; i >= 0; i--)
        {
            if(items[i] == null)
            {
                break;
            }

            items[i].transform.DOPunchScale(Vector3.one, 0.2f, 2, 1f);

            yield return new WaitForSeconds(0.05f);
        }

        animationPerforming = false;
    }
}
