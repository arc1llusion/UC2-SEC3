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
                    state = State.Transcending;
                    Invoke("LoadNextLevel", ImpactDelay);
                    break;
                default:
                    state = State.Dying;
                    audioSource.Stop();
                    Invoke("LoadNextLevel", ImpactDelay);
                    break;
            }
        }
    }

    private void LoadNextLevel()
    {
        switch(state)
        {
            case State.Dying:
                SceneManager.LoadScene(0);
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
            rigidBody.AddRelativeForce(Vector3.up * MainThrust * Time.deltaTime);
            if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }
        }
        else
        {
            audioSource.Stop();
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
