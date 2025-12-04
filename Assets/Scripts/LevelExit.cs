using UnityEngine;

public class LevelExit : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Venceu!");

            GameManager gm = FindObjectOfType<GameManager>();
            
            if (gm != null)
            {
                gm.TriggerWin();
            }
        }
    }
}