using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class ExtensionMethods
{
    public static bool CloseTo(this float thisInstance, float Input, float offset)
    {
        float outputObject;
        if (thisInstance.Equals(Input))
            return true;

        if (thisInstance > Input)        
            outputObject = thisInstance - Input;        
        else        
            outputObject = Input - thisInstance;        

        if (outputObject <= offset)        
            return true;        
        else
            return false;
    }
    public static UnityEngine.GameObject ReturnChildObject(this UnityEngine.GameObject thisGameObject, string Name)
    {
        System.Collections.Generic.List<UnityEngine.GameObject> Children = new System.Collections.Generic.List<UnityEngine.GameObject>();
        int _ChildObjectIndex = thisGameObject.transform.childCount;

        for (int i = 0; i < _ChildObjectIndex; i++)
        {
            Children.Add(thisGameObject.transform.GetChild(_ChildObjectIndex).gameObject);
        }

        foreach (UnityEngine.GameObject _object in Children)
        {
            if (_object.name.ToLower() == Name.ToLower())
            {
                return _object;
            }
        }
        throw new System.Exception("Could not find specific object with the current name");
    }
    public static UnityEngine.Transform[] ReturnAllObjectsRelatedToCurrent(this UnityEngine.Transform _CurrentTransform)
    {
        System.Collections.Generic.List<UnityEngine.Transform> Children = new System.Collections.Generic.List<UnityEngine.Transform>();

        int index = _CurrentTransform.root.childCount;
        for (int i = 0; i < index; i++)
        {
            Children.Add(_CurrentTransform.root.GetChild(i));
        }
        return Children.ToArray();
    }
    public static bool Contains<T>(this T[] Array, T Object)
        
    {
        for (int i = 0; i < Array.Length; i++)
            if (Array[i].Equals(Object))
                return true;        
        return false;
    }
    public static T[] GetAllObjectWihthinRange<T>(this T[] Array, T Origin, float Distance)
        where T : Transform
    {
        List<T> TypeList = new List<T>();
        for (int i = 0; i < Array.Length; i++)
        {
            if (Vector3.Distance(Array[i].position,Origin.position) <= Distance)
            {
                TypeList.Add(Array[i]);
            }
        }
        return TypeList.ToArray();
    }
    public static ComponentType[] ReturnAllComponentsTypeInArray<Array, ComponentType>(this Array[] array)
        where ComponentType : Component
        where Array : Component
    {
        List<ComponentType> CompTypeList = new List<ComponentType>();
        for (int i = 0; i < array.Length; i++)
        {
            ComponentType CompType = array[i].GetComponent<ComponentType>();
            if (CompType != null)
                CompTypeList.Add(CompType);
        }
        
        return CompTypeList.ToArray() ?? default(ComponentType[]);
    }
}
public static class ExtraMethods
{
    public static T[] ReturnAllComponetsTypeInGameobjectsArray<T>(GameObject[] arr)
    {
        List<T> TypeList = new List<T>();
        for (int i = 0; i < arr.Length; i++)
        {
            T newType = arr[i].GetComponent<T>();
            if (newType != null)
            {                
                TypeList.Add(newType);
            }
        }
        return TypeList.ToArray();
    }
    public static Transform[] CheckWithinRangeOf(Transform[] arr, Transform Origin, float range)
    {
        List<Transform> transArray = new List<Transform>();
        for (int i = 0; i < arr.Length; i++)
        {
            if (arr[i].Equals(Origin))            
                continue;
            if (Vector3.Distance(arr[i].position,Origin.position) <= range)
            {
                transArray.Add(arr[i]);
            }
        }
        return transArray.ToArray() ?? default(Transform[]);
    }
    public static T[] CheckWithinRangeOf<T>(T[] arr, T Origin, float range)
        where T : Component
    {
        List<T> TList = new List<T>();
        for (int i = 0; i < arr.Length; i++)
        {
            if (arr[i].Equals(Origin))
                continue;
            if (Vector3.Distance(arr[i].transform.position, Origin.transform.position) <= range)
            {
                TList.Add(arr[i]);
            }
        }

        return TList.ToArray() ?? default(T[]);
    }
}