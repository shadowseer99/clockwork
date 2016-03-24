using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class OptionsMenu : MonoBehaviour {
    public Slider master;
    public Slider music;
    public Slider effects;

    private float tmas = 1;
    private float tmus=1;
    private float teff=1;


    public void setSliders()
    {
        master.value = SoundProfile.master;
        music.value = SoundProfile.music;
        effects.value = SoundProfile.effects;
        tmas = SoundProfile.master;
        tmus = SoundProfile.music;
        teff = SoundProfile.effects;
    }

    public void setSounds()
    {
        SoundProfile.master = tmas;
        SoundProfile.music= tmus;
        SoundProfile.effects= teff;
    }
    public void slideChange(int slider)
    {
        if(slider==1)
        {
            SoundProfile.master = master.value;
        }
        else if(slider==2)
        {
            SoundProfile.music = music.value;
        }
        else if(slider==3)
        {
            SoundProfile.effects = effects.value;
        }
    }
}
