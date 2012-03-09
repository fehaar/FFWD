using PressPlay.FFWD;
using PressPlay.FFWD.Components;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
#if !XBOX
using FluentAssertions;
#endif

public class CloningAGameObject : MonoBehaviour {

	bool? tested;

	// Use this for initialization
	public override void Start () {
	
	}
	
	// Update is called once per frame
	public override void Update () {
#if !XBOX
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
				Debug.Log(GetType().Name + " succeeded");
			} catch ( Exception ex ) {
				// Log the merror
				Debug.LogError(ex.Message);
			} finally {
				Destroy(go);
				Destroy(helpClone.gameObject);
			}
		}
#endif
    }
}
