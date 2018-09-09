// Amplify Occlusion 2 - Robust Ambient Occlusion for Unity
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;
using UnityEngine;

namespace AmplifyOcclusion
{
[Serializable]
public class VersionInfo
{
	public const byte Major = 2;
	public const byte Minor = 0;
	public const byte Release = 3;
	public const byte Revision = 0;

	public static string StaticToString()
	{
		return string.Format( "{0}.{1}.{2}r{3}", Major, Minor, Release, Revision );
	}

	public override string ToString()
	{
		return string.Format( "{0}.{1}.{2}r{3}", m_major, m_minor, m_release, Revision );
	}

	public int Number { get { return m_major * 100 + m_minor * 10 + m_release; } }

	[SerializeField] private int m_major;
	[SerializeField] private int m_minor;
	[SerializeField] private int m_release;

	VersionInfo()
	{
		m_major = Major;
		m_minor = Minor;
		m_release = Release;
	}

	VersionInfo( byte major, byte minor, byte release )
	{
		m_major = major;
		m_minor = minor;
		m_release = release;
	}

	public static VersionInfo Current()
	{
		return new VersionInfo( Major, Minor, Release );
	}

	public static bool Matches( VersionInfo version )
	{
		return ( Major == version.m_major ) && ( Minor == version.m_minor ) && ( Release == version.m_release );
	}
}
}
