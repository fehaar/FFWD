using UnityEngine;
using System.Collections.Generic;

public class CloningHelper : MonoBehaviour {
	
	public int val = 1;
	public string nm = "Test me";
	public GameObject reference;
	public List<GameObject> referenceList;
	public GameObject[] referenceArray;
	public EnumTest[] enumArray;
	public int[] intArray;
	public string[] stringArray;
	public CloneObject objReference;
	public List<CloneObject> objReferenceList;
	public CloneObject[] objReferenceArray;
	public List<EnumTest> enumList;
	public List<int> intList;
	public List<string> strList;
}

public class CloneObject {}

public enum EnumTest
{
	ON,
	OFF
}