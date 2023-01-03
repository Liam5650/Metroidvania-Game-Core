using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HUDController : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI healthText;

    public void UpdateHealth(float value)
    {
        healthText.text = value.ToString();
    }
}
