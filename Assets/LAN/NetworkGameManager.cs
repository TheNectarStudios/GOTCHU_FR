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
                StartHost();
            }

            if (GUILayout.Button("Client", GUILayout.Width(250), GUILayout.Height(80)))
            {
                Debug.Log("Attempting to start as Client...");
                StartClient();
            }

            if (GUILayout.Button("Server", GUILayout.Width(250), GUILayout.Height(80)))
            {
                Debug.Log("Attempting to start as Server...");
                StartServer();
            }
        }

        GUILayout.EndArea();
    }

    void StartHost()
    {
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

    void StartClient()
    {
        try
        {
            if (NetworkManager.Singleton.StartClient())
            {
                isNetworkStarted = true;
                Debug.Log("Client started successfully, attempting to connect to host.");
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

    void StartServer()
    {
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
