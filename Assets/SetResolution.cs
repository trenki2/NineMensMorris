using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetResolution : MonoBehaviour
{
    void Update()
    {
        Screen.SetResolution(960, 540, false);
    }
}
