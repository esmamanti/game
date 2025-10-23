using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ArayuzKontrol : MonoBehaviour
{
    public Text MermiText;
    public Text SaglikText;
    GameObject oyuncu;

    void Start()
    {
        oyuncu = GameObject.Find("Bacý");
    }

    // Update is called once per frame
    void Update()
    {
        MermiText.text = oyuncu.GetComponent<AtesEtme>().GetSarjor().ToString() + "/" + oyuncu.GetComponent<AtesEtme>().GetCephane().ToString();
        SaglikText.text = "HP:" + oyuncu.GetComponent<KarakterKontrol>().GetSaglik();
    }
}
