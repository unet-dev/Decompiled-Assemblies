using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

namespace Facepunch.UI
{
	public class ESPPlayerInfo : MonoBehaviour
	{
		public Vector3 WorldOffset;

		protected UnityEngine.UI.Text Text;

		protected UnityEngine.UI.Image Image;

		public Gradient gradientNormal;

		public Gradient gradientTeam;

		public QueryVis visCheck;

		public BasePlayer Entity
		{
			get;
			set;
		}

		public ESPPlayerInfo()
		{
		}
	}
}