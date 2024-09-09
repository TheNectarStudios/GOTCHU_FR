using UnityEngine;
using Unity.Netcode;
using System.Net;
using System.Net.Sockets;

public class NetworkGameManager : MonoBehaviour
{
    // Track whether the server/client/host is running 
    private bool isNetworkStarted = false;
    private string hostIP = "";

    void Start()
    {
        hostIP = GetLocalIPAddress();  // Get the host's local IP address
    }

    void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 10, 300, 500));

        if (!isNetworkStarted)
        {
            // Show the Host IP
            GUILayout.Label("Host IP: " + hostIP);

            if (GUILayout.Button("Host", GUILayout.Width(250), GUILayout.Height(80)))
            {
                NetworkManager.Singleton.StartHost();  // Start the host (acts as both server and client)
                isNetworkStarted = true;
            }

            if (GUILayout.Button("Client", GUILayout.Width(250), GUILayout.Height(80)))
            {
                // Use the host's IP to connect as a client
                NetworkManager.Singleton.NetworkConfig.ConnectionData = System.Text.Encoding.ASCII.GetBytes(hostIP);
                NetworkManager.Singleton.StartClient();
                isNetworkStarted = true;
            }

            if (GUILayout.Button("Server", GUILayout.Width(250), GUILayout.Height(80)))
            {
                NetworkManager.Singleton.StartServer();  // Start as a dedicated server
                isNetworkStarted = true;
            }
        }

        GUILayout.EndArea();
    }

    // Get the local IP address of the device (host)
    string GetLocalIPAddress()
    {
        string localIP = "";
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                localIP = ip.ToString();
                break;
            }
        }
        return localIP;
    }
}
