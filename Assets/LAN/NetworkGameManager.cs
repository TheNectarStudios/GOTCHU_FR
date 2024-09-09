using UnityEngine;
using Unity.Netcode;

public class NetworkGameManager : MonoBehaviour
{
    // Track whether the server/client/host is running
    private bool isNetworkStarted = false;

    void OnGUI()
    {
        // If the network is already started, don't display the buttons
        if (isNetworkStarted) return;

        GUILayout.BeginArea(new Rect(10, 10, 300, 500));  // Increased the width of the area

        // Make buttons larger using GUILayout.Width and GUILayout.Height
        if (GUILayout.Button("Host", GUILayout.Width(250), GUILayout.Height(80)))
        {
            NetworkManager.Singleton.StartHost();  // Start the host (acts as both server and client)
            isNetworkStarted = true;
        }

        if (GUILayout.Button("Client", GUILayout.Width(250), GUILayout.Height(80)))
        {
            NetworkManager.Singleton.StartClient();  // Join the game as a client
            isNetworkStarted = true;
        }

        if (GUILayout.Button("Server", GUILayout.Width(250), GUILayout.Height(80)))
        {
            NetworkManager.Singleton.StartServer();  // Start as a dedicated server
            isNetworkStarted = true;
        }

        GUILayout.EndArea();
    }
}
