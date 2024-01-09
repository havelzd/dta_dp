using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class DataConversionTest
{

    BatchMessage batch = new();
    List<SoldierListMessage> messageList = new();

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        messageList = new()
        {
            new SoldierListMessage(){ payload = new List<OperativeData>() { new() { id = "01"}, new() { id="02" } } },
            new SoldierListMessage(){ payload = new List<OperativeData>() { new() { id = "01"}, new() { id="02" } } },
        };

        batch = new BatchMessage() { content = messageList };

    }

    [Test]
    public void BatchMessageConversion()
    {
        string messageJson = serialize(batch);

        Assert.IsNotNull(messageJson);

        Message message = deserialize(messageJson);
        BatchMessage batchConverted = message as BatchMessage;

        Assert.IsNotNull(message);
        Assert.IsInstanceOf(typeof(BatchMessage), batchConverted);
        Assert.IsNotNull(batch.content);
    }

    [Test]
    public void ListMessageConversion()
    {
        foreach (var missionMessage in messageList) {
            string missionJson = serialize(missionMessage);

            Assert.IsNotNull(missionJson);

            Message message = deserialize(missionJson);
            SoldierListMessage missionConverted = message as SoldierListMessage;

            Assert.IsNotNull(message);
            Assert.IsInstanceOf(typeof(SoldierListMessage), missionConverted);
            Assert.IsNotNull(missionConverted.payload);
        }
        
    }

    [Test]
    public void MissingMessageCodeConversion()
    {
        Message message = deserialize("{}");
        Assert.IsNull(message);
    }

    private Message deserialize(string messageJson)
    {
        return MessageConverter.deserialize(messageJson);
    }

    private string serialize(Message m)
    {
        return MessageConverter.serialize(m);
    }

    
}
