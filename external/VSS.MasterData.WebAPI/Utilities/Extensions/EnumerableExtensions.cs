﻿using System.Collections.Generic;

namespace VSS.MasterData.WebAPI.Utilities.Extensions
{
	public static class EnumerableExtensions
	{
		public static IEnumerable<IEnumerable<T>> Batch<T>(this IEnumerable<T> collection, int batchSize)
		{
			var nextbatch = new List<T>(batchSize);
			foreach (T item in collection)
			{
				nextbatch.Add(item);
				if (nextbatch.Count == batchSize)
				{
					yield return nextbatch;
					nextbatch = new List<T>();
				}
			}

			if (nextbatch.Count > 0)
				yield return nextbatch;
		}
	}
}
