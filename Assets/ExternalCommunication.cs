using UnityEngine;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using Google.Protobuf;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System;

public class ExternalCommunication
{
    public delegate void RequestAnswerCallback(Telegrams.Request requestAnswer);

    private BlockingCollection<byte[]> send_buffer;
    private UdpClient client;
    private IPEndPoint endpoint;
    private Dictionary<Guid, RequestAnswerCallback> requestAnswerCallbackDictionary;

    private static ExternalCommunication singleton = null;

    private ExternalCommunication()
    {
        requestAnswerCallbackDictionary = new Dictionary<Guid, RequestAnswerCallback>();
        send_buffer = new BlockingCollection<byte[]>();
        client = new UdpClient();
        endpoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 11000); // endpoint where server is listening
        client.Connect(endpoint);

        Thread communicationPipeSend = new Thread(CommunicationPipeSend);
        communicationPipeSend.Start();

        Thread communicationPipeReceive = new Thread(CommunicationPipeReceive);
        communicationPipeReceive.Start();
    }

    public static ExternalCommunication GetSingleton()
    {
        if (singleton == null)
        {
            singleton = new ExternalCommunication();
        }

        return singleton;
    }

    public void SendAsynch(Telegrams.Request request, RequestAnswerCallback requestAnswerCallback)
    {
        requestAnswerCallbackDictionary.Add(Guid.Parse(request.TransactionId), requestAnswerCallback);
        SendAsynch(request);
    }

    public void SendAsynch(Telegrams.Request request)
    {
        var outputByteArray = new byte[request.CalculateSize()];
        request.WriteTo(new CodedOutputStream(outputByteArray));

        send_buffer.Add(outputByteArray);
    }

    void CommunicationPipeReceive()
    {
        while (true)
        {
            var receivedData = client.Receive(ref endpoint);
            var answer = Telegrams.Request.Parser.ParseFrom(receivedData);
            var transaction_id = Guid.Parse(answer.TransactionId);
            var answerCallback = requestAnswerCallbackDictionary[transaction_id];
            requestAnswerCallbackDictionary.Remove(transaction_id);
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                answerCallback(answer);
            });
        }
    }

    void CommunicationPipeSend()
    {
        while (true)
        {
            var message = send_buffer.Take();
            client.Send(message, message.Length);
        }
    }
}
