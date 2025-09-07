using System;

[AttributeUsage(AttributeTargets.Class, Inherited = true)]
public class HasSortingLayer : Attribute
{
	private string[] _names;

	public string[] Names => _names;

	public HasSortingLayer(params string[] names)
	{
		_names = names;
	}
}
