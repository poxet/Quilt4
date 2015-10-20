using System;
using System.Collections;
using System.Collections.Generic;

namespace Quilt4.MultiRepository.Compare
{
    public static class CompareExtensions
    {
        [Flags]
        public enum CompareMode
        {
            Standard = 0x0,
            IgnoreType = 0x1,
        }

        public static IEnumerable<IDiff> Compare(this object s1, object s2, CompareMode compareMode = CompareMode.Standard)
        {
            return DoCompare(null, null, s1, s2, compareMode, new List<object>());
        }

        private static IEnumerable<IDiff> DoCompare(string parentObject1Name, string parentObject2Name, object s1, object s2, CompareMode compareMode, List<object> visited)
        {
            if (ReferenceEquals(s1, s2))
                yield break;

            if (s1 == null && s2 == null)
                yield break;

            if (s1 == null)
                yield return new Diff(parentObject1Name, parentObject2Name, string.Format("One item has a value and the other is null."));
            else if (s2 == null)
                yield return new Diff(parentObject1Name, parentObject2Name, string.Format("One item is null and the other has a value."));
            else
            {
                var tp1 = s1.GetType();
                var tp2 = s2.GetType();

                var item1Name = parentObject1Name != null ? string.Format("{0}.{1}", parentObject1Name, s1.GetType().Name) : s1.GetType().Name;
                var item2Name = parentObject1Name != null ? string.Format("{0}.{1}", parentObject2Name, s2.GetType().Name) : s2.GetType().Name;

                if (tp1 != tp2 && (compareMode & CompareMode.IgnoreType) != CompareMode.IgnoreType)
                    yield return new DifferentTypes(item1Name, tp1, tp2);
                else
                {
                    if (s1 as string != null || s2 as string != null)
                    {
                        if (s1.ToString() != s2.ToString())
                            yield return new Diff(item1Name, item2Name, string.Format("One string has value {0} and the other string has value {1}.", s1, s2));
                    }
                    else if (s1 is DateTime || s2 is DateTime)
                    {
                        foreach (var diff2 in CompareDateTimes(s1, s2, item1Name, item2Name))
                            yield return diff2;
                    }
                    else if (s1 is int || s2 is int)
                    {
                        foreach (var diff2 in CompareInts(s1, s2, item1Name, item2Name))
                            yield return diff2;
                    }
                    else if (s1 is decimal || s2 is decimal)
                    {
                        foreach (var diff2 in CompareDecimals(s1, s2, item1Name, item2Name))
                            yield return diff2;
                    }
                    else if (tp1.IsValueType)
                    {
                        foreach (var diff in CompareValueTypes(item1Name, item2Name, s1, s2, compareMode, visited))
                            yield return diff;
                    }
                    else
                    {
                        foreach (var diff1 in CompareReferenceTypes(item1Name, item2Name, s1, s2, compareMode, visited))
                            yield return diff1;
                    }
                }
            }
        }

        private static IEnumerable<IDiff> CompareInts(object s1, object s2, string item1Name, string item2Name)
        {
            int i1;
            if (!int.TryParse(s1.ToString(), out i1))
                yield return new Diff(item1Name, item2Name, string.Format("Cannot parse {0} with value {1} to an int to compare with {2} with value {3}.", item1Name, s1, item2Name, s2));
            else
            {
                int i2;
                if (!int.TryParse(s2.ToString(), out i2))
                    yield return new Diff(item1Name, item2Name, string.Format("Cannot parse {0} with value {1} to an int to compare with {2} with value {3}.", item2Name, s2, item1Name, s1));
                else if (i1 - i2 != 0)
                    yield return new Diff(item1Name, item2Name, string.Format("Value of int {0} is {1} and differs from {2} with {3} by {4}.", item1Name, s1, item2Name, s2, Math.Abs(i1 - i2)));
            }
        }

        private static IEnumerable<IDiff> CompareDecimals(object s1, object s2, string item1Name, string item2Name)
        {
            decimal i1;
            if (!decimal.TryParse(s1.ToString(), out i1))
                yield return new Diff(item1Name, item2Name, string.Format("Cannot parse {0} with value {1} to an decimal to compare with {2} with value {3}.", item1Name, s1, item2Name, s2));
            else
            {
                decimal i2;
                if (!decimal.TryParse(s2.ToString(), out i2))
                    yield return new Diff(item1Name, item2Name, string.Format("Cannot parse {0} with value {1} to an decimal to compare with {2} with value {3}.", item2Name, s2, item1Name, s1));
                else if (i1 - i2 != 0)
                    yield return new Diff(item1Name, item2Name, string.Format("Value of decimal {0} is {1} and differs from {2} with {3} by {4}.", item1Name, s1, item2Name, s2, Math.Abs(i1 - i2)));
            }
        }

        private static IEnumerable<IDiff> CompareDateTimes(object s1, object s2, string item1Name, string item2Name)
        {
            DateTime i1;
            if (!DateTime.TryParse(s1.ToString(), out i1))
                yield return new Diff(item1Name, item2Name, string.Format("Cannot parse {0} with value {1} to an DateTime to compare with {2} with value {3}.", item1Name, s1, item2Name, s2));
            else
            {
                DateTime i2;
                if (!DateTime.TryParse(s2.ToString(), out i2))
                    yield return new Diff(item1Name, item2Name, string.Format("Cannot parse {0} with value {1} to an DateTime to compare with {2} with value {3}.", item2Name, s2, item1Name, s1));
                else if (i1 - i2 != new TimeSpan())
                    yield return new Diff(item1Name, item2Name, string.Format("Value of DateTime {0} is {1} and differs from {2} with {3} by {4} ticks.", item1Name, ((DateTime)s1).ToShortDateString() + " " + ((DateTime)s1).ToLongTimeString(), item2Name, ((DateTime)s2).ToShortDateString() + " " + ((DateTime)s2).ToLongTimeString(), Math.Abs(((DateTime)s1 - (DateTime)s2).Ticks)));
            }
        }

        private static IEnumerable<IDiff> CompareValueTypes(string item1Name, string item2Name, object s1, object s2, CompareMode compareMode, List<object> visited)
        {
            var itemHasDiffs = false;

            foreach (var diff2 in CompareMembers(item1Name, item2Name, s1, s2, compareMode, visited))
            {
                itemHasDiffs = true;
                yield return diff2;
            }

            visited.Add(s1);
            visited.Add(s2);

            if (!itemHasDiffs)
            {
                if (s1.ToString() != s2.ToString())
                    yield return new Diff(item1Name, item2Name, string.Format("The value of the item {2} is {0} and the value of the other item ({3}) is {1}.", s1, s2, item1Name, item2Name));
            }
        }

        private static IEnumerable<IDiff> CompareReferenceTypes(string item1Name, string item2Name, object s1, object s2, CompareMode compareMode, List<object> visited)
        {
            if (s1 is IEnumerable && s2 is IEnumerable)
            {
                var enumr1 = (s1 as IEnumerable).GetEnumerator();
                var enumr2 = (s2 as IEnumerable).GetEnumerator();

                while (true)
                {
                    var ptr1 = enumr1.MoveNext();
                    var ptr2 = enumr2.MoveNext();

                    if (ptr1 != ptr2)
                        yield return new Diff(item1Name, item2Name, string.Format("One value has the value {0} and the other has value {1}.", ptr1, ptr2));
                    if (!ptr1 || !ptr2)
                        yield break;

                    var data1 = enumr1.Current;
                    var data2 = enumr2.Current;

                    var diffs = DoCompare(item1Name, item2Name, data1, data2, compareMode, visited);
                    foreach (var diff in diffs)
                        yield return diff;
                }
            }
            else
            {
                foreach (var diff2 in CompareMembers(item1Name, item2Name, s1, s2, compareMode, visited))
                    yield return diff2;
            }
        }

        private static IEnumerable<IDiff> CompareMembers(string item1Name, string item2Name, object s1, object s2, CompareMode compareMode, List<object> visited)
        {
            if (visited.Contains(s1) || visited.Contains(s2))
                yield break;

            visited.Add(s1);
            visited.Add(s2);

            var fields = s1.GetType().GetFields();
            foreach (var field in fields)
            {
                var f1 = field.GetValue(s1);

                IDiff diff1 = null;
                var f2 = GetValueFromPropertyOrField(item1Name, item2Name, s2, field.Name, compareMode, out diff1);
                if (f2 == null && diff1 != null)
                    yield return diff1;
                else
                {
                    var diffs = DoCompare(item1Name, item2Name, f1, f2, compareMode, visited);

                    foreach (var diff in diffs)
                    {
                        yield return diff;
                    }
                }
            }

            var tp = s1.GetType();
            var props = tp.GetProperties();
            foreach (var prop in props)
            {
                var v1 = prop.GetValue(s1, null);
                IDiff diff1 = null;
                var v2 = GetValueFromPropertyOrField(item1Name, item2Name, s2, prop.Name, compareMode, out diff1);
                if (v2 == null && diff1 != null)
                    yield return diff1;
                else
                {
                    var diffs = DoCompare(item1Name, item2Name, v1, v2, compareMode, visited);

                    foreach (var diff in diffs)
                    {
                        yield return diff;
                    }
                }
            }
        }

        private static object GetValueFromPropertyOrField(string item1Name, string item2Name, object s2, string name, CompareMode compareMode, out IDiff diff)
        {
            diff = null;

            var prop = s2.GetType().GetProperty(name);
            if (prop != null)
            {
                var val = prop.GetValue(s2, null);
                return val;
            }

            var field = s2.GetType().GetField(name);
            if (field != null)
            {
                var val = field.GetValue(s2);
                return val;
            }

            diff = new Diff(item1Name, item2Name, string.Format("Cannot find the property or field named {0} in object of type {1}.", name, s2.GetType()));
            return null;
        }
    }

}
