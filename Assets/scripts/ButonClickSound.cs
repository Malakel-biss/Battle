using UnityEngine;


public class ButtonClickSound : MonoBehaviour
{
    public AudioSource buttonClickSound; 

    void Update()
    {
      
        if (Input.GetMouseButtonDown(0))
        {
         
            if (buttonClickSound != null)
            {
                buttonClickSound.Play(); 
            }
            else
            {
                Debug.LogError("AudioSource non assigné à buttonClickSound !");
            }
        }
    }
}
