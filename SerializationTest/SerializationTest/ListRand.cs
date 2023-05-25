namespace SerializationTest;

public class ListNode
{
    public ListNode Prev;
    public ListNode Next;
    public ListNode Rand;
    public string Data;
}

public class ListRand
{
    // Оставил поля как в примере.
    // Если бы писал полность сам, то ограничил бы доступ к полям.
    // Написал бы методы изменения и свойства для чтения
    public ListNode Head;
    public ListNode Tail;
    public int Count;

    // Разделил только на методы
    // Сереализация и Десериализация я бы вынес в отдельный класс
    // Делал так, потому что в ТЗ указаны они в этом классе
    public void Serialize(FileStream s)
    {
        using var binaryWrite = new BinaryWriter(s);
        var nodeToIndex = new Dictionary<ListNode, int>(Count);
        
        binaryWrite.Write(Count);
        WriteNodes(binaryWrite, ref nodeToIndex);
        WriteRandNodes(binaryWrite, nodeToIndex);
    }

    public void Deserialize(FileStream s)
    {
        using var binaryReader = new BinaryReader(s);
        
        Count = binaryReader.ReadInt32();
        if (Count == 0) return;

        var nodes = ReadNodes(binaryReader, Count);
        SetRandNodes(binaryReader, nodes);
        
        Head = nodes[0];
        Tail = nodes[Count - 1];
    }
    
    private void WriteNodes(BinaryWriter binaryWrite, ref Dictionary<ListNode, int> nodeToIndex)
    {
        var index = 0;
        var node = Head;
        
        while (node != null)
        {
            binaryWrite.Write(node.Data);
            
            nodeToIndex[node] = index;
            index++;
            node = node.Next;
        }
    }

    private void WriteRandNodes(BinaryWriter binaryWrite, IReadOnlyDictionary<ListNode, int> nodeToIndex)
    {
        var node = Head;
        while (node != null)
        {
            binaryWrite.Write(node.Rand != null ? nodeToIndex[node.Rand] : -1);
            node = node.Next;
        }
    }

    private static ListNode[] ReadNodes(BinaryReader binaryReader, int count)
    {
        var nodes = new ListNode[count];
        
        for (var i = 0; i < count; i++)
        {
            var node = new ListNode();
            node.Data = binaryReader.ReadString();
            nodes[i] = node;
        }

        return nodes;
    }
    
    private static void SetRandNodes(BinaryReader binaryReader, IReadOnlyList<ListNode> nodes)
    {
        for (var i = 0; i < nodes.Count; i++)
        {
            var node = nodes[i];

            if (i != 0) node.Prev = nodes[i - 1];
            if (i != nodes.Count - 1) node.Next = nodes[i + 1];
            
            var randIndex = binaryReader.ReadInt32();
            if (randIndex != -1) node.Rand = nodes[randIndex];
        }
    }
}
