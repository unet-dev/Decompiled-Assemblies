using System;

namespace Apex.AI
{
	public struct ScoredOption<TOption>
	{
		private TOption _option;

		private float _score;

		public TOption option
		{
			get
			{
				return this._option;
			}
		}

		public float score
		{
			get
			{
				return this._score;
			}
		}

		public ScoredOption(TOption option, float score)
		{
			this._option = option;
			this._score = score;
		}
	}
}