using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class ball : MonoBehaviour
{
    public AnimationCurve falloff;

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

    // Start is called before the first frame update
    void Start()
    {
        SetPitch(pitch,timbre);
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float decayRate = 0.025f / size;
        if (amplitude > 0) amplitude -= decayRate;
        else amplitude = 0;
        light.intensity = falloff.Evaluate(amplitude) * 25;
        light.range = falloff.Evaluate(amplitude) * 100 * size;
        mat.SetFloat("Intensity", falloff.Evaluate(amplitude) * 25);
    }

    private void OnCollisionEnter(Collision collision)
    {
        
        collideInstance = FMODUnity.RuntimeManager.CreateInstance(CollideEvent);
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(collideInstance, GetComponent<Transform>(), GetComponent<Rigidbody>());
        collideInstance.setParameterByName("Pitch", Mathf.Lerp(0, 48, Mathf.InverseLerp(0, 48, pitch)));
        collideInstance.setParameterByName("Timbre", timbre);
        collideInstance.setParameterByName("Radius", size / 2);
        //collideInstance = FMODUnity.RuntimeManager.CreateInstance(CollideEvent);
        //collideInstance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(transform.position));
        //FMODUnity.RuntimeManager.AttachInstanceToGameObject(collideInstance, GetComponent<Transform>(), GetComponent<Rigidbody>());
        collideInstance.setParameterByName("Magnitude", collision.impulse.magnitude);
        collideInstance.start();
        collideInstance.release();
        amplitude = Mathf.Lerp(0, 1, Mathf.InverseLerp(0, 25, collision.impulse.magnitude));
        print(collision.impulse.magnitude);
        
    }

    public void SetPitch(float p, int t)
    {
        rb = gameObject.GetComponent<Rigidbody>();
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
        print(size);
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
}
