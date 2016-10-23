using UnityEngine;

public class RichString
{
	static	public	string	BoldString (string str)
	{
		return	"<b>"+str+"</b>";
	}

	static	public	string	ItalicString (string str)
	{
		return	"<i>"+str+"</i>";
	}

	static	public	string	SizeString (string str, int size)
	{
		return	"<size="+size+">"+str+"</size>";
	}

	static	public	string	ColorString (string str, Color color)
	{
		int r = Mathf.Clamp ((int)(color.r*255.0f), 0, 255);
		int g = Mathf.Clamp ((int)(color.g*255.0f), 0, 255);
		int b = Mathf.Clamp ((int)(color.b*255.0f), 0, 255);
		string colorCode = string.Format ("#{0:x2}{1:x2}{2:x2}",r,g,b);

		return	"<color="+colorCode+">"+str+"</color>";
	}

	static	public	string	ColorString (string str, string colorCode)
	{
		return	"<color="+colorCode+">"+str+"</color>";
	}

	public	RichString ()
	{
		new RichString ("", false, false, 0, Color.black);
	}

	public	RichString (string str, bool bold, bool italic, int size, Color color)
	{
		value		= str;
		isBold		= bold;
		isItalic	= italic;
		this.size	= size;
		this.color	= color;
	}

	public	string	value {
		get;
		set;
	}
	public	bool	isBold {
		get;
		set;
	}
	public	bool	isItalic {
		get;
		set;
	}
	public	int		size {
		get;
		set;
	}
	public	string	richString {
		get {
			return	GetValue ();
		}
	}

	public	string	colorCode {
		get {
			int r = Mathf.Clamp ((int)(color.r*255.0f), 0, 255);
			int g = Mathf.Clamp ((int)(color.g*255.0f), 0, 255);
			int b = Mathf.Clamp ((int)(color.b*255.0f), 0, 255);
			return	string.Format ("#{0:x2}{1:x2}{2:x2}",r,g,b);
		}
	}
	public	Color	color {
		get;
		set;
	}

	public	string	GetValue ()
	{
		string	str	= value;
		if (isBold) {
			str	= RichString.BoldString (str);
		}

		if (isItalic) {
			str	= RichString.ItalicString (str);
		}

		str	= RichString.SizeString (str, size);
		str	= RichString.ColorString (str, colorCode);

		return	str;
	}
}
