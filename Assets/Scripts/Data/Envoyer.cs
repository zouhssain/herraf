using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using System;

public class Envoyer : MonoBehaviour
{
    public InputField nom;
    public InputField prenom;
    public InputField email;
    public InputField numero;
    public Button send;

    //[SerializeField] string nom;
    private string NOM;
    private string PRENOM;
    private string EAMIL;
    private string NUMERO;
    private bool state = true;
    [SerializeField] public Color Correct;
    [SerializeField] public Color Incorrect;


    void Update()
    {
        Tester();
    }

    [SerializeField] string url = "https://docs.google.com/forms/d/e/1FAIpQLScLeNit3k0YCx_yo0i4LGdlS4Ds3fkXst4CLkTLZYY859OdIw/formResponse";
    public void Send()
    {
        NOM = nom.GetComponent<InputField>().text;
        PRENOM = prenom.GetComponent<InputField>().text;
        EAMIL = email.GetComponent<InputField>().text;
        NUMERO = numero.GetComponent<InputField>().text;

        Debug.Log("Vous avez cliquez : le nom est : " + NOM);

        StartCoroutine(Post(NOM, PRENOM, EAMIL, NUMERO));
        Edit();
    }
    IEnumerator Post(string nom, string prenom, string email, string numero)
    {
        WWWForm form = new WWWForm();
        form.AddField("entry.411481215", nom);
        form.AddField("entry.700688726", prenom);
        form.AddField("entry.365876566", email);
        form.AddField("entry.1702453875", numero);
        byte[] rawdata = form.data;
        WWW www = new WWW(url, rawdata);
        yield return www;
    }

    public void Edit()
    {
        nom.interactable = state;
        prenom.interactable = state;
        email.interactable = state;
        numero.interactable = state;
        if (state) state = false;
        else state = true;
    }

    public void Tester()
    {
        NOM = nom.GetComponent<InputField>().text;
        PRENOM = prenom.GetComponent<InputField>().text;
        EAMIL = email.GetComponent<InputField>().text;
        NUMERO = numero.GetComponent<InputField>().text;

        if (isValidEmail(EAMIL))
        {
            Debug.Log("eamil naadi");
            email.GetComponent<InputField>().targetGraphic.color = Correct;
        }
        else
        {
            Debug.Log("email manadich");
            email.GetComponent<InputField>().targetGraphic.color = Incorrect;
        }
        if (isValidName(NOM))
        {
            Debug.Log("nom naadi");
            nom.GetComponent<InputField>().targetGraphic.color = Correct;
        }
        else
        {
            Debug.Log("nom manadich");
            nom.GetComponent<InputField>().targetGraphic.color = Incorrect;
        }
        if (isValidName(PRENOM))
        {
            Debug.Log("prenom naadi");
            prenom.GetComponent<InputField>().targetGraphic.color = Correct;
        }
        else
        {
            Debug.Log("prenom manadich");
            prenom.GetComponent<InputField>().targetGraphic.color = Incorrect;
        }
        if (isValidNumber(NUMERO))
        {
            Debug.Log("numero naadi");
            numero.GetComponent<InputField>().targetGraphic.color = Correct;
        }
        else
        {
            Debug.Log("numero manadich");
            numero.GetComponent<InputField>().targetGraphic.color = Incorrect;
        }

        if (isValidName(NOM) && isValidName(PRENOM) && isValidNumber(NUMERO) && isValidEmail(EAMIL)) send.interactable = true;
        else send.interactable = false;


    }

    private static bool isValidEmail(string inputEmail)
    {
        string strRegex = @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}" +
              @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\" +
              @".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";
        Regex re = new Regex(strRegex);
        if (re.IsMatch(inputEmail))
            return (true);
        else
            return (false);
    }
    private static bool isValidName(string inputName)
    {
        string strRegex = @"[a-z|A-Z]{2,30}";
        Regex re = new Regex(strRegex);
        if (re.IsMatch(inputName))
            return (true);
        else
            return (false);
    }
    private static bool isValidNumber(string inputNumber)
    {
        string strRegex = @"06[0-9]{8}";
        Regex re = new Regex(strRegex);
        if (re.IsMatch(inputNumber))
            return (true);
        else
            return (false);
    }
}
