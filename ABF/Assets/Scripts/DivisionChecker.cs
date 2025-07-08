using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DivisionChecker : MonoBehaviour
{
    [SerializeField] private TMP_Text divisionName;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Division"))
        {
            divisionName.text = other.gameObject.name;
        }
    }
}
