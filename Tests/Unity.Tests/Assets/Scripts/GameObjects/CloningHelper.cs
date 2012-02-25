using UnityEngine;
using System.Collections.Generic;

public class CloningHelper : MonoBehaviour {
	
	public int val = 1;
	public string nm = "Test me";
	public GameObject reference;
	public List<GameObject> referenceList;
	public GameObject[] referenceArray;
	public CloneObject objReference;
	public List<CloneObject> objReferenceList;
	public CloneObject[] objReferenceArray;
}

public class CloneObject {}
