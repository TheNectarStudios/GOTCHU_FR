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
        Debug.Log("Host IP detected: " + hostIP);  // Log the host IP for debugging
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
                Debug.Log("Attempting to start as Host...");
                if (NetworkManager.Singleton.StartHost())
                {
                    isNetworkStarted = true;
                    Debug.Log("Host started successfully.");
                }
                else
                {
                    Debug.LogError("Failed to start as Host.");
                }
            }

            if (GUILayout.Button("Client", GUILayout.Width(250), GUILayout.Height(80)))
            {
                Debug.Log("Attempting to start as Client...");
                
                try
                {
                    // Start the client and connect to the host
                    NetworkManager.Singleton.NetworkConfig.ConnectionData = System.Text.Encoding.ASCII.GetBytes(hostIP);
                    if (NetworkManager.Singleton.StartClient())
                    {
                        isNetworkStarted = true;
                        Debug.Log("Client started successfully, attempting to connect to host at: " + hostIP);
                    }
                    else
                    {
                        Debug.LogError("Failed to start as Client.");
                    }
                }
                catch (System.Exception ex)
                {
                    Debug.LogError("Client connection failed: " + ex.Message);
                }
            }

            if (GUILayout.Button("Server", GUILayout.Width(250), GUILayout.Height(80)))
            {
                Debug.Log("Attempting to start as Server...");
                if (NetworkManager.Singleton.StartServer())
                {
                    isNetworkStarted = true;
                    Debug.Log("Server started successfully.");
                }
                else
                {
                    Debug.LogError("Failed to start as Server.");
                }
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
                Debug.Log("Detected IP: " + localIP);  // Log the IP for debugging
            }
        }

        if (string.IsNullOrEmpty(localIP))
        {
            Debug.LogError("No IP address found.");
        }

        return localIP;
    }
}
   