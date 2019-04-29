using System;

namespace Mono.Cecil
{
	public enum TokenType : uint
	{
		Module = 0,
		TypeRef = 16777216,
		TypeDef = 33554432,
		Field = 67108864,
		Method = 100663296,
		Param = 134217728,
		InterfaceImpl = 150994944,
		MemberRef = 167772160,
		CustomAttribute = 201326592,
		Permission = 234881024,
		Signature = 285212672,
		Event = 335544320,
		Property = 385875968,
		ModuleRef = 436207616,
		TypeSpec = 452984832,
		Assembly = 536870912,
		AssemblyRef = 587202560,
		File = 637534208,
		ExportedType = 654311424,
		ManifestResource = 671088640,
		GenericParam = 704643072,
		MethodSpec = 721420288,
		String = 1879048192
	}
}