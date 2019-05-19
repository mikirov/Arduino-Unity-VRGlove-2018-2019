using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseInputController : MonoBehaviour
{
    public abstract List<int> GetValues();
}
