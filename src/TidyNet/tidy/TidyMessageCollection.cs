using System;
using System.Collections;

namespace TidyNet
{
	/// <summary>
	/// Collection of TidyMessages
	/// </summary>
	[Serializable]
	public class TidyMessageCollection : CollectionBase
	{
		/// <summary>
		/// Public default constructor
		/// </summary>
		public TidyMessageCollection()
		{
		}

		/// <summary>
		/// Adds a message.
		/// </summary>
		/// <param name="message">The message to add.</param>
		public void Add(TidyMessage message)
		{
			if (message.Level == MessageLevel.Error)
			{
				_errors++;
			}
			else if (message.Level == MessageLevel.Warning)
			{
				_warnings++;
			}

			InnerList.Add(message);
		}

		/// <summary> Errors - the number of errors that occurred in the most
		/// recent parse operation
		/// </summary>
		public virtual int Errors
		{
			get
			{
				return _errors;
			}
		}
		
		/// <summary> Warnings - the number of warnings that occurred in the most
		/// recent parse operation
		/// </summary>
		public virtual int Warnings
		{
			get
			{
				return _warnings;
			}
		}

		private int _errors = 0;
		private int _warnings = 0;
	}
}
