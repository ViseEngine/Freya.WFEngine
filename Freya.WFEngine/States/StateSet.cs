using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Freya.WFEngine
{
    public sealed class StateSet : HashSet<State>
    {
        #region StateNameComparer
        private class StateNameComparer : IComparer<State>, IEqualityComparer<State>
        {
            public StateNameComparer(StringComparer nameComparer) {
                this.nameComparer = nameComparer;
            }

            private readonly StringComparer nameComparer;

            public int Compare(State x, State y) {
                return this.nameComparer.Compare(x.Name, y.Name);
            }

            public bool Equals(State x, State y) {
                if (ReferenceEquals(null, x))
                    return ReferenceEquals(null, y);

                if (ReferenceEquals(null, y))
                    return false;

                return Compare(x, y) == 0;
            }

            public int GetHashCode(State obj) {
                return obj.Name.GetHashCode();
            }
        }
        #endregion

        private static readonly StringComparer stateNameStringComparer = StringComparer.InvariantCulture;

        public StateSet()
            : base(new StateNameComparer(stateNameStringComparer)) {
        }

        public State Add(string stateName) {
            var state = new State(stateName);
            return this.Add(state) ? state : this[stateName];
        }

        public State this[string stateName] {
            get { return this.Single(s => stateNameStringComparer.Equals(stateName, s.Name)); }
        }
    }
}
