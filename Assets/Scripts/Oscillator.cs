using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Oscillator : MonoBehaviour {

    [SerializeField]
    Vector3 MovementVector = new Vector3(10f, 10f, 10f);

    [SerializeField]
    float Period = 2f;


    private Vector3 startingPosition;
	
	void Start () {
        startingPosition = transform.position;
	}
	
	
	void Update () {
        if (Period > Mathf.Epsilon)
        {
            float cycles = Time.time / Period;

            const float tau = Mathf.PI * 2;
            float rawSineWave = Mathf.Sin(cycles * tau);

            float movementFactor = (rawSineWave / 2f) + 0.5f;
            Vector3 offset = movementFactor * MovementVector;
            transform.position = startingPosition + offset;
        }
	}
}
