using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using Google.Protobuf;

public class ExternalCommunication : MonoBehaviour
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
        var client = new UdpClient();
        IPEndPoint ep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 11000); // endpoint where server is listening
        client.Connect(ep);

        // send data
        var request = new Telegrams.Request();
        request.Command = Telegrams.Request.Types.Command.Print;
        request.Message = "Hello World";
        var outputByteArray = new byte[request.CalculateSize()];
        request.WriteTo(new CodedOutputStream(outputByteArray));
        client.Send(outputByteArray, outputByteArray.Length);

        // then receive data
        var receivedData = client.Receive(ref ep);

        print(">> " + ep.ToString());

        Console.Read();
    }
}
