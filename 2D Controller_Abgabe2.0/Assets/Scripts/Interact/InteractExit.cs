using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InteractExit : MonoBehaviour
{
    public void LoadNextLevel()
    {
        // Lädt eine neue Szene mit dem Namen "EndScreen"
        Debug.Log("Hello!");
        SceneManager.LoadScene("EndScreen");
    }
}
