using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quadtree<T> where T : IPositionable
{
    QuadtreeNode<T> root;

    public Quadtree()
    {
        
    }

    public void Insert(T item)
    {

    }

    public List<T> Query(Rectf bounds)
    {
        return null;
    }
}

public class QuadtreeNode<T>
{
    T data;

    public QuadtreeNode(T data)
    {
        this.data = data;
    }
}

public interface IPositionable
{
    Vector2f getPosition();
}
