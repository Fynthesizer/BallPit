using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class ball : MonoBehaviour
{
    public bool init = false;

    public AnimationCurve falloff;

    public float lightMultiplier = 50;

    public float pitch = 10;
    public float density = 2f;
    public int timbre = 0;
    float size;
    Color color;

    Rigidbody rb;
    Light light;
    public float amplitude = 0;

    public Material mat;

    [FMODUnity.EventRef]
    public string CollideEvent = "";
    FMOD.Studio.EventInstance collideInstance;

    FMODUnity.StudioEventEmitter emitter;

    [FMODUnity.BankRef]
    public string bankRef;

    SteamAudio.SteamAudioSource sac;

    public GameObject noteSelector;


    // Start is called before the first frame update
    void Start()
    {
        //sac = gameObject.AddComponent<SteamAudio.SteamAudioSource>();
        //sac.uniqueIdentifier = gameObject.name;
        //FMODUnity.RuntimeManager.LoadBank(bankRef, true);
        //collideInstance = FMODUnity.RuntimeManager.CreateInstance(CollideEvent);
        //FMODUnity.RuntimeManager.AttachInstanceToGameObject(collideInstance, GetComponent<Transform>(), GetComponent<Rigidbody>());
        //sac = gameObject.GetComponent<SteamAudio.SteamAudioSource>();
        //sac.uniqueIdentifier = gameObject.name;
        //emitter = gameObject.GetComponent<FMODUnity.StudioEventEmitter>();
        //SetPitch(pitch,timbre);
        //noteSelector = GameObject.FindGameObjectWithTag("NoteSelector");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float decayRate = 0.02f / size;
        if (amplitude > 0) amplitude -= decayRate;
        else amplitude = 0;
        light.intensity = falloff.Evaluate(amplitude) * lightMultiplier;
        light.range = falloff.Evaluate(amplitude) * lightMultiplier * size;
        mat.SetFloat("Intensity", falloff.Evaluate(amplitude) * 25);
    }

    private void OnCollisionEnter(Collision collision)
    {
        
        //collideInstance = FMODUnity.RuntimeManager.CreateInstance(CollideEvent);
        //FMODUnity.RuntimeManager.AttachInstanceToGameObject(collideInstance, GetComponent<Transform>(), GetComponent<Rigidbody>());
        //collideInstance.setParameterByName("Pitch", Mathf.Lerp(0, 48, Mathf.InverseLerp(0, 48, pitch)));
        //collideInstance.setParameterByName("Timbre", timbre);
        //collideInstance.setParameterByName("Radius", size / 2);
        //collideInstance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(transform.position));
        //collideInstance.setParameterByName("Magnitude", collision.impulse.magnitude);
        //collideInstance.start();
        //collideInstance.release();

        PlaySound(pitch, timbre, size / 2, collision.impulse.magnitude);
        //collideInstance = FMODUnity.RuntimeManager.CreateInstance(CollideEvent);
        //FMODUnity.RuntimeManager.AttachInstanceToGameObject(collideInstance, GetComponent<Transform>(), GetComponent<Rigidbody>());

        amplitude = Mathf.Lerp(0, 1, Mathf.InverseLerp(0, 25, collision.impulse.magnitude));
        noteSelector.GetComponent<noteSelector>().HighlightNote(Mathf.RoundToInt(pitch),amplitude);
        
    }

    public void Setup(float p, int t)
    {
        sac = gameObject.GetComponent<SteamAudio.SteamAudioSource>();
        sac.uniqueIdentifier = gameObject.name;
        emitter = gameObject.GetComponent<FMODUnity.StudioEventEmitter>();
        rb = gameObject.GetComponent<Rigidbody>();
        noteSelector = GameObject.FindGameObjectWithTag("NoteSelector");

        pitch = p;
        timbre = t;

        float hue = 0;
        if (pitch < 12) hue = Mathf.Lerp(0, 1, Mathf.InverseLerp(0, 12, pitch));
        else if (pitch < 24) hue = Mathf.Lerp(0, 1, Mathf.InverseLerp(12, 24, pitch));
        else if (pitch < 36) hue = Mathf.Lerp(0, 1, Mathf.InverseLerp(24, 36, pitch));
        else if (pitch < 48) hue = Mathf.Lerp(0, 1, Mathf.InverseLerp(36, 48, pitch));
        color = Color.HSVToRGB(hue, 1, 1);

        float freq = 440 * Mathf.Pow(2, (pitch - 48) / 12);
        size = Mathf.Pow(2.5f/Mathf.Pow(freq/72.6f,2f), 1f / 3f);
        float vol = (4 / 3) * Mathf.PI * Mathf.Pow((size / 2), 3);
        rb.mass = ((vol * density)+1)/2;
        //print(rb.mass);
        //rb.mass = 1;

        //size = 1f / (pitch / 12);
        transform.localScale = new Vector3(size, size, size);

        mat = gameObject.GetComponent<Renderer>().material;
        mat.SetColor("_Color", color);
        mat.SetFloat("Intensity", 0);
        light = transform.GetChild(0).GetComponent<Light>();
        //light.range = size * 25;
        light.color = color;

        
    }

    public void PlaySound(float p, float t, float r, float m)
    {
        collideInstance = FMODUnity.RuntimeManager.CreateInstance(CollideEvent);
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(collideInstance, GetComponent<Transform>(), GetComponent<Rigidbody>());
        collideInstance.setParameterByName("Pitch", p);
        collideInstance.setParameterByName("Timbre", t);
        collideInstance.setParameterByName("Radius", r);
        collideInstance.setParameterByName("Magnitude", m);
        collideInstance.start();
        collideInstance.release();
    }
}
