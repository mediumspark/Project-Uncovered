using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine; 

public class GameManager 
{
    private static int nextspawnposition; 

    public static void LevelReload()
    {
        SceneManager.sceneLoaded += SceneManager_sceneLoaded;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public static  void LoadScene(string Levelname, int spawnpoint)
    {
        SceneManager.sceneLoaded -= SceneManager_sceneLoaded;
        nextspawnposition = spawnpoint;
        SceneManager.sceneLoaded += SceneManager_sceneLoaded;
        SceneManager.LoadScene(Levelname);

    }

    private static void SceneManager_sceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        SpawnPlayer(nextspawnposition); 
    }

    private static void SpawnPlayer(int index)
    {
        Player player = GameObject.FindObjectOfType<Player>();

        if (player != null)
        {
            GameObject[] go = GameObject.FindGameObjectsWithTag("Respawn");
            player.GetComponent<CharacterController>().enabled = false;
            player.transform.position = go[index].transform.position;
            player.GetComponent<CharacterController>().enabled = true;

            CinemachineVirtualCamera OWcam = GameObject.FindObjectOfType<CinemachineVirtualCamera>(); 
            if(OWcam != null)
            {
                OWcam.Follow = player.transform;
                OWcam.LookAt = player.transform;
            }
        }
        else
        {
            GameObject go = GameObject.Instantiate((GameObject)Resources.Load("Prefabs/Player"));
            SpawnPlayer(index); 
        }
    }

}
