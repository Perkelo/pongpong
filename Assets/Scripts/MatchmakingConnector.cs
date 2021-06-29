using System;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

public class MatchmakingConnector : MonoBehaviour
{
    [SerializeField] private string matchmakingServerIP = "127.0.0.1";
    [SerializeField] private int matchmakingServerPort = 7878;
    
    [HideInInspector] public static MatchmakingConnector Singleton;

    private void Awake()
    {
        Singleton = this;
    }

    public List<ServerEndpoint> GetServers()
    {
        string response = Connect(matchmakingServerIP, matchmakingServerPort, "GS");
        List<ServerEndpoint> servers = ParseJson(response);
        return servers;

        //List<ServerEndpoint> servers = new List<ServerEndpoint>();
        servers.Add(new ServerEndpoint("127.0.0.1", 1111));
        servers.Add(new ServerEndpoint("127.0.0.1", 1112));
        servers.Add(new ServerEndpoint("127.0.0.1", 1113));
        servers.Add(new ServerEndpoint("127.0.0.1", 1114));
        servers.Add(new ServerEndpoint("127.0.0.1", 1115));
        return servers;
    }

    static private string Connect(String server, Int32 port, String message)
    {
        try
        {
            TcpClient client = new TcpClient(server, port);

            // Translate the passed message into ASCII and store it as a Byte array.
            Byte[] data = System.Text.Encoding.ASCII.GetBytes(message + "\n");

            // Get a client stream for reading and writing.
            //  Stream stream = client.GetStream();

            NetworkStream stream = client.GetStream();

            // Send the message to the connected TcpServer.
            stream.Write(data, 0, data.Length);

            Debug.Log($"Sent: {message}");

            // Receive the TcpServer.response.

            // Buffer to store the response bytes.
            data = new Byte[256];

            // String to store the response ASCII representation.
            String responseData = String.Empty;

            // Read the first batch of the TcpServer response bytes.
            Int32 bytes = stream.Read(data, 0, data.Length);
            responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
            Debug.Log($"Received: {responseData}");

            // Close everything.
            stream.Close();
            client.Close();

            return responseData;
        }
        catch (ArgumentNullException e)
        {
            Debug.LogError($"ArgumentNullException: {e}");
        }
        catch (SocketException e)
        {
            Debug.LogError($"SocketException: {e}");
        }

        return "";
    }

    private static List<ServerEndpoint> ParseJson(string json)
    {
        string current = json;
        List<ServerEndpoint> servers = new List<ServerEndpoint>();
        while (current.Contains("{"))
        {
            Debug.Log(current);
            string element = current.Substring(current.IndexOf("{"), current.IndexOf("}"));
            //Debug.Log(element);
            servers.Add(JsonUtility.FromJson<ServerEndpoint>(element));
            current = current.Substring(current.IndexOf("}") + 1);
            Debug.Log(current);
        }
        return servers;
    }
}
