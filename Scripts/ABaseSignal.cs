// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ABaseSignal.cs" company="Supyrb">
//   Copyright (c) 2019 Supyrb. All rights reserved.
// </copyright>
// <author>
//   Johannes Deml
//   send@johannesdeml.com
// </author>
// --------------------------------------------------------------------------------------------------------------------

namespace Supyrb
{
	/// <summary>
	/// Base interface for Signals
	/// </summary>
	public interface ISignal
	{
		string Hash { get; }
	}

	public abstract class ABaseSignal : ISignal
	{
		protected int currentIndex;
		private bool consumed;
		private bool paused;
		private bool finished;

		protected string _hash;

		/// <summary>
		/// Unique id for this signal
		/// </summary>
		public string Hash
		{
			get
			{
				if (string.IsNullOrEmpty(_hash))
				{
					_hash = this.GetType().ToString();
				}

				return _hash;
			}
		}

		public abstract int ListenerCount { get; }

		protected ABaseSignal()
		{
			this.currentIndex = 0;
			this.consumed = false;
			this.paused = false;
			this.finished = true;
		}

		protected void CleanupForDispatch()
		{
			currentIndex = 0;
			consumed = false;
			paused = false;
			finished = false;
		}

		protected void Run()
		{
			if (paused || finished || consumed)
			{
				return;
			}

			if (currentIndex >= ListenerCount)
			{
				OnFinish();
				return;
			}

			Invoke(currentIndex);
			currentIndex++;
			Run();
		}

		protected virtual void OnFinish()
		{
			finished = true;
		}

		protected abstract void Invoke(int index);

		public void Pause()
		{
			paused = true;
		}

		public void Continue()
		{
			paused = false;
			Run();
		}

		public void Consume()
		{
			consumed = true;
		}
	}
}