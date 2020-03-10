// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ABaseSignal.cs" company="Supyrb">
//   Copyright (c) 2019 Supyrb. All rights reserved.
// </copyright>
// <author>
//   Johannes Deml
//   public@deml.io
// </author>
// --------------------------------------------------------------------------------------------------------------------

using UnityEngine.Profiling;

namespace Supyrb
{
	public abstract class ASignal : ABaseSignal
	{
		public enum State
		{
			/// <summary>
			/// Signal was never called or is finished
			/// </summary>
			Idle,
			/// <summary>
			/// Signal is currently active and processing listener at <see cref="ASignal.currentIndex"/>
			/// </summary>
			Running,
			/// <summary>
			/// Signal is paused and can be continued at the next index by calling <see cref="ASignal.Continue"/>
			/// </summary>
			Paused,
			/// <summary>
			/// Signal was consumed at <see cref="ASignal.currentIndex"/>
			/// </summary>
			Consumed
		}
		
		protected int currentIndex;
		private State state;


		/// <summary>
		/// Number of registered listeners
		/// </summary>
		public abstract int ListenerCount { get; }
		

		protected ASignal() : base()
		{
			this.currentIndex = 0;
			this.state = State.Idle;
		}
		
		/// <summary>
		/// Removes all registered listeners
		/// </summary>
		public abstract void Clear();

		/// <summary>
		/// Pause dispatching
		/// Dispatching can be continued by calling <see cref="Continue"/> 
		/// </summary>
		public void Pause()
		{
			this.state = State.Paused;
		}

		/// <summary>
		/// Continue dispatching
		/// Only applicable if <see cref="Pause"/> was called before
		/// </summary>
		public void Continue()
		{
			if (state != State.Paused)
			{
				return;
			}

			BeginSignalProfilerSample("Continue Signal");

			currentIndex++;
			state = State.Running;
			Run();
			
			EndSignalProfilerSample();
		}

		/// <summary>
		/// Consume the signal, no further listener will receive the dispatched signal
		/// </summary>
		public void Consume()
		{
			state = State.Consumed;
		}
		
		protected void StartDispatch()
		{
			currentIndex = 0;
			state = State.Running;
			
			Run();
		}

		private void Run()
		{
			while (true)
			{
				if (currentIndex >= ListenerCount)
				{
					OnFinish();
					return;
				}

				Invoke(currentIndex);
				
				if (state != State.Running)
				{
					return;
				}
				currentIndex++;
			}
		}

		protected void AddListenerAt(int index)
		{
			if (state == State.Idle)
			{
				return;
			}

			if (currentIndex >= index)
			{
				currentIndex++;
			}
		}

		protected void RemoveListenerAt(int index)
		{
			if (state == State.Idle)
			{
				return;
			}

			if (currentIndex >= index)
			{
				currentIndex--;
			}
		}

		protected virtual void OnFinish()
		{
			state = State.Idle;
		}

		protected void BeginSignalProfilerSample(string sampleName)
		{
			Profiler.BeginSample(sampleName);
			Profiler.BeginSample(this.GetType().FullName);
		}
		
		protected void EndSignalProfilerSample()
		{
			Profiler.EndSample();
			Profiler.EndSample();
		}

		protected abstract void Invoke(int index);

		/// <inheritdoc />
		public override string ToString()
		{
			
			return string.Format("Signal {0}: {1} Listeners, State {2}, Index {3}", 
				this.GetType().Name, ListenerCount, state, currentIndex);
		}
	}
}