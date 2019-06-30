using System;

namespace Steamworks
{
	public enum CheckFileSignature
	{
		InvalidSignature,
		ValidSignature,
		FileNotFound,
		NoSignaturesFoundForThisApp,
		NoSignaturesFoundForThisFile
	}
}