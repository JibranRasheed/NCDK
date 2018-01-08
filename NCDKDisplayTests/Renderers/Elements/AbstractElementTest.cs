/* Copyright (C) 2010  Egon Willighagen <egonw@users.sf.net>
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This program is free software; you can redistribute it and/or modify it under
 * the terms of the GNU Lesser General Public License as published by the Free
 * Software Foundation; either version 2.1 of the License, or (at your option)
 * any later version. All we ask is that proper credit is given for our work,
 * which includes - but is not limited to - adding the above copyright notice to
 * the beginning of your source code files, and to any copyright notice that you
 * may distribute with programs based on this work.
 *
 * This program is distributed in the hope that it will be useful, but WITHOUT
 * ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS
 * FOR A PARTICULAR PURPOSE.  See the GNU Lesser General Public License for more
 * details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software Foundation, Inc.,
 * 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Windows.Media;

namespace NCDK.Renderers.Elements
{
    // @cdk.module test-renderbasic
    [TestClass()]
    public abstract class AbstractElementTest
    {
        private static IRenderingElement element;

        protected static void SetRenderingElement(IRenderingElement renderingElement)
        {
            element = renderingElement;
        }

        public class MockVisitor
            : IRenderingVisitor
        {
            internal bool isVisited = false;

            public void Visit(IRenderingElement element)
            {
                isVisited = true;
            }

            public Transform Transform { get => Transform.Identity; set { } }
        }

        [TestMethod()]
        public void TestConstructor()
        {
            Assert.IsNotNull(element);
        }

        [TestMethod()]
        public void TestAccept()
        {
            MockVisitor visitor = new MockVisitor();
            Assert.IsFalse(visitor.isVisited);
            element.Accept(visitor);
            Assert.IsTrue(visitor.isVisited);
        }
    }
}
