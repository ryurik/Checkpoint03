using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CP3Task1
{
    public class TerminalSet : ICollection<Terminal>
    {
        private readonly ICollection<Terminal> _terminals = new List<Terminal>();

        #region ICollection<Terminal>
        public void Add(Terminal item)
        {
            _terminals.Add(item);
        }

        public void Clear()
        {
            _terminals.Clear();
        }

        public bool Contains(Terminal item)
        {
            return _terminals.Contains(item);
        }

        public void CopyTo(Terminal[] array, int arrayIndex)
        {
            _terminals.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return _terminals.Count; }
        }

        public bool IsReadOnly
        {
            get { return _terminals.IsReadOnly; }
        }

        public bool Remove(Terminal item)
        {
            return _terminals.Remove(item);
        }

        public IEnumerator<Terminal> GetEnumerator()
        {
            return _terminals.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
        #endregion
    }
}
