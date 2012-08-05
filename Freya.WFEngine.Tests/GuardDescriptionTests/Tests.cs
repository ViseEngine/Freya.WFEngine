using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Freya.WFEngine.Tests.GuardDescriptionTests
{
    [TestFixture]
    public class Tests
    {
        [ExpectedException(typeof(ArgumentNullException))]
        [Test]
        public void Ctor_Throws_Argn_On_Null_Type() {
            new GuardDescription(null);
        }

        [ExpectedException(typeof(ArgumentException))]
        [Test]
        public void Ctor_Throws_ArgumentException_When_Type_Is_Not_IGuard() {
            new GuardDescription(typeof (Type));
        }

        [Test]
        public void Ctor_Accepts_Generic_IGuard_Type() {
            new GuardDescription(typeof (IGuard<object>));
        }
    }
}
