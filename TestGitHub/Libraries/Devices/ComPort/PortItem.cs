using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace TestGitHub.Libraries.Devices.ComPort
{
    public class PortItem
    {
        public PortItem(string caption)
        {
            static string FuncGetPort(string str)
            {
                var header = str.LastIndexOf("(COM", StringComparison.CurrentCulture);
                var footer = str.LastIndexOf(")", StringComparison.CurrentCulture);

                if (header > 0 && footer > 0)
                {
                    header++;

                    return str[header..footer];
                }

                return string.Empty;
            }

            this.Name = caption;
            if (int.TryParse(FuncGetPort(caption), out var result))
            {
                this.Index = result;
            }
        }

        public int Index { get; set; }

        public string Name { get; set; }

        public static void ChangeItems(ObservableCollection<PortItem> src, IEnumerable<PortItem> dst)
        {
            var addItems = dst.Concat(src).Distinct().Except(src, new PortItemStringComparer());

            var removeIndex = src.Select((item, index) => new { Index = index, Value = item }).
                Where(a => (!dst.Any(b => b.Name == a.Value.Name))).Select(_ => _.Index).Reverse();

            foreach (var a in removeIndex)
            {
                src.RemoveAt(a);
            }

            foreach (var a in addItems)
            {
                src.Add(a);
            }
        }

        private class PortItemStringComparer : IEqualityComparer<PortItem>
        {
            /// <inheritdoc/>
            public bool Equals(PortItem src, PortItem dst)
            {
                if (src == null || dst == null)
                {
                    return false;
                }

                return src.Name == dst.Name;
            }

            /// <inheritdoc/>
            public int GetHashCode(PortItem obj)
            {
                return obj?.Name.GetHashCode() ?? 0;
            }
        }
    }
}
