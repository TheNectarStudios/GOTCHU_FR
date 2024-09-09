using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;  // Ensure you have this namespace for UnityTransport

public class NetworkGameManager : MonoBehaviour
{
    // Track whether the server/client/host is running
    private bool isNetworkStarted = false;
    private string hostIP = "IP: Not Available";  // Default text

    void Start()
    {
        // Display local IP address on startup
        hostIP = GetLocalIPAddress();
    }

    void OnGUI()
    {
        // Display the IP address on the screen
        GUILayout.BeginArea(new Rect(10, 10, 300, 500));  // Increased the width of the area
        GUILayout.Label("Host IP Address: " + hostIP, GUILayout.Width(300), GUILayout.Height(30));

        // If the network is already started, don't display the buttons
        if (!isNetworkStarted)
        {
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
        }

        GUILayout.EndArea();
    }

    // Function to get the local IP address
    private string GetLocalIPAddress()
    {
        string localIP = "IP: Not Available";

        foreach (var iface in System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces())
        {
            var ipProps = iface.GetIPProperties();
            foreach (var addr in ipProps.UnicastAddresses)
            {
                if (addr.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    localIP = addr.Address.ToString();
                    return localIP;  // Return the first found IP address
                }
            }
        }

        return localIP;
    }
}
