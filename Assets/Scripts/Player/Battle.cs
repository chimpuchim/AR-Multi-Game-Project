using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;
using UnityEngine.Serialization;

public class Battle : MonoBehaviourPun
{
    [SerializeField] private SpinController spinnerScript;

    [SerializeField] private GameObject uI3DGameobject;
    [SerializeField] private GameObject deathPanelUIPrefab;
    private GameObject deathPanelUIGameobject;


    private Rigidbody rb;

    private float startSpinSpeed;
    private float currentSpinSpeed;
    [SerializeField] private Image spinSpeedBarImage;
    [SerializeField] private TextMeshProUGUI spinSpeedRatioText;


    [SerializeField] private float commonDamageCoefficient = 0.04f;

    [SerializeField] private bool isAttacker;
    [SerializeField] private bool isDefender;
    private bool isDead = false;


    [Header("Player Type Damage Coefficients")]
    [SerializeField] private float doDamageCoefficientAttacker = 10f;
    [SerializeField] private float getDamagedCoefficientAttacker = 1.2f;

    [SerializeField] private float doDamageCoefficientDefender = 0.75f;
    [SerializeField] private float getDamagedCoefficientDefender = 0.2f;


    private void Awake()
    {
        startSpinSpeed = spinnerScript._spinSpeed;
        currentSpinSpeed = spinnerScript._spinSpeed;

        spinSpeedBarImage.fillAmount = currentSpinSpeed / startSpinSpeed;

    }

    private void CheckPlayerType()
    {
        if (gameObject.name.Contains("Attacker"))
        {
            isAttacker = true;
            isDefender = false;


        }else if (gameObject.name.Contains("Defender"))
        {
            isDefender = true;
            isAttacker = false;

            spinnerScript._spinSpeed = 4400;

            startSpinSpeed = spinnerScript._spinSpeed;
            currentSpinSpeed = spinnerScript._spinSpeed;

            spinSpeedRatioText.text = currentSpinSpeed + "/" + startSpinSpeed;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (photonView.IsMine)
            {
                Vector3 effectPosition = (gameObject.transform.position + collision.transform.position) / 2 + new Vector3(0, 0.05f, 0);

                GameObject collisionEffectGameobject = GetPooledObject();
                if (collisionEffectGameobject != null)
                {
                    collisionEffectGameobject.transform.position = effectPosition;
                    collisionEffectGameobject.SetActive(true);
                    collisionEffectGameobject.GetComponentInChildren<ParticleSystem>().Play();

                    StartCoroutine(DeactivateAfterSeconds(collisionEffectGameobject, 0.5f));
                }
            }

            float mySpeed = gameObject.GetComponent<Rigidbody>().velocity.magnitude;
            float otherPlayerSpeed = collision.collider.gameObject.GetComponent<Rigidbody>().velocity.magnitude;

            Debug.Log("My speed: "+ mySpeed+ " -----Other player speed: "+ otherPlayerSpeed);

            if (mySpeed>otherPlayerSpeed)
            {
                Debug.Log(" You DAMAGE the other player.");
                float default_Damage_Amount = gameObject.GetComponent<Rigidbody>().velocity.magnitude * 3600f * commonDamageCoefficient;

                if (isAttacker)
                {
                    default_Damage_Amount *= doDamageCoefficientAttacker;

                }
                else if (isDefender)
                {
                    default_Damage_Amount *= doDamageCoefficientDefender;
                }

                if (collision.collider.gameObject.GetComponent<PhotonView>().IsMine)
                {          
                    collision.collider.gameObject.GetComponent<PhotonView>().RPC("DoDamage", RpcTarget.AllBuffered, default_Damage_Amount);
                }
            }         
        }
    }


    [PunRPC]
    public void DoDamage(float _damageAmount)
    {
        if (!isDead)
        {
            if (isAttacker)
            {
                _damageAmount *= getDamagedCoefficientAttacker;

                if (_damageAmount > 1000)
                {
                    _damageAmount = 400f;
                }
            }
            else if (isDefender)
            {
                _damageAmount *= getDamagedCoefficientDefender;
            }
            spinnerScript._spinSpeed -= _damageAmount;
            currentSpinSpeed = spinnerScript._spinSpeed;

            spinSpeedBarImage.fillAmount = currentSpinSpeed / startSpinSpeed;
            spinSpeedRatioText.text = currentSpinSpeed.ToString("F0") + "/" + startSpinSpeed;

            if (currentSpinSpeed < 100)
            {
                Die();
            }
        }

    }

    void Die()
    {
        isDead = true;

        GetComponent<MovementController>().enabled = false;
        rb.freezeRotation = false;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        spinnerScript._spinSpeed = 0f;

        uI3DGameobject.SetActive(false);

        if (photonView.IsMine)
        {
            StartCoroutine(ReSpawn());
        }
    }

    IEnumerator ReSpawn()
    {
        GameObject canvasGameobject = GameObject.Find("Canvas");
        if (deathPanelUIGameobject==null)
        {
            deathPanelUIGameobject = Instantiate(deathPanelUIPrefab, canvasGameobject.transform);
        }
        else
        {
            deathPanelUIGameobject.SetActive(true);
        }

        Text respawnTimeText = deathPanelUIGameobject.transform.Find("RespawnTimeText").GetComponent<Text>();

        float respawnTime = 8.0f;

        respawnTimeText.text = respawnTime.ToString(".00");

        while (respawnTime > 0.0f)
        {
            yield return new WaitForSeconds(1.0f);
            respawnTime -= 1.0f;
            respawnTimeText.text = respawnTime.ToString(".00");

            GetComponent<MovementController>().enabled = false;

        }

        deathPanelUIGameobject.SetActive(false);

        GetComponent<MovementController>().enabled = true;

        photonView.RPC("ReBorn",RpcTarget.AllBuffered);
    }

    [PunRPC]
    public void ReBorn()
    {
        spinnerScript._spinSpeed = startSpinSpeed;
        currentSpinSpeed = spinnerScript._spinSpeed;

        spinSpeedBarImage.fillAmount = currentSpinSpeed / startSpinSpeed;
        spinSpeedRatioText.text = currentSpinSpeed + "/" + startSpinSpeed;

        rb.freezeRotation = true;
        transform.rotation = Quaternion.Euler(Vector3.zero);

        uI3DGameobject.SetActive(true);

        isDead = false;
    }



    public List<GameObject> pooledObjects;
    public int amountToPool = 8;
    public GameObject CollisionEffectPrefab;
    void Start()
    {
        CheckPlayerType();

        rb = GetComponent<Rigidbody>();

        if (photonView.IsMine)
        {
            pooledObjects = new List<GameObject>();
            for (int i = 0; i < amountToPool; i++)
            {
                GameObject obj = (GameObject)Instantiate(CollisionEffectPrefab, Vector3.zero, Quaternion.identity);
                obj.SetActive(false);
                pooledObjects.Add(obj);
            }
        }
    }

    public GameObject GetPooledObject()
    {
        for (int i = 0; i < pooledObjects.Count; i++)
        {
            if (!pooledObjects[i].activeInHierarchy)
            {
                return pooledObjects[i];
            }
        }
       
        return null;
    }

    IEnumerator DeactivateAfterSeconds(GameObject _gameObject, float _seconds)
    {
        yield return new WaitForSeconds(_seconds);
        _gameObject.SetActive(false);
    }
}
