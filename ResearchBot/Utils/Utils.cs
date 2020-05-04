using System;

namespace ResearchBot.Util
{
    public class Utils
    {
        /// <summary>
        /// Given various objects or an array of objects, this method will choose randomily an object
        /// </summary>
        /// <typeparam name="T">The type of the objects</typeparam>
        /// <param name="choices">Various objects or an array</param>
        /// <returns>An object from the collection</returns>
        public static T Choose<T>(params T[] choices) => choices[new Random().Next(0, choices.Length)];
    }
}