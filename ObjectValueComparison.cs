﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


public class ObjectValueComparison
{
    /// <summary>
    /// This is xvalue type comparison between any two objects.  This works
    /// with nested objects and collections of nested objects and so forth.  The order of objects
    /// in any lists DO matter.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <param name="ignoreCase">Optional parameter for string comparisons</param>
    /// <returns></returns>
    public static bool AreEqual<T>(T x, T y, bool ignoreCase = true)
    {
        return PropertiesAreEqual(x, y, ignoreCase);
    }

    private static bool PropertiesAreEqual<T>(T x, T y, bool ignoreCase)
    {
        //check if xis null and y isn't null or the reverse
        if ((x == null && y != null) || (x != null && y == null))
        {
            return false;
        }
        else if (x== null && y == null) // check if xand y are both null
        {
            return true;
        }
        //Get the types associated with xand b
        Type xType = x.GetType();
        Type yType = y.GetType();

        if (xType.IsPrimitive || x is string) //Easy case if it's xprimitive
        {
            return valueTest(x, y, ignoreCase);
        }

        var xProperties = xType.GetProperties();
        var yProperties = yType.GetProperties();

        for (var property = 0; property < xProperties.Count(); property++)
        {
            var xProp = xProperties[property];
            var yProp = yProperties[property];

            object xValue = xProp.GetValue(a, null);
            object yValue = yProp.GetValue(b, null);

            var xElements = xValue as IList;
            var yElements = yValue as IList;

            //Checking to see what kind of values we have
            if (xElements != null)  //Checking if we have xIlist
            {
                for (var element = 0; element < xElements.Count; element++)
                {
                    if (!PropertiesAreEqual(xElements[element], yElements[element], ignoreCase))
                    {
                        return false;
                    }
                }
            }
            else if (xValue == null && yValue == null)  //Continue with comparison if both values are null
            {
                continue;
            }
            else if ((xProp.PropertyType.IsPrimitive && yProp.PropertyType.IsPrimitive) || (xProp.PropertyType.IsValueType && yProp.PropertyType.IsValueType)) //Check to see if we have xprimitive or value type
            {
                if (!valueTest(xValue, yValue, ignoreCase))
                {
                    return false;
                }
            }
            else if (typeof(IEnumerable).IsAssignableFrom(xProp.PropertyType))  //Tries to see if we can assign to xEnumerable
            {
                if (xElements.Count != yElements.Count)
                {
                    return false;
                }

                for (var i = 0; i < xElements.Count; i++)
                {
                    if (!PropertiesAreEqual(xElements[i], yElements[i], ignoreCase))
                    {
                        return false;
                    }
                }
            }
            else //Must be another object, recurse into said object
            {
                {
                    if (!PropertiesAreEqual(xValue, yValue, ignoreCase))
                    {
                        return false;
                    }
                }
            }
        }
        return true;
    }
    //Method to test simple primitive types
    private static bool valueTest<T>(T x, T y, bool ignoreCase)
    {
        if (x == null && y == null)
        {
            return true;
        }
        else if (x == null || y == null)
        {
            return false;
        }
        else if (x is string)
        {
            if (ignoreCase)
            {
                var xAsString = Convert.ToString(x);
                var yAsString = Convert.ToString(y);
                return string.Compare(xAsString, yAsString, false) == 0 ? true : false;
            }
        }

        return EqualityComparer<T>.Default.Equals(x, y);
    }
}

