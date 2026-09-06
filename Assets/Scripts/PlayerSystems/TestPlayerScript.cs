using UnityEngine;

public class TestPlayerScript : MonoBehaviour
{
    private void Awake()
    {
        Debug.Log("PLAYER AWAKE WORKING", this);
    }

    private void Start()
    {
        Debug.Log("PLAYER START WORKING", this);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            Debug.Log("T PRESSED", this);
        }
    }
}