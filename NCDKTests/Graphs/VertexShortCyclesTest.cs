/*
 * Copyright (c) 2013 European Bioinformatics Institute (EMBL-EBI)
 *                    John May <jwmay@users.sf.net>
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This program is free software; you can redistribute it and/or modify it
 * under the terms of the GNU Lesser General Public License as published by
 * the Free Software Foundation; either version 2.1 of the License, or (at
 * your option) any later version. All we ask is that proper credit is given
 * for our work, which includes - but is not limited to - adding the above
 * copyright notice to the beginning of your source code files, and to any
 * copyright notice that you may distribute with programs based on this work.
 *
 * This program is distributed in the hope that it will be useful, but WITHOUT
 * Any WARRANTY; without even the implied warranty of MERCHANTABILITY or
 * FITNESS FOR A PARTICULAR PURPOSE.  See the GNU Lesser General Public
 * License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 U
 */
using NCDK.Common.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static NCDK.Graphs.InitialCyclesTest;

namespace NCDK.Graphs
{
    // @author John May
    // @cdk.module test-core
    [TestClass()]
    public class VertexShortCyclesTest
    {

        [TestMethod()]
        public virtual void Paths_norbornane()
        {
            int[][] norbornane = Norbornane;
            VertexShortCycles vsc = new VertexShortCycles(norbornane);
            int[][] paths = vsc.GetPaths();
            int[][] expected = new int[][] { new[] { 5, 6, 2, 1, 0, 5 }, new[] { 5, 6, 2, 3, 4, 5 } };
            Assert.IsTrue(Compares.AreOrderLessDeepEqual(expected, paths));
        }

        [TestMethod()]
        public virtual void Paths_bicyclo()
        {
            int[][] bicyclo = Bicyclo;
            VertexShortCycles vsc = new VertexShortCycles(bicyclo);
            int[][] paths = vsc.GetPaths();
            int[][] expected = new int[][] { new[] { 5, 0, 1, 2, 3, 4, 5 }, new[] { 5, 0, 1, 2, 7, 6, 5 }, new[] { 5, 4, 3, 2, 7, 6, 5 } };
            Assert.IsTrue(Compares.AreOrderLessDeepEqual(expected, paths));
        }

        [TestMethod()]
        public virtual void Paths_napthalene()
        {
            int[][] napthalene = Naphthalene;
            VertexShortCycles vsc = new VertexShortCycles(napthalene);
            int[][] paths = vsc.GetPaths();
            int[][] expected = new int[][] { new[] { 5, 0, 1, 2, 3, 4, 5 }, new[] { 5, 4, 7, 8, 9, 6, 5 } };
            Assert.IsTrue(Compares.AreOrderLessDeepEqual(expected, paths));
        }

        [TestMethod()]
        public virtual void Paths_anthracene()
        {
            int[][] anthracene = Anthracene;
            VertexShortCycles vsc = new VertexShortCycles(anthracene);
            int[][] paths = vsc.GetPaths();
            int[][] expected = new int[][] 
            {
                new[] { 5, 0, 1, 2, 3, 4, 5 },
                new[] { 9, 6, 5, 4, 7, 8, 9 },
                new[] { 9, 8, 10, 11, 12, 13, 9 }
            };
            Assert.IsTrue(Compares.AreOrderLessDeepEqual(expected, paths));
        }

        [TestMethod()]
        public virtual void Paths_cyclophane_even()
        {
            int[][] cyclophane_even = CyclophaneEven;
            VertexShortCycles vsc = new VertexShortCycles(cyclophane_even);
            int[][] paths = vsc.GetPaths();
            int[][] expected = new int[][]
            {
                new[] {3, 2, 1, 0, 5, 4, 3},
                new[] {3, 6, 7, 8, 9, 10, 11, 0, 1, 2, 3},
                new[] {3, 6, 7, 8, 9, 10, 11, 0, 5, 4, 3},
            };
            Assert.IsTrue(Compares.AreOrderLessDeepEqual(expected, paths));
        }

        [TestMethod()]
        public virtual void Paths_cyclophane_odd()
        {
            int[][] cyclophane_even = CyclophaneEven;
            VertexShortCycles vsc = new VertexShortCycles(cyclophane_even);
            int[][] paths = vsc.GetPaths();
            int[][] expected = new int[][]{
                new[] {3, 2, 1, 0, 5, 4, 3},
                new[] {3, 6, 7, 8, 9, 10, 11, 0, 1, 2, 3},
                new[] {3, 6, 7, 8, 9, 10, 11, 0, 5, 4, 3}};
            Assert.IsTrue(Compares.AreOrderLessDeepEqual(expected, paths));
        }

        [TestMethod()]
        public virtual void Size_norbornane()
        {
            int[][] norbornane = Norbornane;
            VertexShortCycles vsc = new VertexShortCycles(norbornane);
            int[][] paths = vsc.GetPaths();
            Assert.AreEqual(2, paths.Length);
        }

        [TestMethod()]
        public virtual void Size_bicyclo()
        {
            int[][] bicyclo = Bicyclo;
            VertexShortCycles vsc = new VertexShortCycles(bicyclo);
            Assert.AreEqual(3, vsc.Count);
        }

        [TestMethod()]
        public virtual void Size_napthalene()
        {
            int[][] napthalene = Naphthalene;
            VertexShortCycles vsc = new VertexShortCycles(napthalene);
            Assert.AreEqual(2, vsc.Count);
        }

        [TestMethod()]
        public virtual void Size_anthracene()
        {
            int[][] anthracene = Anthracene;
            VertexShortCycles vsc = new VertexShortCycles(anthracene);
            Assert.AreEqual(3, vsc.Count);
        }

        [TestMethod()]
        public virtual void Size_cyclophane_even()
        {
            int[][] cyclophane_even = CyclophaneEven;
            VertexShortCycles vsc = new VertexShortCycles(cyclophane_even);
            Assert.AreEqual(3, vsc.Count);
        }

        [TestMethod()]
        public virtual void Size_cyclophane_odd()
        {
            int[][] cyclophane_even = CyclophaneEven;
            VertexShortCycles vsc = new VertexShortCycles(cyclophane_even);
            Assert.AreEqual(3, vsc.Count);
        }

        [TestMethod()]
        public virtual void Paths_cyclophanelike1()
        {
            int[][] g = Cyclophanelike1();
            VertexShortCycles vsc = new VertexShortCycles(g);
            int[][] paths = vsc.GetPaths();
            int[][] expected = new int[][]
            {
                new[] {5, 0, 1, 2, 3, 4, 5},
                new[] {8, 7, 6, 5, 10, 9, 8},
                new[] {13, 12, 11, 8, 19, 18, 13},
                new[] {13, 14, 15, 2, 17, 16, 13}
            };
            Assert.IsTrue(Compares.AreOrderLessDeepEqual(expected, paths));
        }

        [TestMethod()]
        public virtual void Paths_cyclophanelike2()
        {
            int[][] g = Cyclophanelike2();
            VertexShortCycles vsc = new VertexShortCycles(g);
            int[][] paths = vsc.GetPaths();
            int[][] expected = new int[][]
            {
                new[] {5, 0, 1, 2, 3, 4, 5},
                new[] {9, 8, 7, 6, 11, 10, 9},
                new[] {15, 14, 13, 12, 17, 16, 15},
                new[] {21, 20, 19, 18, 23, 22, 21}
            };
            Assert.IsTrue(Compares.AreOrderLessDeepEqual(expected, paths));
        }

        /// <summary>
        // @cdk.inchi InChI=1/C20H32/c1-2-18-6-3-17(1)4-7-19(8-5-17)13-15-20(11-9-18,12-10-18)16-14-19/h1-16H2
        /// </summary>
        static int[][] Cyclophanelike1()
        {
            return new int[][]{ new[] {1, 5}, new[] {0, 2}, new[] {1, 3, 15, 17}, new[] {2, 4}, new[] {3, 5}, new[] {4, 0, 6, 10}, new[] {5, 7}, new[] {6, 8},
                new[] {7, 9, 11, 19}, new[] {8, 10}, new[] {9, 5}, new[] {8, 12}, new[] {11, 13}, new[] {12, 14, 16, 18}, new[] {13, 15}, new[] {14, 2}, new[] {13, 17},
                new[] {16, 2}, new[] {13, 19}, new[] {18, 8}};
        }

        /// <summary>
        // @cdk.inchi InChI=1/C24H40/c1-2-18-4-3-17(1)19-5-7-21(8-6-19)23-13-15-24(16-14-23)22-11-9-20(18)10-12-22/h17-24H,1-16H2
        /// </summary>
        static int[][] Cyclophanelike2()
        {
            return new int[][]{ new[] {1, 5}, new[] {0, 2}, new[] {1, 3, 21}, new[] {2, 4}, new[] {3, 5}, new[] {4, 0, 6}, new[] {5, 7, 11}, new[] {6, 8}, new[] {7, 9},
                new[] {8, 10, 12}, new[] {9, 11}, new[] {10, 6}, new[] {9, 13, 17}, new[] {12, 14}, new[] {13, 15}, new[] {14, 16, 18}, new[] {15, 17}, new[] {16, 12},
                new[] {15, 19, 23}, new[] {18, 20}, new[] {19, 21}, new[] {20, 2, 22}, new[] {21, 23}, new[] {22, 18}};
        }
    }
}

