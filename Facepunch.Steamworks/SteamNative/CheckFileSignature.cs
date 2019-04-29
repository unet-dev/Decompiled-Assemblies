using System;

namespace SteamNative
{
	internal enum CheckFileSignature
	{
		InvalidSignature,
		ValidSignature,
		FileNotFound,
		NoSignaturesFoundForThisApp,
		NoSignaturesFoundForThisFile
	}
}