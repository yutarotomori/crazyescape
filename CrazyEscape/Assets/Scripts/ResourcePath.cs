using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourcePath : ScriptableObject
{
	static private readonly string thisResourcePath = "Data/ResourcePath";
	static private ResourcePath instanceValue;
	static public ResourcePath instance {
		get {
			if (instanceValue == null) {
				instanceValue = Resources.Load<ResourcePath> (thisResourcePath);
			}
			return instanceValue;
		}
	}


	static public string _ostrich {
		get {
			return instance.m_Ostrich;
		}
	}

	static public string _enemy {
		get {
			return instance.m_Enemy;
		}
	}

	static public string _rock {
		get {
			return instance.m_Rock;
		}
	}

	static public string _corn {
		get {
			return instance.m_Corn;
		}
	}


	[SerializeField]
	private string m_Ostrich;

	[SerializeField]
	private string m_Enemy;

	[SerializeField]
	private string m_Rock;

	[SerializeField]
	private string m_Corn;
}
