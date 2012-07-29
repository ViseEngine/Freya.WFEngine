using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Freya.WFEngine.Tests
{
    public class SpecificationBase
    {
        [SetUp]
        public void SetUp() {
            Given();
            When();
        }

        public virtual void Given() {
        }

        public virtual void When() {
        }
    }
}
