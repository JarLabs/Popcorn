﻿using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace Popcorn.Helpers
{
    /// <summary>
    /// Used to define extension methods
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Sort an observable collection of an IComparable object
        /// </summary>
        /// <typeparam name="T">The type of the object</typeparam>
        /// <param name="collection">The observable collection to sort</param>
        public static void Sort<T>(this ObservableCollection<T> collection) where T : IComparable<T>
        {
            var sorted = collection.OrderBy(x => x).ToList();
            for (var i = 0; i < sorted.Count(); i++)
                collection.Move(collection.IndexOf(sorted[i]), i);
        }

        /// <summary>
        /// Finds a Child of a given item in the visual tree. 
        /// </summary>
        /// <param name="parent">A direct parent of the queried item.</param>
        /// <typeparam name="T">The type of the queried item.</typeparam>
        /// <param name="childName">x:Name or Name of child. </param>
        /// <returns>The first parent item that matches the submitted type parameter. 
        /// If not matching item can be found, 
        /// a null parent is being returned.</returns>
        /// <remarks>
        /// http://stackoverflow.com/a/1759923/1188513
        /// </remarks>
        public static T FindChild<T>(this DependencyObject parent, string childName)
            where T : DependencyObject
        {
            if (parent == null) return null;

            T foundChild = null;

            var childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (var i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                var childType = child as T;
                if (childType == null)
                {
                    foundChild = FindChild<T>(child, childName);
                    if (foundChild != null) break;
                }
                else if (!string.IsNullOrEmpty(childName))
                {
                    var frameworkElement = child as FrameworkElement;
                    if (frameworkElement != null && frameworkElement.Name == childName)
                    {
                        foundChild = (T) child;
                        break;
                    }
                }
                else
                {
                    foundChild = (T) child;
                    break;
                }
            }

            return foundChild;
        }
    }
}