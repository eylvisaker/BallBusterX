﻿//
//    Copyright (c) 2006-2018 Erik Ylvisaker
//
//    Permission is hereby granted, free of charge, to any person obtaining a copy
//    of this software and associated documentation files (the "Software"), to deal
//    in the Software without restriction, including without limitation the rights
//    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//    copies of the Software, and to permit persons to whom the Software is
//    furnished to do so, subject to the following conditions:
//
//    The above copyright notice and this permission notice shall be included in all
//    copies or substantial portions of the Software.
//
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//    SOFTWARE.
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace AgateLib.Mathematics
{
	/// <summary>
	/// LINQ-like extensions for enumerables of mathematics types.
	/// </summary>
	public static class LinqExtensions
	{
		/// <summary>
		/// Sums the vectors in the enumerable.
		/// </summary>
		/// <param name="points"></param>
		/// <returns></returns>
		public static Vector2 Sum(this IEnumerable<Vector2> points)
		{
			Vector2 result = Vector2.Zero;

			foreach (var point in points)
				result += point;

			return result;
		}
		/// <summary>
		/// Averages the vectors in the enumerable.
		/// </summary>
		/// <param name="points"></param>
		/// <returns></returns>
		public static Vector2 Average(this IEnumerable<Vector2> points)
		{
			Vector2 sum = Vector2.Zero;
			long count = 0;

			foreach (var point in points)
			{
				sum += point;
				count++;
			}

			return sum / count;
		}
	}
}
