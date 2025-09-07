using System;

namespace EditorCools;

[AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
public sealed class ButtonAttribute : Attribute
{
	public readonly string Name;

	public readonly string Row;

	public readonly float Space;

	public readonly bool HasRow;

	public ButtonAttribute(string name = null, string row = null, float space = 0f)
	{
		Row = row;
		HasRow = !string.IsNullOrEmpty(Row);
		Name = name;
		Space = space;
	}
}
