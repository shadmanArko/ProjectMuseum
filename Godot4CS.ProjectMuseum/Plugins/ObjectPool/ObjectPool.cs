using Godot;
using System.Collections.Generic;

public class ObjectPool : Node
{
    [Export]
    public PackedScene PooledObject; // The object you want to pool

    private List<Node> pool = new List<Node>();
    public int PreloadAmount { get; set; }
    public int InitialPoolSize { get; set; }

    public override void _Ready()
    {
        PreloadObjects(InitialPoolSize); // Preload objects when the game starts
    }

    

    public Node GetObject()
    {
        if (pool.Count == 0)
        {
            PreloadObjects(PreloadAmount); // Preload more objects if the pool is empty
        }

        Node obj = pool[pool.Count - 1];
        pool.RemoveAt(pool.Count - 1);
        obj.QueueFree(); // Ensure the object is not part of the scene
        return obj;
    }

   

    public void ReturnObject(Node obj)
    {
        pool.Add(obj);
        obj.QueueFree(); // Ensure the object is not part of the scene
    }

    private void PreloadObjects(int count)
    {
        for (int i = 0; i < count; i++)
        {
            Node obj = PooledObject.Instantiate();
            obj.Name = PooledObject.Instantiate().Name + "_" + i;
            //obj.Visible = false; // Optionally hide the object
            pool.Add(obj);
        }
    }
}