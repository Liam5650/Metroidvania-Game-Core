using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HUDController : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI healthNumber, ammoNumber;

    public void UpdateHealth(float value)
    {
        healthNumber.text = value.ToString();
    }

    public void UpdateAmmo(int value)
    {
        ammoNumber.text = value.ToString();
    }
}
