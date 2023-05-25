using SerializationTest;

namespace Tests;

public class Tests
{
    private const string FileName = "Test.data";

    [Test]
    public void TestSerializationDeserializationList()
    {
        var list = new ListRand();
        var node1 = new ListNode { Data = "Node 1" };
        var node2 = new ListNode { Data = "Node 2" };
        var node3 = new ListNode { Data = "Node 3" };
        var node4 = new ListNode { Data = "Node 4" };
        var node5 = new ListNode { Data = "Node 5" };

        node1.Next = node2;
        node2.Prev = node1;
        node2.Next = node3;
        node3.Prev = node2;
        node3.Next = node4;
        node4.Prev = node3;
        node4.Next = node5;
        node5.Prev = node4;

        node2.Rand = node2;
        node3.Rand = node5;
        node4.Rand = node1;

        list.Head = node1;
        list.Tail = node5;
        list.Count = 5;

        Serialize(list);
        var deserializedList = GetDeserializationList();

        CheckCount(list, deserializedList);
        Assert.That(deserializedList.Head.Data, Is.EqualTo(list.Head.Data));
        Assert.That(deserializedList.Tail.Data, Is.EqualTo(list.Tail.Data));

        CheckNodes(list, deserializedList);
    }

    [Test]
    public void TestSerializationDeserializationEmptyList()
    {
        var emptyList = new ListRand();

        Serialize(emptyList);
        var deserializedList = GetDeserializationList();

        CheckCount(emptyList, deserializedList);
        Assert.That(deserializedList.Head, Is.Null);
        Assert.That(deserializedList.Tail, Is.Null);
    }

    [Test]
    public void TestSerializationDeserializationSingleElementList()
    {
        var list = new ListRand();
        var node1 = new ListNode { Data = "Node 1" };

        list.Head = node1;
        list.Tail = node1;
        list.Count = 1;

        Serialize(list);
        var deserializedList = GetDeserializationList();

        CheckCount(list, deserializedList);
        Assert.That(deserializedList.Head.Data, Is.EqualTo(list.Head.Data));
        Assert.That(deserializedList.Tail.Data, Is.EqualTo(list.Tail.Data));
    }
    
    [Test]
    public void TestSerializationDeserializationListWithTwoElement()
    {
        var list = new ListRand();
        var node1 = new ListNode { Data = "Node 1" };
        var node2 = new ListNode { Data = "Node 2" };

        node1.Next = node2;
        node2.Prev = node1;
        
        node1.Rand = node2;
        node2.Rand = node1;

        list.Head = node1;
        list.Tail = node2;
        list.Count = 2;

        Serialize(list);
        var deserializedList = GetDeserializationList();

        CheckCount(list, deserializedList);
        CheckNodes(list, deserializedList);
    }

    private static void Serialize(ListRand list)
    {
        if (File.Exists(FileName)) File.Delete(FileName);
        using var stream = new FileStream(FileName, FileMode.Create);
        list.Serialize(stream);
    }

    private static ListRand GetDeserializationList()
    {
        var deserializedList = new ListRand();
        using var stream = new FileStream(FileName, FileMode.Open);
        deserializedList.Deserialize(stream);

        return deserializedList;
    }

    private static void CheckCount(ListRand originalList, ListRand deserializedList) =>
        Assert.That(deserializedList.Count, Is.EqualTo(originalList.Count));
    
    private static void CheckNodes(ListRand originalList, ListRand deserializedList)
    {
        var originalNode = originalList.Head;
        var deserializedNode = deserializedList.Head;
        
        while (originalNode != null && deserializedNode != null)
        {
            Assert.That(deserializedNode.Data, Is.EqualTo(originalNode.Data));

            if (originalNode.Rand != null)
                Assert.That(deserializedNode.Rand.Data, Is.EqualTo(originalNode.Rand.Data));

            originalNode = originalNode.Next;
            deserializedNode = deserializedNode.Next;
        }
    }
}
