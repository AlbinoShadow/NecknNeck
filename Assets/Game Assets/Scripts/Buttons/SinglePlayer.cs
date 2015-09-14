using UnityEngine;
using System.Collections;

public class SinglePlayer : MonoBehaviour {

    private int counter = 0;
    
    void OnClick ()
    {
        counter++;
        print ("Clicked " + counter.ToString () + " times.");
    }
}
