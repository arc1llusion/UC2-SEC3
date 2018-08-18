using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour
{
    [SerializeField]
    float MainThrust = 200;

    [SerializeField]
    float RCSThrust = 50;

    [SerializeField]
    float ImpactDelay = 1;

    [SerializeField]
    AudioClip mainThrustSound;

    [SerializeField]
    AudioClip successSound;

    [SerializeField]
    AudioClip deathSound;

    [SerializeField]
    ParticleSystem mainThrustParticles;

    [SerializeField]
    ParticleSystem successParticles;

    [SerializeField]
    ParticleSystem deathParticles;

    private Rigidbody rigidBody;
    private AudioSource audioSource;

    [SerializeField]
    GameObject launchPoint;

    enum State {  Alive, Dying, Transcending }
    State state = State.Alive;

    // Use this for initialization
    public void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    public void Update()
    {
        if (state == State.Alive)
        {
            Thrust();
            Rotate();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (state == State.Alive)
        {
            switch (collision.gameObject.tag)
            {
                case "Friendly":
                    break;
                case "Finish":
                    StartFinishSequence();
                    break;
                default:
                    StartDeathSequence();
                    break;
            }
        }
    }

    private void StartDeathSequence()
    {
        state = State.Dying;
        audioSource.Stop();
        mainThrustParticles.Stop();
        deathParticles.Play();
        audioSource.Stop();
        audioSource.PlayOneShot(deathSound, .25f);
        Invoke("LoadNextLevel", ImpactDelay);
    }

    private void StartFinishSequence()
    {
        state = State.Transcending;
        audioSource.PlayOneShot(successSound);
        successParticles.Play();
        Invoke("LoadNextLevel", ImpactDelay);
    }

    private void LoadNextLevel()
    {
        switch(state)
        {
            case State.Dying:
                transform.position = launchPoint.transform.position + new Vector3(0, 1.28f, 0);
                transform.rotation = Quaternion.identity;
                rigidBody.velocity = Vector3.zero;
                state = State.Alive;
                //SceneManager.LoadScene(0);
                break;
            case State.Transcending:
                SceneManager.LoadScene((SceneManager.GetActiveScene().buildIndex + 1) % SceneManager.sceneCountInBuildSettings);
                break;
        }
    }

    private void Thrust()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            mainThrustParticles.Play();
            ApplyThrust();
        }
        else
        {
            audioSource.Stop();
            mainThrustParticles.Stop();
        }
    }

    private void ApplyThrust()
    {
        rigidBody.AddRelativeForce(Vector3.up * MainThrust * Time.deltaTime);
        if (!audioSource.isPlaying)
        {
            audioSource.PlayOneShot(mainThrustSound);
        }
    }

    private void Rotate()
    {
        rigidBody.freezeRotation = true;

        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(Vector3.forward * RCSThrust * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(Vector3.back * RCSThrust * Time.deltaTime);
        }

        rigidBody.freezeRotation = false;
    }
}
