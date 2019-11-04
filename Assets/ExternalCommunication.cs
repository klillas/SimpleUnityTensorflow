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
using System.Collections.Concurrent;

public class ExternalCommunication : MonoBehaviour
{
    BlockingCollection<byte[]> send_buffer;
    Thread pipeThread;
    UdpClient client;

    // Start is called before the first frame update
    void Start()
    {
        send_buffer = new BlockingCollection<byte[]>();
        client = new UdpClient();
        IPEndPoint ep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 11000); // endpoint where server is listening
        client.Connect(ep);

        pipeThread = new Thread(CommunicationPipe);
        pipeThread.Start();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void SendAsynch(byte[] data)
    {
        send_buffer.Add(data);
    }

    void CommunicationPipe()
    {
        while (true)
        {
            var message = send_buffer.Take();
            client.Send(message, message.Length);
        }

        /*
        // send data
        var request = new Telegrams.Request();
        request.Command = Telegrams.Request.Types.Command.Print;
        request.Message = "Hello World";
        var outputByteArray = new byte[request.CalculateSize()];
        request.WriteTo(new CodedOutputStream(outputByteArray));

        while (true)
        {
            client.Send(outputByteArray, outputByteArray.Length);

            // then receive data
            var receivedData = client.Receive(ref ep);
            var answer = Telegrams.Request.Parser.ParseFrom(receivedData);
            print(">> " + answer.Message);
        }
        */
    }
}
