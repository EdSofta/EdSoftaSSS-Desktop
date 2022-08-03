using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using EdSofta.Enums;
using EdSofta.Interfaces;
using EdSofta.Models;
using EdSofta.ViewModels.Collections;
using EdSofta.Views.Pages;
using HtmlAgilityPack;
using Ionic.Zip;

namespace EdSofta.ViewModels.Utility
{
    internal static class UtilityClass
    {
        public static void SetModal(this List<IModal> modalList, string name)
        {
            var activeModal = modalList.Find(x => x.getModalName() == name);
            if (activeModal == null) return;

            foreach (var modal in modalList.Where(x => x != activeModal && x.isActive()))
            {
                modal.setModalState(ModalState.Hidden);
            }

            activeModal.setModalState(!activeModal.isActive() ? ModalState.Visible : ModalState.Hidden);

        }

        public static void CloseModals(this List<IModal> modalList)
        {
            foreach (var modal in modalList.Where(x => x.isActive()))
            {
                modal.setModalState(ModalState.Hidden);
            }
        }

        public static T ConvertEnumObject<T>(object o)
        {
            T enumVal = (T)Enum.Parse(typeof(T), o.ToString());
            return enumVal;
        }

        public static double PointsToPixels(double points)
        {
            return points * (96.0 / 80.0);
        }

        public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            return new ObservableCollection<T>(source);
        }

        public static T GetChildOfType<T>(this DependencyObject depObj)
            where T : DependencyObject
        {
            if (depObj == null) return null;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
            {
                var child = VisualTreeHelper.GetChild(depObj, i);

                var result = (child as T) ?? GetChildOfType<T>(child);
                if (result != null) return result;
            }
            return null;
        }

        public static T GetParentOfType<T>(this DependencyObject depObj)
            where T : DependencyObject
        {
            if (depObj == null) return null;

            T parent = null;
            var currentParent = VisualTreeHelper.GetParent(depObj);
            while (currentParent != null && parent == null)
            {
                parent = currentParent as T;
                currentParent = VisualTreeHelper.GetParent(currentParent);
            }

            return parent;
        }


        private static readonly string[] SizeSuffixes =
            { "bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };
        public static string SizeSuffix(Int64 value, int decimalPlaces = 1)
        {
            if (decimalPlaces < 0) { throw new ArgumentOutOfRangeException("decimalPlaces"); }
            if (value < 0) { return "-" + SizeSuffix(-value); }
            if (value == 0) { return string.Format("{0:n" + decimalPlaces + "} bytes", 0); }

            // mag is 0 for bytes, 1 for KB, 2, for MB, etc.
            int mag = (int)Math.Log(value, 1024);

            // 1L << (mag * 10) == 2 ^ (10 * mag) 
            // [i.e. the number of bytes in the unit corresponding to mag]
            decimal adjustedSize = (decimal)value / (1L << (mag * 10));

            // make adjustment when the value is large enough that
            // it would round up to 1000 or more
            if (Math.Round(adjustedSize, decimalPlaces) >= 1000)
            {
                mag += 1;
                adjustedSize /= 1024;
            }

            return string.Format("{0:n" + decimalPlaces + "} {1}",
                adjustedSize,
                SizeSuffixes[mag]);
        }

        public static int ConvertMillisecondsToSeconds(this double value)
        {
            return (int)(TimeSpan.FromMilliseconds(value).TotalSeconds);
        }

        public static double ConvertSecondsToMilliseconds(this double value)
        {
            return TimeSpan.FromSeconds(value).TotalMilliseconds;
        }


        public static string HumanizeTime(this int value)
        {
            return TimeSpan.FromSeconds(value).ToUserFriendlyString();
        }

        public static string HumanizeTime(this double value)
        {
            return TimeSpan.FromSeconds(value).ToUserFriendlyString();
        }

        public static string HumanizeDateTime(this DateTime dateTime)
        {
            var days = DateTime.Now.Subtract(dateTime).Days;
            switch (days)
            {
                case 0:
                    return $"Today, {dateTime.ToShortTimeString()}";
                case 1:
                    return $"Yesterday, {dateTime.ToShortTimeString()}";
                default:
                    return days > 1 && days < 10 ? $"{days} days ago, {dateTime.ToShortTimeString()}" : dateTime.ToLongDateString();
            }
        }

        public static string ToUserFriendlyString(this TimeSpan span)
        {
            const int DaysInYear = 365;
            const int DaysInMonth = 30;

            // Get each non-zero value from TimeSpan component
            var values = new List<string>();

            // Number of years
            var days = span.Days;
            if (days >= DaysInYear)
            {
                var years = (days / DaysInYear);
                values.Add(CreateValueString(years, "year"));
                days = (days % DaysInYear);
            }
            // Number of months
            if (days >= DaysInMonth)
            {
                var months = (days / DaysInMonth);
                values.Add(CreateValueString(months, "month"));
                days = (days % DaysInMonth);
            }
            // Number of days
            if (days >= 1)
                values.Add(CreateValueString(days, "day"));
            // Number of hours
            if (span.Hours >= 1)
                values.Add(CreateValueString(span.Hours, "hour"));
            // Number of minutes
            if (span.Minutes >= 1)
                values.Add(CreateValueString(span.Minutes, "minute"));
            // Number of seconds (include when 0 if no other components included)
            if (span.Seconds >= 1 || values.Count == 0)
                values.Add(CreateValueString(span.Seconds, "second"));

            // Combine values into string
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < values.Count; i++)
            {
                if (builder.Length > 0)
                    builder.Append((i == (values.Count - 1)) ? " and " : ", ");
                builder.Append(values[i]);
            }
            // Return result
            return builder.ToString();
        }

        public static string HumanizeList(this IEnumerable<string> list)
        {
            var builder = new StringBuilder();
            var enumerable = list as string[] ?? list.ToArray();
            for (var i = 0; i < enumerable.Count() ; i++)
            {
                if (builder.Length > 0)
                    builder.Append((i == (enumerable.Count() - 1)) ? " and " : ", ");
                builder.Append(enumerable.ElementAt(i));
            }
            // Return result
            return builder.ToString();
        }

        private static string CreateValueString(double value, string description)
        {
            return string.Format("{0:#,##0} {1}",
                value, ((int) value == 1) ? description : $"{description}s");
        }

        
        public static Task SaveAsync(this ZipFile zipFile, string filePath)
        {
            zipFile.Save(filePath); 
            return Task.FromResult(true);
        }

        public static string formatHtmlText(this string studyMaterialText)
        {
            var text = System.Net.WebUtility.HtmlDecode(studyMaterialText);

            var mainDoc = new HtmlDocument();
            mainDoc.LoadHtml(text);

            var cleanText = mainDoc.DocumentNode.InnerText;
            return cleanText.Trim();
            //return cleanText.Replace("&nbsp;", " ");

        }

        public static string getGradeComment(int perc)
        {

            if (perc >= 0 && perc < 40)
            {
                return "You can do better";
            }
            else if (perc >= 40 && perc < 70)
            {
                return "Good job";
            }
            else if (perc == 100)
            {
                return "Flawless performance";
            }
            else
            {
                return "Excellent performance";
            }

            return string.Empty;
        }
    }

    /// <summary>
    /// Class ThreadSafeRandom.
    /// </summary>
    internal static class ThreadSafeRandom
    {
        /// <summary>
        /// The local
        /// </summary>
        [ThreadStatic]
        private static Random _local;

        /// <summary>
        /// Gets the this threads random.
        /// </summary>
        /// <value>The this threads random.</value>
        public static Random ThisThreadsRandom
        {
            get { return _local ?? (_local = new Random(unchecked(Environment.TickCount * 31 + System.Threading.Thread.CurrentThread.ManagedThreadId))); }
        }
    }
}
