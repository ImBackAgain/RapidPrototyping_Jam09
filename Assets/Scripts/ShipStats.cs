using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipStats : MonoBehaviour
{
    //Name of the ship
    public string model;

    //Value of the ship
    public float value;

    //The five stats for determining the selling price
    public int appearance;
    public int interior;
    public int safety;
    public int speed;
    public enum SizeCategory { Small, Regular, Large }
    public SizeCategory size;

    // Saved for future use of special ship characteristics
    public enum ShipModifier { None }
    public ShipModifier modifier = ShipModifier.None;

    // Start is called before the first frame update
    void Start()
    {
        // All values are set in the Inspector
    }

    public string GenerateTooltip()
    {
        string tooltip;

        tooltip = size + " " + model + " Class" +
                    "\n\n Appearance: " + appearance +
                    "\n Interior: " + interior +
                    "\n Safety: " + safety +
                    "\n Speed: " + speed +
                    "\n\n Total Value: " + value;

        return tooltip;

    }
}
