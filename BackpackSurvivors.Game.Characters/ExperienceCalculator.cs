using System.Collections.Generic;
using BackpackSurvivors.Game.Core;
using BackpackSurvivors.System;

namespace BackpackSurvivors.Game.Characters;

public class ExperienceCalculator
{
	private Dictionary<int, int> _experienceTable;

	public ExperienceCalculator()
	{
		_experienceTable = new Dictionary<int, int>();
		_experienceTable.Add(1, 100);
		_experienceTable.Add(2, 200);
		_experienceTable.Add(3, 300);
		_experienceTable.Add(4, 400);
		_experienceTable.Add(5, 500);
		_experienceTable.Add(6, 500);
		_experienceTable.Add(7, 500);
		_experienceTable.Add(8, 500);
		_experienceTable.Add(9, 500);
		_experienceTable.Add(10, 500);
		_experienceTable.Add(11, 750);
		_experienceTable.Add(12, 750);
		_experienceTable.Add(13, 750);
		_experienceTable.Add(14, 750);
		_experienceTable.Add(15, 750);
		_experienceTable.Add(16, 750);
		_experienceTable.Add(17, 750);
		_experienceTable.Add(18, 750);
		_experienceTable.Add(19, 750);
		_experienceTable.Add(20, 750);
		_experienceTable.Add(21, 1000);
		_experienceTable.Add(22, 1000);
		_experienceTable.Add(23, 1000);
		_experienceTable.Add(24, 1000);
		_experienceTable.Add(25, 1000);
		_experienceTable.Add(26, 1000);
		_experienceTable.Add(27, 1000);
		_experienceTable.Add(28, 1000);
		_experienceTable.Add(29, 1000);
		_experienceTable.Add(30, 1000);
		_experienceTable.Add(31, 2000);
		_experienceTable.Add(32, 2000);
		_experienceTable.Add(33, 2000);
		_experienceTable.Add(34, 2000);
		_experienceTable.Add(35, 2000);
		_experienceTable.Add(36, 2000);
		_experienceTable.Add(37, 2000);
		_experienceTable.Add(38, 2000);
		_experienceTable.Add(39, 2000);
		_experienceTable.Add(40, 2000);
		_experienceTable.Add(41, 4000);
		_experienceTable.Add(42, 4000);
		_experienceTable.Add(43, 4000);
		_experienceTable.Add(44, 4000);
		_experienceTable.Add(45, 4000);
		_experienceTable.Add(46, 4000);
		_experienceTable.Add(47, 4000);
		_experienceTable.Add(48, 4000);
		_experienceTable.Add(49, 4000);
		_experienceTable.Add(50, 4000);
		_experienceTable.Add(51, 10000);
		_experienceTable.Add(52, 10000);
		_experienceTable.Add(53, 10000);
		_experienceTable.Add(54, 10000);
		_experienceTable.Add(55, 10000);
		_experienceTable.Add(56, 10000);
		_experienceTable.Add(57, 10000);
		_experienceTable.Add(58, 10000);
		_experienceTable.Add(59, 10000);
		_experienceTable.Add(60, 10000);
		_experienceTable.Add(61, 15000);
		_experienceTable.Add(62, 15000);
		_experienceTable.Add(63, 15000);
		_experienceTable.Add(64, 15000);
		_experienceTable.Add(65, 15000);
		_experienceTable.Add(66, 15000);
		_experienceTable.Add(67, 15000);
		_experienceTable.Add(68, 15000);
		_experienceTable.Add(69, 15000);
		_experienceTable.Add(70, 15000);
		_experienceTable.Add(71, 20000);
		_experienceTable.Add(72, 20000);
		_experienceTable.Add(73, 20000);
		_experienceTable.Add(74, 20000);
		_experienceTable.Add(75, 20000);
		_experienceTable.Add(76, 20000);
		_experienceTable.Add(77, 20000);
		_experienceTable.Add(78, 20000);
		_experienceTable.Add(79, 20000);
		_experienceTable.Add(80, 20000);
		_experienceTable.Add(81, 40000);
		_experienceTable.Add(82, 40000);
		_experienceTable.Add(83, 40000);
		_experienceTable.Add(84, 40000);
		_experienceTable.Add(85, 40000);
		_experienceTable.Add(86, 40000);
		_experienceTable.Add(87, 40000);
		_experienceTable.Add(88, 40000);
		_experienceTable.Add(89, 40000);
		_experienceTable.Add(90, 100000);
		_experienceTable.Add(91, 100000);
		_experienceTable.Add(92, 100000);
		_experienceTable.Add(93, 100000);
		_experienceTable.Add(94, 100000);
		_experienceTable.Add(95, 100000);
		_experienceTable.Add(96, 100000);
		_experienceTable.Add(97, 100000);
		_experienceTable.Add(98, 100000);
		_experienceTable.Add(99, 100000);
		_experienceTable.Add(100, 100000);
	}

	public float GetExperienceNeededForLevel(int level)
	{
		if (GameDatabase.IsDemo && level >= SingletonController<GameDatabase>.Instance.GameDatabaseSO.MaxLevel)
		{
			return 1E+09f;
		}
		if (_experienceTable.ContainsKey(level))
		{
			return _experienceTable[level];
		}
		return -1f;
	}

	public int GetTotalExperienceNeededForLevel(int level)
	{
		int num = 0;
		if (_experienceTable.ContainsKey(level))
		{
			foreach (KeyValuePair<int, int> item in _experienceTable)
			{
				if (item.Key <= level)
				{
					num += item.Value;
				}
			}
		}
		return num;
	}
}
