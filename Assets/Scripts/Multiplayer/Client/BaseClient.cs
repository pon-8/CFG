using Unity.Networking.Transport;
using UnityEngine;
using Unity.Collections;

public class BaseClient : MonoBehaviour
{
    // Handles communications between software and network
    public NetworkDriver NetDriver;
    // Connetcion
    protected NetworkConnection Connection;

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
        // Reset connection to default
        Connection = default(NetworkConnection);
        // Where to connect?
        NetworkEndPoint endpoint = NetworkEndPoint.LoopbackIpv4;
        // Defined Port
        endpoint.Port = 4000;
        // Connect driver to endpoint
        Connection = NetDriver.Connect(endpoint);
    }
    public virtual void Shutdown()
    {
        NetDriver.Dispose();
    }
    public virtual void UpdateServer()
    {
        NetDriver.ScheduleUpdate().Complete();
        CheckAlive();
        UpdateMessagePump();
    }
    private void CheckAlive()
    {
        if (!Connection.IsCreated)
        {
            Debug.Log("Lost connection to server");
        }
    }
    protected virtual void UpdateMessagePump()
    {
        // Create data stream reader for reading information sent from clients
        DataStreamReader stream;

        // Check incoming information from server
        NetworkEvent.Type cmd;
        while ((cmd = Connection.PopEvent(NetDriver, out stream)) != NetworkEvent.Type.Empty)
        {
            if (cmd == NetworkEvent.Type.Connect)
            {
                Debug.Log("Client connected to server");
            }
            // server sent data
            else if (cmd == NetworkEvent.Type.Data)
            {
                uint number = stream.ReadByte();
                Debug.Log("got " + number + " back from server");
            }
            // Client disconnected
            else if (cmd == NetworkEvent.Type.Disconnect)
            {
                Debug.Log("Client disconnected from server");
                // Changing connection to default
                Connection = default(NetworkConnection);
            }
        }
    }
}