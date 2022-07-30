using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
[CreateAssetMenu(fileName = "AddressableAssetStringer.asset", menuName = "Addressable/AssetString", order = 1)]
public class AddressableAssetString : ScriptableObject
{
    [SerializeField]
    public List<AssetStringPath> assetStringPathsList = new List<AssetStringPath>();
    [CanBeNull]
    public AssetStringPath[] GetAll()
    {
        if (assetStringPathsList.Count < 1)
        {
            return new AssetStringPath[1];
        }
        return assetStringPathsList.ToArray();
    }
}
[System.Serializable]
public class AssetStringPath
{
    public string uiName;

    [SerializeField]
    public UnityEngine.Object name;
    [SerializeField]
    public string pathList;

    public bool IsTypeName(string typeName)
    {
        return uiName == typeName;
    }
}