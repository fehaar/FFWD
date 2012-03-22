using System;
using UnityEngine;
using System.Collections.Generic;
using FluentAssertions;

public class CloningAGameObject : MonoBehaviour {

	bool? tested;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (!tested.HasValue) {
			tested = true;
			
			GameObject go = new GameObject();
			CloningHelper help = go.AddComponent<CloningHelper>();
			help.val = 100;
			help.nm = "Tested";
			help.reference = gameObject;
			help.referenceList = new List<GameObject>();
			help.referenceList.Add(gameObject);
			help.referenceArray = new GameObject[] { gameObject };
			help.objReference = new CloneObject();
			help.objReferenceList = new List<CloneObject>();
			help.objReferenceList.Add(help.objReference);
			help.objReferenceArray = new CloneObject[] { help.objReference };
			help.intList = new List<int>() { 1, 2, 3 };
			help.strList = new List<string>() { "One", "Two" };
			help.enumList = new List<EnumTest>() { EnumTest.ON };
			help.intArray = new int[] { 1, 2, 3 };
			help.stringArray = new string[] { "One", "Two" };
			help.enumArray = new EnumTest[] { EnumTest.ON };
			
			CloningHelper helpClone = (CloningHelper)Instantiate(help);
			
			try {
				helpClone.val.Should().Be(help.val);
				helpClone.nm.Should().Be(help.nm);
				helpClone.reference.Should().Be(gameObject);
				((object)helpClone.referenceList).Should().NotBe(help.referenceList);
				helpClone.referenceList.Should().NotBeEmpty();
				helpClone.referenceList.Should().Contain(gameObject);
				helpClone.referenceArray.Should().NotBeNull();
				helpClone.referenceArray.Should().NotBeEmpty();
				helpClone.referenceArray.Should().Contain(gameObject);
				helpClone.objReference.Should().NotBe(help.objReference);
				helpClone.objReference.Should().BeNull();
				helpClone.objReferenceArray.Should().BeNull();
				((object)helpClone.objReferenceList).Should().NotBe(help.objReferenceList);
				((object)helpClone.objReferenceList).Should().BeNull();
				((object)helpClone.intList).Should().NotBe(help.intList);				
				((object)helpClone.intList).Should().NotBeNull();
				helpClone.intList.Count.Should().Be(help.intList.Count);
				((object)helpClone.strList).Should().NotBe(help.strList);
				((object)helpClone.strList).Should().NotBeNull();
				helpClone.strList.Count.Should().Be(help.strList.Count);
				((object)helpClone.enumList).Should().NotBe(help.enumList);
				((object)helpClone.enumList).Should().NotBeNull();
				helpClone.enumList.Count.Should().Be(help.enumList.Count);
				((object)helpClone.intArray).Should().NotBe(help.intList);
				((object)helpClone.intArray).Should().NotBeNull();
				((object)helpClone.stringArray).Should().NotBe(help.strList);
				((object)helpClone.stringArray).Should().NotBeNull();
				((object)helpClone.enumArray).Should().NotBe(help.enumList);
				((object)helpClone.enumArray).Should().NotBeNull();
				
				Debug.Log(GetType().Name + " succeeded");
			} catch ( Exception ex ) {
				// Log the merror
				Debug.LogError(ex.Message);
			} finally {
				Destroy(go);
				Destroy(helpClone.gameObject);
			}
		}
	}
}
