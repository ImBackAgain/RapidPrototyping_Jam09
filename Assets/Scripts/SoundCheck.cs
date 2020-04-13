using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SoundCheck : MonoBehaviour
{
    // Public Properties
    public List<InputTestEvent> testEvents;

    // Start()
    private void Start()
    {
        if (testEvents == null || testEvents.Count == 0)
        {
            Debug.LogWarning("\t[ InputTestEvent ] list of [ InputTester ] is empty!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        foreach(InputTestEvent testEvent in testEvents)
        {
            if (Input.GetKeyDown(testEvent.key))
            {
                testEvent.events.Invoke();
            }
        }
    }
}

[System.Serializable]
public class InputTestEvent
{
    // Public Properties
    public KeyCode key;
    public UnityEvent events;
}