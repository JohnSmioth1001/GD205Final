using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundVisual : MonoBehaviour {

    private const int SAMPLE_SIZE = 1024;

    public float rmsValue;
    public float dbValue;
    public float pitchValue;

    public float backgroundIntensity;
    public Material backgroundMaterial;
    public Color minColor;
    public Color maxColor;


    public int amnVisual = 64;

    public float maxVisualScale = 25f;
    public float visualModifier = 50f;
    public float smoothSpeed = 10f;
    public float keepPercent = 0.5f;

    private AudioSource source;
    private float[] samples;
    private float[] spectrum;
    private float sampleRate;


    private Transform[] visualList;
    private float[] visualScale;
    private int visualIndex;
    private int spectrumIndex;

    private void Start()
    {
        source = GetComponent<AudioSource>();
        samples = new float[SAMPLE_SIZE];
        spectrum = new float[SAMPLE_SIZE];
        sampleRate = AudioSettings.outputSampleRate;


       // SpawnLine();
        SpawnCircle();
    }

    private void SpawnLine()
    {

        visualScale = new float[amnVisual];
        visualList = new Transform[amnVisual];

        for(int i = 0; i < amnVisual; i++)
        {
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube) as GameObject;
            visualList[i] = go.transform;
            visualList[i].position = Vector3.right * i;


        }


    }

    private void SpawnCircle()
    {
        visualScale = new float[amnVisual];
        visualList = new Transform[amnVisual];

        Vector3 center = Vector3.zero;
        float radius = 10f;

        for( int i = 0; i < amnVisual; i++)
        {
            float angle = i * 1.0f / amnVisual;
            angle = angle * Mathf.PI * 2;

            float x = center.x + Mathf.Cos(angle) * radius;
            float y = center.y + Mathf.Sin(angle) * radius;

            Vector3 pos = center + new Vector3(x, y, 0);
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube) as GameObject;
            go.transform.position = pos;
            go.transform.rotation = Quaternion.LookRotation(Vector3.forward, pos);
            visualList[i] = go.transform;
        }

    }

    private void Update()
    {

        AnalyzeSound();
        UpdateVisual();
        UpdateBackground();

    }

    private void UpdateVisual()
    {
        int  visualIndex = 0;
        int  spectrumIndex = 0;
        int averageSize = (int)((SAMPLE_SIZE * keepPercent) / amnVisual);

        while (visualIndex < amnVisual)
        {
            int j = 0;
            float sum = 0;
            while(j < averageSize)
            {
                sum += spectrum[spectrumIndex];
                spectrumIndex++;
                j++;

            }

            float scaleY = sum / averageSize * visualModifier;

            visualScale[visualIndex] -= Time.deltaTime * smoothSpeed;
            if (visualScale[visualIndex] < scaleY)
                visualScale[visualIndex] = scaleY;

            if (visualScale[visualIndex] > maxVisualScale)
                visualScale[visualIndex] = maxVisualScale;


            visualList[visualIndex].localScale = Vector3.one + Vector3.up * visualScale[visualIndex];
            visualIndex++;

        }
    }

    private void UpdateBackground()
    {
        backgroundIntensity -= Time.deltaTime * 0.5f;
        if (backgroundIntensity < dbValue / 40)
            backgroundIntensity = dbValue / 40;

        backgroundMaterial.color = Color.Lerp(maxColor, minColor, -backgroundIntensity);
        

    }


    private void AnalyzeSound()
    {

        source.GetOutputData(samples, 0);

        //get the rms value
        int i = 0;
        float sum = 0;
        for (; i < SAMPLE_SIZE; i++)
        {

            sum = samples[i] * samples[i];

        }

        rmsValue = Mathf.Sqrt(sum / SAMPLE_SIZE);

        // get the dbvalue
        dbValue = 20 * Mathf.Log10(rmsValue / 0.1f);


        // get the sound spectrum
        source.GetSpectrumData(spectrum, 0, FFTWindow.BlackmanHarris);

        //find pitch
        float maxV = 0;
        var maxN = 0;
        for(i = 0; i < SAMPLE_SIZE; i++)
        {
            if (!(spectrum[i] > maxV) || !(spectrum[i] > 0.0f))
                continue;

            maxV = spectrum[i];
            maxN = 1;

        }

        float freqN = maxN;
        if(maxN > 0 && maxN < SAMPLE_SIZE - 1)
        {
            var dL = spectrum[maxN - 1] / spectrum[maxN];
            var dR = spectrum[maxN + 1] / spectrum[maxN];
            freqN += 0.5f * (dR * dR - dL * dL);

        }

        pitchValue = freqN * (sampleRate / 2) / SAMPLE_SIZE;

    }



}
