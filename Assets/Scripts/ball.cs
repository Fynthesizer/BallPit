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

    SteamAudio.SteamAudioSource sas;

    public GameObject noteSelector;

    public GameObject soundObject;
    GameObject soundInstance;


    // Start is called before the first frame update
    void Start()
    {

        //collideInstance = FMODUnity.RuntimeManager.CreateInstance(CollideEvent);
        //FMODUnity.RuntimeManager.AttachInstanceToGameObject(collideInstance, GetComponent<Transform>(), GetComponent<Rigidbody>());
        //sas = gameObject.GetComponent<SteamAudio.SteamAudioSource>();
        //sas.uniqueIdentifier = gameObject.name;
        //emitter = gameObject.GetComponent<FMODUnity.StudioEventEmitter>();
        //SetPitch(pitch,timbre);
        //noteSelector = GameObject.FindGameObjectWithTag("NoteSelector");
        //Setup(24, 0, 0);
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

    public void Setup(float p, int t, int i)
    {
        //sas = gameObject.GetComponent<SteamAudio.SteamAudioSource>();
        //sas.uniqueIdentifier = gameObject.name;
        rb = gameObject.GetComponent<Rigidbody>();
        light = gameObject.GetComponent<Light>();
        noteSelector = GameObject.FindGameObjectWithTag("NoteSelector");
        mat = gameObject.GetComponent<Renderer>().material;

        pitch = p;
        timbre = t;
        gameObject.name = "Ball " + i;

        //sas = gameObject.GetComponent<SteamAudio.SteamAudioSource>();
        //sas.uniqueIdentifier = gameObject.name;
        FMODUnity.RuntimeManager.LoadBank(bankRef, true);
        //emitter = gameObject.GetComponent<FMODUnity.StudioEventEmitter>();
        collideInstance = FMODUnity.RuntimeManager.CreateInstance(CollideEvent);
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(collideInstance, GetComponent<Transform>(), GetComponent<Rigidbody>());
        collideInstance.setParameterByName("Pitch", p);
        collideInstance.setParameterByName("Timbre", t);

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
        collideInstance.setParameterByName("Radius", size/2);

        mat.SetColor("_Color", color);
        mat.SetFloat("Intensity", 0);
        //light.range = size * 25;
        light.color = color;

        
    }

    public void PlaySound(float p, float t, float r, float m)
    {
        //if (sas != null) Destroy(sas);
        //sas = gameObject.AddComponent<SteamAudio.SteamAudioSource>();
        //sas.uniqueIdentifier = "" + Time.time;
        if (soundInstance != null) Destroy(soundInstance);
        soundInstance = Instantiate(soundObject, transform);
        //soundInstance.GetComponent<FMODUnity.StudioEventEmitter>().EventInstance.setParameterByName("Pitch", p);
        //soundInstance.GetComponent<FMODUnity.StudioEventEmitter>().EventInstance.setParameterByName("Magnitude", m);
        //soundInstance.GetComponent<FMODUnity.StudioEventEmitter>().EventInstance.setParameterByName("Radius", r);
        //soundInstance.GetComponent<FMODUnity.StudioEventEmitter>().EventInstance.setParameterByName("Timbre", t);
        soundInstance.GetComponent<SteamAudio.SteamAudioSource>().uniqueIdentifier = "" + Time.time;
        //soundInstance.GetComponent<FMODUnity.StudioEventEmitter>().SetParameter("Pitch", p);
        soundInstance.GetComponent<FMODUnity.StudioEventEmitter>().Params[0].Value = m;
        soundInstance.GetComponent<FMODUnity.StudioEventEmitter>().Params[1].Value = p;
        soundInstance.GetComponent<FMODUnity.StudioEventEmitter>().Params[2].Value = r;
        soundInstance.GetComponent<FMODUnity.StudioEventEmitter>().Params[3].Value = t;
        //soundInstance.GetComponent<FMODUnity.StudioEventEmitter>().SetParameter("Magnitude", m);
        //soundInstance.GetComponent<FMODUnity.StudioEventEmitter>().SetParameter("Radius", r);
        //soundInstance.GetComponent<FMODUnity.StudioEventEmitter>().SetParameter("Timbre", t);
        soundInstance.GetComponent<FMODUnity.StudioEventEmitter>().Play();

        //collideInstance = FMODUnity.RuntimeManager.CreateInstance(CollideEvent);
        //FMODUnity.RuntimeManager.AttachInstanceToGameObject(collideInstance, GetComponent<Transform>(), GetComponent<Rigidbody>());
        //collideInstance.setParameterByName("Pitch", p);
        //collideInstance.setParameterByName("Timbre", t);
        //collideInstance.setParameterByName("Radius", r);
        //collideInstance.setParameterByName("Magnitude", m);
        //collideInstance.setParameterByName("Magnitude", m);
        //collideInstance.start();
        //collideInstance.release();


        //gameObject.GetComponent<AudioSource>().Play();
    }
}
