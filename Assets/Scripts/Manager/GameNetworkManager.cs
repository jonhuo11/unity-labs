using UnityEngine;
using Mirror;

public class GameNetworkManager : NetworkManager
{
    public Itemdex itemdex;
    private Itemdex itemdexInstance;
    public GameManager1 gameManager;
    private GameManager1 gameManagerInstance;

    public override void OnStartServer()
    {
        base.OnStartServer();

        // spawn in the itemdex and gamemanager entities
        SpawnManagers();
    }

    public override void OnStopServer()
    {
        base.OnStopServer();

        // cleanup managers
        DestroyManagers();
    }

    void SpawnManagers()
    {
        if (gameManager == null && itemdex == null)
        {
            Debug.LogError("no itemdex or gameManager assigned");
            return;
        }
        itemdexInstance = Instantiate(itemdex);
        gameManagerInstance = Instantiate(gameManager);
        Debug.Log("successfully instantiated itemdex and gameManager");
    }

    void DestroyManagers()
    {
        if (itemdexInstance != null)
        {
            Destroy(itemdexInstance.gameObject);
        }
        if (gameManagerInstance != null)
        {
            Destroy(gameManagerInstance.gameObject);
        }
    }
}
