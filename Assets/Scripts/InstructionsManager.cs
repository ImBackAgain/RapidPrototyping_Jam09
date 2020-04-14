using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InstructionsManager : MonoBehaviour
{
    [SerializeField] Button next, prev;

    [SerializeField] GameObject[] pages;
    int currrentPageNum, pageCount;

    private void Awake()
    {
        pageCount = pages.Length;
    }
    // Start is called before the first frame update
    void OnEnable()
    {
        currrentPageNum = 0;
        UpdatePage();
    }

    // Update is called once per frame
    void UpdatePage()
    {
        if (currrentPageNum >= pageCount || currrentPageNum < 0) throw new System.IndexOutOfRangeException("Whadddaya mean, turn to page " + currrentPageNum + "? There's only " + pageCount + " pages here!");

        for (int i = 0; i < pageCount; i++)
        {
            pages[i].SetActive(i == currrentPageNum);
        }

        prev.interactable = currrentPageNum != 0;
        next.interactable = currrentPageNum != pageCount - 1;
    }

    public void NextPage()
    {
        currrentPageNum++;
        UpdatePage();
    }

    public void PreviousPage()
    {
        currrentPageNum--;
        UpdatePage();
    }
}
