using UnityEngine;
using System.Collections.Generic;

public class GateLeverChecker : MonoBehaviour
{

    public List<Lever> levers = new List<Lever>();

    private Gate gate;

    private void Awake()
    {
        gate = GetComponent<Gate>();
    }

    private void Update()
    {
        if (!gate.closed) return;

        foreach(Lever lever in levers)
        {
            if (!lever.isActivated)
            {
                return;
            }
        }

        gate.TriggerGate();
    }
}
