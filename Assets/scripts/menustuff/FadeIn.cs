using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FadeIn : MonoBehaviour
{
    public float fadetime;
    public GameObject panel;
    private Image img;
    private float timer;
    private Color fade = Color.black;

    void Start()
    {
        panel.SetActive(true);
        img = panel.GetComponent<Image>();
        timer = fadetime;
    }
    void Update()
    {
        timer -= Time.deltaTime;
        fade.a =  (timer/fadetime);
        img.color = fade;
        if(timer<=0)
        {
            Destroy(this);
        }
    }


}