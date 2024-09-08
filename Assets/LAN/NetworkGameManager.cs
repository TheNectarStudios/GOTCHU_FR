using UnityEngine;
using Unity.Netcode;

public class NetworkGameManager : MonoBehaviour
{
    void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 10, 200, 500));
        
        if (GUILayout.Button("Host"))
        {
            NetworkManager.Singleton.StartHost();  // Start the host (acts as both server and client)
        }

        if (GUILayout.Button("Client"))
        {
            NetworkManager.Singleton.StartClient();  // Join the game as a client
        }

        if (GUILayout.Button("Server"))
        {
            NetworkManager.Singleton.StartServer();  // Start as a dedicated server
        }

        GUILayout.EndArea();
    }
}
