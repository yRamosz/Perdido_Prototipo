using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class VoltarMenu : MonoBehaviour
{
    [SerializeField] private string nomeCenaMenuInicial = "MenuInicial";

    public void VoltarParaMenuPrincipal()
    {
        SceneManager.LoadScene("MenuInicial");
    }
}
