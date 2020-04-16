using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DockHandler : MonoBehaviour, IPointerClickHandler
{
    public bool isDefault;

    public void OnPointerClick(PointerEventData eventData)
    {
        GameManager.instance.GetCurrentShip(transform.parent.GetComponentInChildren<ShipStats>(), transform.parent);
        //print("hei");
    }

    //public void TurnOnDefault()
    //{
    //    if (isDefault)
    //    {
    //        GameManager.instance.GetCurrentShip(transform.parent.GetComponentInChildren<ShipStats>(), transform.parent);
    //    }
    //}
}
