using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Promact.Trappist.UnitTests
{
    public class MyTests
    {
        public MyTests()
        {
                
        }

        [Fact]
        public void Add()
        {
            Assert.Equal((2 + 2), 4);
        }
    }
}
