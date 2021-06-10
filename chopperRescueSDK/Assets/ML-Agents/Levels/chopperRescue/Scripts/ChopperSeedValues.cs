using UnityEngine;

[DefaultExecutionOrder(-1000)]
public class ChopperSeedValues : MonoBehaviour
{
	public float troopReward { get; set; }
	public float enemyReward { get; set; }
	
	// TODO: there is no such parameter exposed in other scripts
	// public float LaserFireInterval { get; set; }
	public float LaserReach { get; set; }
	public float FrozenInterval { get; set; }

	public static ChopperSeedValues instance;

	private void Awake()
	{
		instance = this;
	}
}
