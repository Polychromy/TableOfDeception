using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

public class GuiliotineGIF : MonoBehaviour
{
    public GameObject[] frames;
    float framesPerSecond = 10;
    public GameObject guiliotine;
    
 
    public void Update() {
        int index  = (int)(Time.time * framesPerSecond) % frames.Length;
     
        guiliotine.GetComponent<Image>().sprite = frames[index].GetComponent<SpriteRenderer>().sprite;
    }
}
