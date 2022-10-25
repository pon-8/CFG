using Unity.Networking.Transport;
using UnityEngine;
using Unity.Collections;

public class BaseServer : MonoBehaviour
{
    // Handles communications between software and network
    public NetworkDriver NetDriver;
    // List to store all Connections
    protected NativeList<NetworkConnection> Connections;

    // Override standard method names to new ones
#if UNITY_EDITOR
    private void Start() { Init(); }
    private void Update() { UpdateServer(); }
    private void OnDestroy() { Shutdown(); }
#endif

    public virtual void Init()
    {
        // Driver init
        NetDriver = NetworkDriver.Create();
        // Who can connect?
        NetworkEndPoint endpoint = NetworkEndPoint.AnyIpv4;
        // Defined Port
        endpoint.Port = 4000;

        // Error msg if bind unsuccessfull, else activate driver
        if (NetDriver.Bind(endpoint) != 0)
        {
            Debug.Log("Error binding to port " + endpoint.Port);
        }
        else
        {
            NetDriver.Listen();
        }

        // Connection List init
        Connections = new NativeList<NetworkConnection>(2, Allocator.Persistent);
    }
    public virtual void UpdateServer()
    {
        NetDriver.ScheduleUpdate().Complete();
        CleanupConnections();
        AcceptNewConnections();
        UpdateMessagePump();
    }
    public virtual void Shutdown()
    {
        NetDriver.Dispose();
        Connections.Dispose();
    }
    private void CleanupConnections()
    {
        //Clean unused connections
        for (int i = 0; i < Connections.Length; i++)
        {
            if (!Connections[i].IsCreated)
            {
                Connections.RemoveAtSwapBack(i);
                i--;
            }

        }
    }
    private void AcceptNewConnections()
    {
        // Accept new Connections
        NetworkConnection c;
        while ((c = NetDriver.Accept()) != default(NetworkConnection))
        {
            Connections.Add(c);
            Debug.Log("Accepted new connection");
        }
    }
    protected virtual void UpdateMessagePump()
    {
        // Create data stream reader for reading information sent from clients
        DataStreamReader stream;

        // Check incoming information from all connections
        for (int i = 0; i < Connections.Length; i++)
        {
            NetworkEvent.Type cmd;
            while ((cmd = NetDriver.PopEventForConnection(Connections[i], out stream)) != NetworkEvent.Type.Empty)
            {
                // Client sent data
                if (cmd == NetworkEvent.Type.Data)
                {
                    uint number = stream.ReadByte();
                    Debug.Log("got " + number + " from the client");
                }
                // Client disconnected
                else if (cmd == NetworkEvent.Type.Disconnect)
                {
                    Debug.Log("Client disconnected from server");
                    // Changing connection to default
                    Connections[i] = default(NetworkConnection);
                }
            }
        }
    }
}