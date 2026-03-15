
public struct ExperienceLevelThresholds
{
	//Orc1 = 4 Exp
	//Orc1 = 10 Exp
	//Orc1 = 15 Exp
	public const int level1Threshold = 15;
	public const int level2Threshold = 35;
	public const int level3Threshold = 60;
	public const int level4Threshold = 90;
	public const int level5Threshold = 125;
	public const int level6Threshold = 165;
	public const int level7Threshold = 210;
	public const int level8Threshold = 260;
	public const int level9Threshold = 310;
	public static readonly int[] thresholds = new int[]
	{
		level1Threshold,
		level2Threshold,
		level3Threshold,
		level4Threshold,
		level5Threshold,
		level6Threshold,
		level7Threshold,
		level8Threshold,
		level9Threshold
	};
}