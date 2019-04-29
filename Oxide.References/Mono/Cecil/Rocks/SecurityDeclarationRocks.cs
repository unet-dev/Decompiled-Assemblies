using Mono.Cecil;
using Mono.Collections.Generic;
using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
using System.Security.Permissions;

namespace Mono.Cecil.Rocks
{
	public static class SecurityDeclarationRocks
	{
		private static void CompleteSecurityAttribute(System.Security.Permissions.SecurityAttribute security_attribute, Mono.Cecil.SecurityAttribute attribute)
		{
			if (attribute.HasFields)
			{
				SecurityDeclarationRocks.CompleteSecurityAttributeFields(security_attribute, attribute);
			}
			if (attribute.HasProperties)
			{
				SecurityDeclarationRocks.CompleteSecurityAttributeProperties(security_attribute, attribute);
			}
		}

		private static void CompleteSecurityAttributeFields(System.Security.Permissions.SecurityAttribute security_attribute, Mono.Cecil.SecurityAttribute attribute)
		{
			Type type = security_attribute.GetType();
			foreach (Mono.Cecil.CustomAttributeNamedArgument field in attribute.Fields)
			{
				type.GetField(field.Name).SetValue(security_attribute, field.Argument.Value);
			}
		}

		private static void CompleteSecurityAttributeProperties(System.Security.Permissions.SecurityAttribute security_attribute, Mono.Cecil.SecurityAttribute attribute)
		{
			Type type = security_attribute.GetType();
			foreach (Mono.Cecil.CustomAttributeNamedArgument property in attribute.Properties)
			{
				type.GetProperty(property.Name).SetValue(security_attribute, property.Argument.Value, null);
			}
		}

		private static IPermission CreatePermission(SecurityDeclaration declaration, Mono.Cecil.SecurityAttribute attribute)
		{
			Type type = Type.GetType(attribute.AttributeType.FullName);
			if (type == null)
			{
				throw new ArgumentException("attribute");
			}
			System.Security.Permissions.SecurityAttribute securityAttribute = SecurityDeclarationRocks.CreateSecurityAttribute(type, declaration);
			if (securityAttribute == null)
			{
				throw new InvalidOperationException();
			}
			SecurityDeclarationRocks.CompleteSecurityAttribute(securityAttribute, attribute);
			return securityAttribute.CreatePermission();
		}

		private static PermissionSet CreatePermissionSet(SecurityDeclaration declaration)
		{
			PermissionSet permissionSets = new PermissionSet(PermissionState.None);
			foreach (Mono.Cecil.SecurityAttribute securityAttribute in declaration.SecurityAttributes)
			{
				permissionSets.AddPermission(SecurityDeclarationRocks.CreatePermission(declaration, securityAttribute));
			}
			return permissionSets;
		}

		private static System.Security.Permissions.SecurityAttribute CreateSecurityAttribute(Type attribute_type, SecurityDeclaration declaration)
		{
			System.Security.Permissions.SecurityAttribute securityAttribute;
			try
			{
				securityAttribute = (System.Security.Permissions.SecurityAttribute)Activator.CreateInstance(attribute_type, new object[] { (System.Security.Permissions.SecurityAction)declaration.Action });
			}
			catch (MissingMethodException missingMethodException)
			{
				securityAttribute = (System.Security.Permissions.SecurityAttribute)Activator.CreateInstance(attribute_type, new object[0]);
			}
			return securityAttribute;
		}

		public static PermissionSet ToPermissionSet(this SecurityDeclaration self)
		{
			PermissionSet permissionSets;
			if (self == null)
			{
				throw new ArgumentNullException("self");
			}
			if (SecurityDeclarationRocks.TryProcessPermissionSetAttribute(self, out permissionSets))
			{
				return permissionSets;
			}
			return SecurityDeclarationRocks.CreatePermissionSet(self);
		}

		public static SecurityDeclaration ToSecurityDeclaration(this PermissionSet self, Mono.Cecil.SecurityAction action, ModuleDefinition module)
		{
			if (self == null)
			{
				throw new ArgumentNullException("self");
			}
			if (module == null)
			{
				throw new ArgumentNullException("module");
			}
			SecurityDeclaration securityDeclaration = new SecurityDeclaration(action);
			Mono.Cecil.SecurityAttribute securityAttribute = new Mono.Cecil.SecurityAttribute(module.TypeSystem.LookupType("System.Security.Permissions", "PermissionSetAttribute"));
			securityAttribute.Properties.Add(new Mono.Cecil.CustomAttributeNamedArgument("XML", new CustomAttributeArgument(module.TypeSystem.String, self.ToXml().ToString())));
			securityDeclaration.SecurityAttributes.Add(securityAttribute);
			return securityDeclaration;
		}

		private static bool TryProcessPermissionSetAttribute(SecurityDeclaration declaration, out PermissionSet set)
		{
			set = null;
			if (!declaration.HasSecurityAttributes && declaration.SecurityAttributes.Count != 1)
			{
				return false;
			}
			Mono.Cecil.SecurityAttribute item = declaration.SecurityAttributes[0];
			if (!item.AttributeType.IsTypeOf("System.Security.Permissions", "PermissionSetAttribute"))
			{
				return false;
			}
			PermissionSetAttribute permissionSetAttribute = new PermissionSetAttribute((System.Security.Permissions.SecurityAction)declaration.Action);
			Mono.Cecil.CustomAttributeNamedArgument customAttributeNamedArgument = item.Properties[0];
			string value = (string)customAttributeNamedArgument.Argument.Value;
			string name = customAttributeNamedArgument.Name;
			if (name == "XML")
			{
				permissionSetAttribute.XML = value;
			}
			else
			{
				if (name != "Name")
				{
					throw new NotImplementedException(customAttributeNamedArgument.Name);
				}
				permissionSetAttribute.Name = value;
			}
			set = permissionSetAttribute.CreatePermissionSet();
			return true;
		}
	}
}