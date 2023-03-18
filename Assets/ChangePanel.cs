 using UnityEngine;
 using UnityEngine.UI;
 using System.Collections;
 
 public class ChangePanel : MonoBehaviour
 {
     // Use this for initialization
     void Start () {
         Image img =  GameObject.Find("MyPanel").GetComponent<Image>();
         img.color = Color.red;
     }
 
 }