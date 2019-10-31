using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.IO.Pipes;
using System;
using System.Linq;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using System.Text;

public class NamedPipesServer : MonoBehaviour
{
    Thread pipeThread;

    // Start is called before the first frame update
    void Start()
    {
        pipeThread = new Thread(CommunicationPipe);
        pipeThread.Start();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void CommunicationPipe()
    {
        UdpClient udpServer = new UdpClient(11000);

        while (true)
        {
            var remoteEP = new IPEndPoint(IPAddress.Any, 11000);
            var data = udpServer.Receive(ref remoteEP); // listen on port 11000
            print(">> " + remoteEP.ToString());
            udpServer.Send(new byte[] { 1 }, 1, remoteEP); // reply back
        }
    }
}
