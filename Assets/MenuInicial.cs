using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuInicial : MonoBehaviour
{
    [SerializeField] private string nomeDaCenaDoJogo = "Jogo"; 
    [SerializeField] private string nomeDaCenaDeControles = "Controles"; 
    [SerializeField] private string nomeDaCenaDeHistoria = "Historia"; 
    [SerializeField] private string nomeDaCenaPrincipal = "MenuInicial";

    /// <summary>
    /// </summary>
    public void Jogar()
    {
        SceneManager.LoadScene(nomeDaCenaDoJogo);
    }

    /// <summary>
    /// </summary>
    public void AbrirControles()
    {
        SceneManager.LoadScene(nomeDaCenaDeControles);
    }

    public void AbrirHistoria()
    {
        SceneManager.LoadScene(nomeDaCenaDeHistoria);
    }

    public void VoltarAoMenu()
    {
        SceneManager.LoadScene(nomeDaCenaPrincipal);
    }

    /// <summary>
    /// </summary>
    public void SairDoJogo()
    {
        Debug.Log("Pedido para Sair do Jogo!");
        Application.Quit();
    }
}
