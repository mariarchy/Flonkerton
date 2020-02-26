using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinScript : MonoBehaviour
{
    public int speed;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void Update()
    {
        // Rotate animation for the coin
        this.transform.Rotate(new Vector3(speed * Time.fixedDeltaTime, 0, 0));
    }
}
