using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RocketMovement : MonoBehaviour
{
    [SerializeField] float rcsThrust = 150f;
    [SerializeField] float mainThrust = 150f;
    [SerializeField] int currentLevel = 0;
    [SerializeField] AudioClip mainEngine;
    [SerializeField] AudioClip success;
    [SerializeField] AudioClip explosion;
    [SerializeField] ParticleSystem mainEngineParticles;
    [SerializeField] ParticleSystem successParticles;
    [SerializeField] ParticleSystem explosionParticles;
    enum State { Alive, Dead, Loading }
    State state = State.Alive;
    new Rigidbody rigidbody;
    AudioSource audioSource;
       
    void Start () {
        rigidbody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {
        if (state == State.Alive) //TODO stop sound on death
        {
            Thrust();
            Rotate();
        }
        
	}
    void OnCollisionEnter(Collision collision)
    {
        if (state != State.Alive) { return; } //ignore colissions when dead
        switch (collision.gameObject.tag)
        {
            case "Friendly":
                StartSuccessSequence();
                break;
            case "Fuel":
                print("Loading fuel");
                break;
            default:
                StartDeathSequence();
                break;

        }
    }

    private void StartDeathSequence()
    {
        state = State.Dead;
        audioSource.Stop();
        audioSource.PlayOneShot(explosion);
        explosionParticles.Play();
        Invoke("ReloadScene", 1f);
    }

    private void StartSuccessSequence()
    {
        state = State.Loading;
        audioSource.Stop();
        audioSource.PlayOneShot(success);
        successParticles.Play();
        Invoke("LoadNextScene", 1f); //parametirize
    }

    private void ReloadScene()
    {
        SceneManager.LoadScene(currentLevel);
    }

    private void LoadNextScene()
    {
        currentLevel++;
        SceneManager.LoadScene(currentLevel);
    }

    private void Rotate()
    {
        
        float rotationOnThisFrame = rcsThrust * Time.deltaTime; //rotation speed based on fps
        rigidbody.freezeRotation = true; // take paralyzed control
        if (Input.GetKey(KeyCode.A))
        {
            
            transform.Rotate(Vector3.forward * rotationOnThisFrame);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(Vector3.back * rotationOnThisFrame);
        }
        

        rigidbody.freezeRotation = false; //resume control
    }

    private void Thrust()
    {
        if (Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.W))
        {
            Trusterino();

        }
        else
        {
            audioSource.Stop();
            mainEngineParticles.Stop();
        }
    }

    private void Trusterino()
    {
        rigidbody.AddRelativeForce(Vector3.up * mainThrust);
        if (!audioSource.isPlaying) //playing Audio Source
        {
            audioSource.PlayOneShot(mainEngine);
        }
        mainEngineParticles.Play();
    }
}
    