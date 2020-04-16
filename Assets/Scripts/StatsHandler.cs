using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class StatsHandler : MonoBehaviour, IPointerClickHandler
{
    private ShipStats selectedShip;

    // Start is called before the first frame update
    void Start()
    {
        selectedShip = GetComponent<ShipStats>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        GameManager.instance.GetCurrentShip(selectedShip, eventData.pointerPress.transform.parent);
        print("hi");
    }
}
