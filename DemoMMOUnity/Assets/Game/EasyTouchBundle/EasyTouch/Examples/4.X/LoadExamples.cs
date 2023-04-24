using UnityEngine;
using System.Collections;

public class LoadExamples : MonoBehaviour {

	public void LoadExample(string level){
#if UNITY_5_3
#else
#pragma warning disable CS0618
		Application.LoadLevel( level );
#pragma warning restore CS0618
#endif
	}
}
