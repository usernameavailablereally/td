using Unity.Netcode;
using UnityEngine;
using Unity.Netcode.Transports.UTP;

public class Loft3DNetworkManager : MonoBehaviour
{
    private static NetworkManager m_NetworkManager;
    private static string address = "127.0.0.1:7777"; 
    void Awake()
    {
        m_NetworkManager = GetComponent<NetworkManager>();
    }

    void OnGUI()
    {
        if (!m_NetworkManager.IsClient && !m_NetworkManager.IsServer)
        {
            GUILayout.BeginArea(new Rect(Screen.width / 2 - 75,
                Screen.height / 2 - 50,
                150,
                Screen.height/4));
            GUILayout.BeginVertical("box");
            address = GUI.TextField(new Rect(5, 5, 140, 20), address, 25);
            GUILayout.Space(25);
            StartButtons();
            GUILayout.EndVertical();
            GUILayout.EndArea();
        }
        else
        {
            GUILayout.BeginArea(new Rect(10,
                110,
                300,
                100));
            StatusLabels();
            GUILayout.EndArea();
        }
    }

    static void StartButtons()
    {
        var hostClick = GUILayout.Button("Host");
        var clientClick = GUILayout.Button("Client");
        var serverClick = GUILayout.Button("Server");

        if (hostClick || clientClick || serverClick)
        {
            var _address = address.Split(":");
            var port = 7777;
            int.TryParse(_address[1], out port);
            m_NetworkManager.GetComponent<UnityTransport>().SetConnectionData(_address[0], (ushort)port);
        }

        if (hostClick)
        {
            m_NetworkManager.StartHost();
        }
        else if (clientClick)
        {
            m_NetworkManager.StartClient();
        }
        else if (serverClick)
        {
            m_NetworkManager.StartServer();
        }
    }

    static void StatusLabels()
    {
        var mode = m_NetworkManager.IsHost
            ? "Host"
            : m_NetworkManager.IsServer
                ? "Server"
                : "Client";

        GUILayout.Label("Transport: " +
                        m_NetworkManager.NetworkConfig.NetworkTransport.GetType()
                            .Name);
        GUILayout.Label("Mode: " + mode);
    }
}