using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CP3Task1
{
    public class PortSet : ICollection<Port>
    {
        private readonly  ICollection<Port> _ports = new List<Port>();

        #region ICollection<Port>
        public void Add(Port item)
        {
            _ports.Add(item);
        }

        public void Clear()
        {
            _ports.Clear();
        }

        public bool Contains(Port item)
        {
            return _ports.Contains(item);
        }

        public void CopyTo(Port[] array, int arrayIndex)
        {
            array.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return _ports.Count; }
        }

        public bool IsReadOnly
        {
            get { return _ports.IsReadOnly; }
        }

        public bool Remove(Port item)
        {
            return _ports.Remove(item);
        }

        public IEnumerator<Port> GetEnumerator()
        {
            return _ports.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
        #endregion
    }
}
