﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace MS.Katusha.Web.Helpers.Generators
{
    public static class GeneratorHelper
    {
        public static Random RND = new Random((int) DateTime.Now.Ticks & 0x0000FFFF);

        public static int RandomNumber(int min, int max)
        {
            return RND.Next(min, max);
        }

        /// <summary>
        /// Generates a random string with the given length
        /// </summary>
        /// <param name="size">Size of the string</param>
        /// <param name="lowerCase">If true, generate lowercase string</param>
        /// <returns>Random string</returns>
        public static string RandomString(int size, bool lowerCase)
        {
            StringBuilder builder = new StringBuilder();
            //Random random = new Random();
            char ch;
            for (int i = 0; i < size; i++) {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * RND.NextDouble() + 65)));
                builder.Append(ch);
            }
            if (lowerCase)
                return builder.ToString().ToLower();
            return builder.ToString();
        }
    }
}