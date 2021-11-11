using System;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

public class MatchmakingConnector : MonoBehaviour
{
    private static int MAX_TRIES = 10;
    //private static int DATAGRAM_SIZE = 32;
    [SerializeField] private static string matchmakingServerIP = "127.0.0.1";
    [SerializeField] private static int matchmakingServerPort = 56567;
    
    [HideInInspector] public static MatchmakingConnector Singleton;

    private void Awake()
    {
        Singleton = this;
    }

    public List<Room> GetRooms()
    {
        string response = UDPSendMessage(matchmakingServerIP, matchmakingServerPort, "List");
        List<Room> rooms = ParseJson(response);
        return rooms;
    }

    static public string UDPSendMessage(String message)
    {
        return UDPSendMessage(matchmakingServerIP, matchmakingServerPort, message);
    }

    static private string UDPSendMessage(String server, Int32 port, String message)
    {
        try
        {
            UdpClient client = new UdpClient(server, port);
            var endpoint = new System.Net.IPEndPoint(System.Net.IPAddress.Any, 0);
            client.Client.ReceiveTimeout = 2000;
            client.Client.SendTimeout = 2000;
            byte[] request = System.Text.Encoding.ASCII.GetBytes(message);
            byte[] response = null;

            for (int i = 0; i < MAX_TRIES; i++)
            {
                try
                {
                    client.Send(request, request.Length);
                    Debug.Log($"Sent: {message}");
                    response = client.Receive(ref endpoint);
                    break;
                }
                catch (SocketException)
                {
                    Debug.LogWarning("Socket exception, retrying...");
                }
            }

            if(response == null)
            {
                Debug.LogError("Unable to connect to server");
                throw new SocketException();
            }

            var responseMsg = System.Text.Encoding.ASCII.GetString(response, 0, response.Length);

            Debug.Log(responseMsg);
            client.Close();

            return responseMsg;
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

    private static List<Room> ParseJson(string json)
    {
        string current = json;
        List<Room> rooms = new List<Room>();
        while (current.Contains("{"))
        {
            string element = current.Substring(current.IndexOf("{"), current.IndexOf("}"));
            rooms.Add(JsonUtility.FromJson<Room>(element));
            current = current.Substring(current.IndexOf("}") + 1);
        }
        return rooms;
    }
}
